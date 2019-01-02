namespace LanguageServer2.LanguageProtocol
{
    public class CompletionItem
    {
        public string Label;

        public CompletionItemKind Kind;

        public string Detail;

        public string Documentation;

        public bool Deprecated;

        public bool Preselect;

        public string SortText;

        public string FilterText;

        public string InsertText;

        public InsertTextFormat InsertTextFormat;

        public TextEdit TextEdit;

        public TextEdit[] AdditionalTextEdits;

        public string[] CommitCharacters;

        public ClientCommand Command;

        public object Data;
    }
}
