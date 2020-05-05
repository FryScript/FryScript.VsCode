using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Constants;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public class LSPMethods : ProtocolMethodsBase
    {
        private readonly ISourceManager _sourceManager = new SourceManager();

        [ProtocolMethod("initialize")]
        public InitializeResult Initialize(InitializeParams @params)
        {
            return new InitializeResult(new ServerCapabilities
            {
                CompletionProvider = new CompletionOptions
                {
                    TriggerCharacters = new[] {"."},
                },
                TextDocumentSync = new TextDocumentSyncOptions
                {
                    Change = TextDocumentSyncKind.Full,
                    OpenClose = true
                }
            })
            {
                ServerInfo = new ServerInfo("Test language server")
            };
        }

        [ProtocolMethod("initialized")]
        public object? Initialized(object @params)
        {
            ClientRequest(new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = "Server test for version 0.0.1"
                }
            });
            return null;
        }

        [ProtocolMethod("textDocument/completion")]
        public object? TextDocumentCompletion(CompletionParams @params)
        {
            ClientRequest(new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = $"Completion at {@params.TextDocument.Uri?.AbsolutePath ?? "Unknown uri"} line {@params.Position.Line} character {@params.Position.Character}"
                }
            });

            return new object();
        }

        [ProtocolMethod("textDocument/didOpen")]
        public object? TextDocumentDidOpen(DidOpenTextDocumentParams @params)
        {
            _sourceManager.TryOpen(@params.TextDocument.Uri ?? Uris.Empty, out object? obj);

            return null;
        }

        [ProtocolMethod("textDocument/didClose")]
        public object? TextDocumentDidClose(DidCloseTextDocumentParams @params)
        {
            _sourceManager.Close(@params.TextDocument.Uri ?? Uris.Empty);

            return null;
        }

        [ProtocolMethod("textDocument/didChange")]
        public object? TextDocumentDidChange(DidChangeTextDocumentParams @params)
        {
            return null;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            while (!Debugger.IsAttached) { }

            var server = Server.Build(new LSPMethods());

            await server.Start();
        }
    }
}
