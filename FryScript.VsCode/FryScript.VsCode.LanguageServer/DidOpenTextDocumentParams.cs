namespace FryScript.VsCode.LanguageServer
{
    public class DidOpenTextDocumentParams
    {
        public TextDocumentItem TextDocument { get; set; } = new TextDocumentItem();
    }
}