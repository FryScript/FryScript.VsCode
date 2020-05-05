namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        public int? Number { get; set; }
    }
}