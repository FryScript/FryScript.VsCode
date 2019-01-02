using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LanguageServer2
{
    public class HeaderReader
    {
        private static readonly string[] _headerDelimiters = new[] { ": " };
        private readonly StringBuilder _sb = new StringBuilder();
        private readonly List<string> _headerTexts = new List<string>();
        private readonly TextReader _reader;

        public HeaderReader(TextReader reader)
        {
            _reader = reader;
        }

        public RequestHeaders Read()
        {
            while (true)
            {
                while (_reader.Peek() != '\r')
                {
                    _sb.Append((char)_reader.Read());
                }

                _sb.Append((char)_reader.Read());
                _sb.Append((char)_reader.Read());

                var text = _sb.ToString();
                _sb.Clear();

                if (text != "\r\n")
                {
                    _headerTexts.Add(text);
                    continue;
                }

                var headerValues = _headerTexts.Select(h => h.Split(_headerDelimiters, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(k => k[0], v => v[1]);
                _headerTexts.Clear();

                var headers = new RequestHeaders();

                string contentLength;
                if (headerValues.TryGetValue("Content-Length", out contentLength))
                    headers.ContentLength = int.Parse(contentLength);

                return headers;
            }
        }
    }
}
