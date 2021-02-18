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
            _responseWriter.WriteAsync(obj);
        }

        public async Task HandleAsync()
        {
            var requestMessage = await _requestReader.ReadAsync();
            var responseMessage = await _protocolMethods.ExecuteAsync(requestMessage);
            await _responseWriter.WriteAsync(responseMessage);
        }
    }
}
