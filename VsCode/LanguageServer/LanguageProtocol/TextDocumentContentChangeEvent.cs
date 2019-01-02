namespace LanguageServer2.LanguageProtocol
{
    public class TextDocumentContentChangeEvent
    {
        public Range Range { get; set; }

        public int RangeLength { get; set; }

        public string Text { get; set; }
    }
}
