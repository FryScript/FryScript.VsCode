using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer
{
    public class DidCloseTextDocumentParams
    {
        public TextDocumentIdentifier TextDocument { get; set; } = new TextDocumentIdentifier();
    }
}