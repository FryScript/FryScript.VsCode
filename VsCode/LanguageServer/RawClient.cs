using LanguageServer2.LanguageProtocol;

namespace LanguageServer2
{
    public class RawClient : IClient
    {
        private readonly int _id;
        private readonly ResponseHandler _responseHandler;

        public RawClient(int id, ResponseHandler responseHandler)
        {
            _id = id;
            _responseHandler = responseHandler;
        }

        public void Diagnostics(PublishDiagnosticsParams diagnostics)
        {
            var response = new
            {
                method = "textDocument/publishDiagnostics",
                @params = diagnostics
            };

            _responseHandler.Push(response);
        }

        public void Response(object result)
        {
            var response = new ResponseMessage(_id, result);

            _responseHandler.Push(response);
        }

        public void ShowMessage(string message)
        {
            var response = new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = message
                }
            };

            _responseHandler.Push(response);
        }
    }
}
