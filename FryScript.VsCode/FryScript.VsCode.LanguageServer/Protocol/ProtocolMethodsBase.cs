using FryScript.VsCode.LanguageServer.Protocol.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public abstract class ProtocolMethodsBase : IProtocolMethods
    {
        private abstract class MethodInvoker
        {
            public abstract object? Invoke(object? request);
        }

        private class MethodInvoker<TRequest, TResponse> : MethodInvoker
        {
            private readonly Func<TRequest, TResponse> _invoker;

            public MethodInvoker(Func<TRequest, TResponse> invoker) => (_invoker) = (invoker);

            public override object? Invoke(object? request) => _invoker((TRequest)request!);
        }

        private readonly Dictionary<string, MethodInvoker> _methodInvokers = new Dictionary<string, MethodInvoker>();

        public Task<ResponseMessage> Execute(RequestMessage requestMessage)
        {
            var methodInvoker = GetMethodInvoker(requestMessage.Method);

            return Task.Run(() =>new ResponseMessage
            {
                Id = requestMessage.Id,
                Result = methodInvoker?.Invoke(requestMessage?.Params)
            });
        }

        private MethodInvoker GetMethodInvoker(string method)
        {
            if (!_methodInvokers.TryGetValue(method, out MethodInvoker? methodInvoker))
            {
                var results = (from m in GetType().GetTypeInfo().DeclaredMethods
                               from a in m.GetCustomAttributes()
                               let pa = a as ProtocolMethodAttribute
                               where pa != null
                               && string.Compare(method, pa.Method, true) == 0
                               select new { m, pa }).Single();

                var methodInfo = results.m;

                var paramType = methodInfo.GetParameters().First().ParameterType;
                var delegateType = typeof(Func<,>)
                    .MakeGenericType(paramType,
                    methodInfo.ReturnType);

                var del = methodInfo.CreateDelegate(delegateType, this);

                var methodInvokerType = typeof(MethodInvoker<,>)
                    .MakeGenericType(paramType, methodInfo.ReturnType);

                methodInvoker = (MethodInvoker)Activator.CreateInstance(methodInvokerType, del)!;

                _methodInvokers.Add(results.pa.Method, methodInvoker);
            }

            return methodInvoker;
        }
    }
}
