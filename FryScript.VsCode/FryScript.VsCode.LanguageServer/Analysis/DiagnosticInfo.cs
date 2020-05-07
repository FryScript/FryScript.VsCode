namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class DiagnosticInfo
    {
        public int Line { get; }

        public int Column { get; }

        public string Message { get; } = string.Empty;

        public DiagnosticType DiagnosticType { get; }

        public DiagnosticInfo(int line, int column, string message, DiagnosticType diagnosticType)
            => (Line, Column, Message, DiagnosticType) = (line, column, message, diagnosticType);
    }
}