using System;

namespace LanguageServer2
{
    public class RequestReader
    {
        private readonly HeaderReader _headerReader;
        private readonly ContentReader _contentReader;

        public RequestReader(HeaderReader headerReader, ContentReader contentReader)
        {
            _headerReader = headerReader;
            _contentReader = contentReader;
        }

        public RequestReader()
            : this(new HeaderReader(Console.In), new ContentReader(Console.In))
        {
        }

        public string Read()
        {
            var headers = _headerReader.Read();
            var content = _contentReader.Read(headers.ContentLength);

            return content;
        }
    }
}
