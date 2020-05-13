namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class CommandReference
    {
        public string Title { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public object[]? Arguments { get; set; }
    }
}