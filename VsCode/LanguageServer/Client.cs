using LanguageServer2.LanguageProtocol;

namespace LanguageServer2
{
    public class Client<T> : IClient
    {
        private IClient _client;

        public Client(IClient client)
        {
            _client = client;
        }

        public void Diagnostics(PublishDiagnosticsParams diagnostics)
        {
            _client.Diagnostics(diagnostics);
        }

        public void Response(T result)
        {
            _client.Response(result);
        }

        void IClient.Response(object result)
        {
            Response((T)result);
        }

        public void ShowMessage(string message)
        {
            _client.ShowMessage(message);
        }
    }
}
