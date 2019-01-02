using LanguageServer2.LanguageProtocol;

namespace LanguageServer2
{
    public interface IClient
    {
        void Response(object result);

        void Diagnostics(PublishDiagnosticsParams @params);

        void ShowMessage(string message);
    }
}
