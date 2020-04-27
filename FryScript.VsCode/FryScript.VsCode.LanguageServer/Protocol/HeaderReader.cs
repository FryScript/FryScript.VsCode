using FryScript.VsCode.LanguageServer.Protocol.Constants;
using FryScript.VsCode.LanguageServer.Protocol.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class HeaderReader : IHeaderReader
    {
        private readonly TextReader _textReader;

        public HeaderReader(TextReader textReader) => (_textReader) = (textReader);

        public async Task<int> Read()
        {
            var contentLine = await _textReader.ReadLineAsync();
            await _textReader.ReadLineAsync();

            var header = contentLine?.Split(Delimiters.Header) 
                ?? throw new HeaderException("Content-Length header was null");

            if (header.Length != 2)
                throw new HeaderException($"Content-Length header was malformed: \"{contentLine}\"");

            if (!int.TryParse(header[1], out int length))
                throw new HeaderException($"Content-Length is not a number: \"{header[1]}\"");

            return length;
        }
    }
}
