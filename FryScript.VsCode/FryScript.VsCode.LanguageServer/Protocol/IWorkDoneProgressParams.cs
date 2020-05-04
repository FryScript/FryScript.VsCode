namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IWorkDoneProgressParams
    {
        string? WorkDoneToken { get; set; }
    }
}