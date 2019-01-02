namespace LanguageServer2.LanguageProtocol
{
    public class PublishDiagnosticsParams
    {
        public string Uri;

        public Diagnostic[] Diagnostics;
    }
}
