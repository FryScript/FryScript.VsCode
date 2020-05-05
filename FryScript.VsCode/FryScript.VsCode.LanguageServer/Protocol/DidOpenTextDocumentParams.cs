namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class DidOpenTextDocumentParams
    {
        public TextDocumentItem TextDocument { get; set; } = new TextDocumentItem();
    }
}