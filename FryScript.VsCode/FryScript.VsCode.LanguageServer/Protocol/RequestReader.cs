using System;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Schema;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class RequestReader : IRequestReader
    {
        private readonly IHeaderReader _headerReader;
        private readonly IContentReader _contentReader;

        public RequestReader(IHeaderReader headerReader, IContentReader contentReader)
            => (_headerReader, _contentReader) = (headerReader, contentReader);

        public Task<RequestMessage> Read()
        {
            throw new NotImplementedException();
        }
    }
}
