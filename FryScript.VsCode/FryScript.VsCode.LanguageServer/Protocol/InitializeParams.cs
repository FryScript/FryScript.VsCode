using System;
using FryScript.VsCode.LanguageServer.Protocol.Constants;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class InitializeParams
    {
        public int? ProcessId { get; set; }

        public ClientInfo? clientInfo { get; set; }

        public string? RootPath { get; set; }

        public Uri? RootUri { get; set; }

        public object? InitializationOptions { get; set; }

        public ClientCapabilities Capabilities { get; set; } = new ClientCapabilities();

        public string Trace { get; set; } = TraceSettings.Off;

        public WorkspaceFolder[]? WorkspaceFolders { get; set; }
    }
}