using System;
using FryScript.VsCode.LanguageServer.Protocol.Constants;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class PublishDiagnosticsParams
    {
        public Uri Uri { get; set; } = Uris.Empty;

        public int? Version { get; set; }

        public Diagnostic[] Diagnostic { get; set; } = Array.Empty<Diagnostic>();
    }
}