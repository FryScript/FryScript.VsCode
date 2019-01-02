namespace LanguageServer2.LanguageProtocol
{
    public class Diagnostic
    {
        public Range Range;

        public DiagnosticSeverity Severity;

        public int Code;

        public string Source;

        public string Message;

        DiagnosticRelatedInformation[] RelatedInformation;
    }
}
