namespace FryScript.VsCode.LanguageServer
{
    public class TextDocumentContentChangeEvent
    {
        public Range Range { get; set; } = new Range();

        public int? RangeLength { get; set; }

        public string Text {get;set;} = string.Empty;
    }
}