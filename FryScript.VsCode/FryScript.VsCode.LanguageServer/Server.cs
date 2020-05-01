using System.Threading;

namespace FryScript.VsCode.LanguageServer
{
    public class Server : IServer
        where TProtocolMethods : IProtocolMethods
    {
        private readonly IProtocolMethods _protocolMethods;

        public Server(IProtocolMethods protocolMethods) => (_protocolMethods) = (protocolMethods);

        public CancellationToken Start()
        {
            throw new System.NotImplementedException();
        }
    }
}