namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class TextDocumentSyncOptions
    {
        public bool? OpenClose { get; set; }

        public TextDocumentSyncKind? Change { get; set; }
    }
}