namespace LanguageServer2.LanguageProtocol
{
    public class DidChangeTextDocumentParams
    {
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        public TextDocumentContentChangeEvent[] ContentChanges { get; set; }
    }
}
