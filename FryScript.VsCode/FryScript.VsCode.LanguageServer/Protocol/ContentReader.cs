using System;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ContentReader : IContentReader
    {
        private char[] _buffer = new char[1024];

        private readonly TextReader _textReader;

        public ContentReader(TextReader textReader) 
            => (_textReader) = (textReader);

        public async Task<string> Read(int contentLength)
        {
            if(contentLength > _buffer.Length)
                Array.Resize(ref _buffer, contentLength);

            var readLength = await _textReader.ReadAsync(_buffer, 0, contentLength);

            return new string(_buffer, 0, readLength);
        }
    }
}
