namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface ITextDocumentPositionParams
    {
        TextDocumentIdentifier TextDocument { get; set; }
    }
}