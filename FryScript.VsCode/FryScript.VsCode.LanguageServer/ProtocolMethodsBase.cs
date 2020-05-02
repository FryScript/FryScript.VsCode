using FryScript.VsCode.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public abstract class ProtocolMethodsBase : IProtocolMethods
    {
        private abstract class MethodInvoker
        {
            public abstract object? Invoke(JObject? request);
        }

        private class MethodInvoker<TRequest, TResponse> : MethodInvoker
        {
            private readonly Func<TRequest, TResponse> _invoker;

            public MethodInvoker(Func<TRequest, TResponse> invoker) => _invoker = invoker;

            public override object? Invoke(JObject? request) => _invoker((request != null ? request.ToObject<TRequest>() : default)!);
        }

        private static readonly MethodInfo MissingMethodInfo = typeof(ProtocolMethodsBase).GetTypeInfo().GetMethod(nameof(MissingMethod), BindingFlags.NonPublic | BindingFlags.Instance)!;

        private readonly Dictionary<string, MethodInvoker> _methodInvokers = new Dictionary<string, MethodInvoker>();

        public event Action<object>? OnSendClient;

        public Task<ResponseMessage> Execute(RequestMessage requestMessage)
        {
            var methodInvoker = GetMethodInvoker(requestMessage.Method);

            return Task.Run(() => new ResponseMessage
            {
                Id = requestMessage.Id,
                Result = methodInvoker?.Invoke(requestMessage?.Params)
            });
        }

        protected void SendClient(object val)
        {
            OnSendClient?.Invoke(val);
        }

        private MethodInvoker GetMethodInvoker(string method)
        {
            if (_methodInvokers.TryGetValue(method, out MethodInvoker? methodInvoker))
                return methodInvoker;

            var methodInfo = (from m in GetType().GetTypeInfo().DeclaredMethods
                              let pa = m.GetCustomAttribute<ProtocolMethodAttribute>()
                              where pa != null
                              && string.Compare(method, pa.Method, true) == 0
                              select m).SingleOrDefault() ?? MissingMethodInfo;

            var paramType = methodInfo.GetParameters().SingleOrDefault()?.ParameterType;

            if (paramType == null)
                throw new InvalidOperationException($"Protocol method \"{methodInfo.Name}\" must accept exactly one parameter");

            if (methodInfo.ReturnType == typeof(void))
                throw new InvalidOperationException($"Protocol method \"{methodInfo.Name} cannot be void\"");

            var delegateType = typeof(Func<,>)
                .MakeGenericType(paramType,
                methodInfo.ReturnType);

            var del = methodInfo.CreateDelegate(delegateType, this);

            var methodInvokerType = typeof(MethodInvoker<,>)
                .MakeGenericType(paramType, methodInfo.ReturnType);

            methodInvoker = (MethodInvoker)Activator.CreateInstance(methodInvokerType, del)!;

            _methodInvokers.Add(method, methodInvoker);

            return methodInvoker;
        }

        private object? MissingMethod(object val)
        {
            return null;
        }
    }
}
