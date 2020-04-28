using System;
using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Schema;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IRequestReader _requestReader;
        private readonly IProtocolMethods _protocolMethods;
        private readonly IResponseWriter _responseWriter;

        public RequestHandler(IRequestReader requestReader, IProtocolMethods protocolMethods, IResponseWriter responseWriter)
            => (_requestReader, _protocolMethods, _responseWriter) = (requestReader, protocolMethods, responseWriter);

        public async Task Handle()
        {
            var requestMessage = await _requestReader.Read();
            var responseMessage = await _protocolMethods.Execute(requestMessage);
            await _responseWriter.Write(responseMessage);
        }
    }
}
