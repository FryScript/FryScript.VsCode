using FryScript.Compilation;
using FryScript.HostInterop;
using FryScript.Parsing;
using FryScript.ScriptProviders;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Constants;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public class LSPMethods : ProtocolMethodsBase
    {
        private readonly ISourceManager _sourceManager;

        public LSPMethods()
        {
            var parser = new ScriptParser();
            var runtime = new ScriptRuntime(
                new DirectoryScriptProvider(Environment.CurrentDirectory),
                new ScriptCompiler(parser, new ScriptParser(FryScriptLanguageData.LanguageData.Grammar.SnippetRoots.Single(n => n.Name == NodeNames.Expression))),
                new ObjectRegistry(),
                new ScriptObjectFactory(),
                new TypeProvider());

            _sourceManager = new SourceManager(new SourceAnalyser(runtime, parser, (uri, astNode) => new SourceInfo(uri, astNode)));
        }

        [ProtocolMethod("initialize")]
        public InitializeResult Initialize(InitializeParams @params)
        {
            return new InitializeResult(new ServerCapabilities
            {
                CompletionProvider = new CompletionOptions
                {
                    TriggerCharacters = new[] { "." },
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
            _sourceManager.Open(@params.TextDocument.Uri ?? Uris.Empty, @params.TextDocument.Text);

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
            var changes = @params.ContentChanges[0];
            var uri = @params.TextDocument.Uri ?? Uris.Empty;

            _sourceManager.Update(uri, @params.ContentChanges[0].Text);

            var info = _sourceManager.Update(uri, @params.ContentChanges.Single().Text);

            ClientRequest(new 
            {
                method = "textDocument/publishDiagnostics",
                @params = new 
                {
                    uri = @params.TextDocument.Uri,
                    version = @params.TextDocument.Number,
                    diagnostics = info.Diagnostics
                }
            });

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
