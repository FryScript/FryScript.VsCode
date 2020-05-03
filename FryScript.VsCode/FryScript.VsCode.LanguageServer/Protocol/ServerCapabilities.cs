namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ServerCapabilities
    {
        public TextDocumentSyncOptions? TextDocumentSync { get; set; }

        public CompletionOptions? CompletionProvider { get; set; }

        public HoverOptions? HoverProvider { get; set; }

        public SignatureHelpOptions? SignatureHelpProvider { get; set; }

        public DeclarationOptions? DeclarationProvidre { get; set; }

        public DefinitionOptions? DefinitionProvider { get; set; }

        public TypeDefinitionOptions? TypeDefinitionProvider { get; set; }

        public ImplementationOptions? ImplementationProvider { get; set; }

        public ReferenceOptions? ReferencesProvider { get; set; }

        public DocumentHighlightOptions? DocumentHighlightProvider { get; set; }

        public DocumentSymbolOptions? DocumentSymbolProvider { get; set; }

        public CodeActionOptions? CodeActionProvidre { get; set; }

        public CodeLensOptions? CodeLensProvider { get; set; }

        public DocumentLinkOptions? DocumentLinkProvider { get; set; }

        public DocumentColorOptions? ColorProvider { get; set; }

        public DocumentFormattingOptions? DocumentFormattingProvider { get; set; }

        public DocumentRangeFormattingOptions? DocumentRangeFormattingProvider { get; set; }

        public DocumentOnTypeFormattingOptions? DocumentOnTypeFormattingProvider { get; set; }

        public RenameOptions? RenameProvider { get; set; }

        public FoldingRangeOptions? FoldingRangeProvider { get; set; }

        public ExecuteCommandOptions? ExecuteCommandProvider { get; set; }

        public SelectionRangeOptions? SelectionRangeProvider { get; set; }

        public bool? WorkspaceSymbolProvider { get; set; }

        public Workspace? Workspace { get; set; }

        public object? Experimental { get; set; }
    }
}