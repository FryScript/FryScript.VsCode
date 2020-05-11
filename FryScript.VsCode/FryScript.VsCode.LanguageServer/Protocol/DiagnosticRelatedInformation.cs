namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class DiagnosticRelatedInformation
    {
        public Location Location { get; set; } = new Location();

        public string Message { get; set; } = string.Empty;
    }
}