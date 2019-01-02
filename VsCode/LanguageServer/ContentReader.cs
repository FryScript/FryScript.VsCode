using System;
using System.IO;

namespace LanguageServer2
{
    public class ContentReader
    {
        private readonly TextReader _reader;
        private char[] _buffer = new char[1024];

        public ContentReader(TextReader reader)
        {
            _reader = reader;
        }

        public string Read(int length)
        {
            if (length >= _buffer.Length)
                Array.Resize(ref _buffer, length * 2);

            _reader.ReadBlock(_buffer, 0, length);

            return new string(_buffer, 0, length);
        }
    }
}
