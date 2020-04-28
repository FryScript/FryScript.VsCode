using FryScript.VsCode.LanguageServer.Protocol.Constants;
using FryScript.VsCode.LanguageServer.Protocol.Exceptions;
using FryScript.VsCode.LanguageServer.Protocol.Schema;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class RequestReader : IRequestReader
    {
        private readonly TextReader _textReader;
        private char[] _buffer = new char[1024];

        public RequestReader(TextReader textReader) => (_textReader) = (textReader);

        public async Task<RequestMessage> Read()
        {
            var contentLine = await _textReader.ReadLineAsync();
            await _textReader.ReadLineAsync();

            var header = contentLine?.Split(Delimiters.Header) 
                ?? throw new HeaderException("Content-Length header was null");

            if (header.Length != 2)
                throw new HeaderException($"Content-Length header was malformed: \"{contentLine}\"");

            if (!int.TryParse(header[1], out int contentLength))
                throw new HeaderException($"Content-Length is not a number: \"{header[1]}\"");

            if (contentLength > _buffer.Length)
                Array.Resize(ref _buffer, contentLength);

            var readLength = await _textReader.ReadAsync(_buffer, 0, contentLength);

            return JsonConvert.DeserializeObject<RequestMessage>(new string(_buffer));
        }
    }
}
