namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class Workspace
    {
        public bool? ApplyEdit { get; set; }

        public WorkspaceEditClientCapabilities? WorkspaceEdit { get; set; }

        public DidChangeConfigurationClientCapabilities? didChangeConfiguration { get; set; }

        public DidChangeWatchedFilesClientCapabilities? didChangeWatchedFiles { get; set; }

        public WorkspaceSymbolClientCapabilities? Symbol { get; set; }

        public ExecuteCommandClientCapabilities? ExecuteCommand { get; set; }

        public bool? WorkspaceFolders { get; set; }

        public bool? Configuration { get; set; }
    }
}