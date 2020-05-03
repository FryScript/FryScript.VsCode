using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IRequestReader _requestReader;
        private readonly IProtocolMethods _protocolMethods;
        private readonly IResponseWriter _responseWriter;

        public RequestHandler(IRequestReader requestReader, IProtocolMethods protocolMethods, IResponseWriter responseWriter)
        {
            _requestReader = requestReader;
            _protocolMethods = protocolMethods;
            _responseWriter = responseWriter;

            _protocolMethods.OnClientRequest += _protocolMethods_OnNotification;
        }

        private void _protocolMethods_OnNotification(object obj)
        {
            _responseWriter.Write(obj);
        }

        public async Task Handle()
        {
            var requestMessage = await _requestReader.Read();
            var responseMessage = await _protocolMethods.Execute(requestMessage);
            await _responseWriter.Write(responseMessage);
        }
    }
}
