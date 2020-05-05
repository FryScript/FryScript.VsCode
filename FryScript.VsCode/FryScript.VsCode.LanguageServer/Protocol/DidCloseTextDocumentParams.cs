namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class DidCloseTextDocumentParams
    {
        public TextDocumentIdentifier TextDocument { get; set; } = new TextDocumentIdentifier();
    }
}