using System;
using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Schema;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IHeaderReader _headerReader;
        private readonly IContentReader _contentReader;
        private readonly IProtocolMethods _protocolMethods;
        private readonly TextWriter _textWriter;

        public RequestHandler(IHeaderReader headerReader, IContentReader contentReader, IProtocolMethods protocolMethods, TextWriter textWriter)
            => (_headerReader, _contentReader, _protocolMethods, _textWriter) = (headerReader, contentReader, protocolMethods, textWriter);

        public async Task Handle()
        {
            var contentLength = await _headerReader.Read();
            var content = await _contentReader.Read(contentLength);
        }
    }
}
