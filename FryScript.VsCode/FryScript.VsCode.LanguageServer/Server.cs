using System;
using System.Threading;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public class Server : IServer
    {
        private readonly IRequestHandler _requestHandler;

        public Server(IRequestHandler requestHandler) 
            => (_requestHandler) = (requestHandler);

        public CancellationToken Start()
        {
            while(true)
            {
                _requestHandler.Handle().Wait();
            }
        }

        public static Server Build(IProtocolMethods protocolMethods)
        {
            return new Server(
                new RequestHandler(new RequestReader(Console.In), 
                protocolMethods, 
                new ResponseWriter(Console.Out)));
        }
    }
}