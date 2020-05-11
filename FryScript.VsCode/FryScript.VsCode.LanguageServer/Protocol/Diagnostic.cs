using System;
using FryScript.VsCode.LanguageServer.Protocol.Constants;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class Diagnostic
    {
        public Range Range { get; set; } = new Range();

        public DiagnosticSeverity? Severity { get; set; }

        public object? Code { get; set; }

        public string? Source { get; set; }

        public string Message { get; set; } = string.Empty;

        public DiagnosticTag[]? Tags { get; set; }

        public DiagnosticRelatedInformation[]? RelatedInformation { get; set; }
    }
}