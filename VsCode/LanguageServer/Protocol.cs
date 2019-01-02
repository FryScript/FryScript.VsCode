using Newtonsoft.Json.Linq;
using System;

namespace LanguageServer2
{
    //public class Protocol<T> : IProtocol
    //{
    //    private readonly Action<RequestContext<T>> _method;

    //    public Protocol(Action<RequestContext<T>> method)
    //    {
    //        _method = method;
    //    }

    //    public void Execute(IClient connection, JToken request)
    //    {
    //        var @params = request.ToObject<T>();

    //        var context = new RequestContext<T>(connection, @params);

    //        _method(context);
    //    }
    //}

    public class Protocol<TRequest, TResponse> : IProtocol
    {
        private readonly Action<TRequest, Client<TResponse>> _action;

        public Protocol(Action<TRequest, Client<TResponse>> action)
        {
            _action = action;
        }

        public void Execute(IClient client, JToken request)
        {
            var @params = request.ToObject<TRequest>();

            _action(@params, new Client<TResponse>(client));
        }
    }
}
