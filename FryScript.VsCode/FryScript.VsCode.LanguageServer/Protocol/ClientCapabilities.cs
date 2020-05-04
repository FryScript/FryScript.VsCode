namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ClientCapabilities
    {
        public Workspace? Workspace { get; set; }

        public TextDocumentClientCapabilities? textDocument { get; set; }

        public Window? Window { get; set; }

        public object? Experimental { get; set; }
    }
}