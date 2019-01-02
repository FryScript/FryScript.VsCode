namespace LanguageServer2.LanguageProtocol
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        public int Version { get; set; }
    }
}
