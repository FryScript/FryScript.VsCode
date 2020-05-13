namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class TextEdit
    {
        public Range Range { get; set; } = new Range();

        public string NewText {get;set;} = string.Empty;
    }
}