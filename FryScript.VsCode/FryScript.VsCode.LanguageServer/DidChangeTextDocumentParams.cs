using System;

namespace FryScript.VsCode.LanguageServer
{
    public class DidChangeTextDocumentParams
    {
        public VersionedTextDocumentIdentifier TextDocument { get; set; } = new VersionedTextDocumentIdentifier();

        public TextDocumentContentChangeEvent[] ContentChanges {get;set;} = Array.Empty<TextDocumentContentChangeEvent>();
    }
}