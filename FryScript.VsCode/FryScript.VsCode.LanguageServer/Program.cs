using FryScript.Compilation;
using FryScript.HostInterop;
using FryScript.Parsing;
using FryScript.ScriptProviders;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.FryScriptExtensions;
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
            var compiler = new ScriptCompiler(parser, new ScriptParser(FryScriptLanguageData.LanguageData.Grammar.SnippetRoots.Single(n => n.Name == NodeNames.Expression)));
            var runtime = new ScriptRuntime(
                new DirectoryScriptProvider(Environment.CurrentDirectory),
                compiler,
                new ObjectRegistry(),
                new ScriptObjectFactory(),
                new TypeProvider());

            _sourceManager = new SourceManager(new SourceAnalyser(runtime, compiler, (uri, source, astNode) => new SourceInfo(uri, source, astNode)));
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
            // ClientRequest(new
            // {
            //     method = "window/showMessage",
            //     @params = new
            //     {
            //         type = 3,
            //         message = $"Completion at {@params.TextDocument.Uri?.AbsolutePath ?? "Unknown uri"} line {@params.Position.Line} character {@params.Position.Character}"
            //     }
            // });
            var sourceInfo = _sourceManager.GetInfo(@params.TextDocument?.Uri ?? Uris.Empty);

            var position = sourceInfo.GetPosition(@params.Position.Line, @params.Position.Character);

            var members = sourceInfo.Scope.GetCompletions(position);

            return members.Select(m => new CompletionItem
            {
                Kind= CompletionItemKind.Field,
                Label = m.Name
            });


            //return sourceInfo
            //    .Fragments
            //    .Where(f => f.Type == "Identifier")
            //    .GroupBy(f => f.Value)
            //    .Select(grp => grp.First())
            //    .Select(f => new CompletionItem
            //    {
            //        Kind = CompletionItemKind.Field,
            //        Label = f.Value
            //    }).ToArray();
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
            
            var uri = @params.TextDocument.Uri ?? Uris.Empty;

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
