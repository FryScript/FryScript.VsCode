namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class CompletionItem
    {
        public string Label { get; set; } = string.Empty;

        public CompletionItemKind? Kind { get; set; }

        public CompletionItemTag[]? Tags { get; set; }

        public string? Detail { get; set; }

        public string? Documentation { get; set; }

        public bool? Preselect { get; set; }

        public string? SortText { get; set; }

        public string? FilterText { get; set; }

        public InsertTextFormat? InsertTextFormat { get; set; }

        public TextEdit? TextEdit { get; set; }

        public TextEdit[]? AdditionalTextEdits { get; set; }

        public string[]? CommitCharacters { get; set; }

        public CommandReference? Command { get; set; }

        public object? Data { get; set; }
    }
}