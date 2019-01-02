using System;
using System.Collections.Concurrent;

namespace LanguageServer2
{
    public class ResponseHandler
    {
        private readonly ResponseWriter _writer;
        private readonly ConcurrentQueue<object> _responses = new ConcurrentQueue<object>();

        public ResponseHandler(ResponseWriter writer)
        {
            _writer = writer;
        }

        public ResponseHandler()
            : this(new ResponseWriter(Console.Out))
        {
        }

        public void Handle()
        {
            object response;

            if (!_responses.TryDequeue(out response))
                return;

            _writer.Write(response);
        }

        public void Push(object response)
        {
            _responses.Enqueue(response);
        }
    }
}
