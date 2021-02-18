using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.FryScriptExtensions;
using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Constants;
using System;
using System.Linq;

namespace FryScript.VsCode.LanguageServer
{
    public class LanguageServerMethods : ProtocolMethodsBase
    {
        private readonly ISourceManager _sourceManager;

        public LanguageServerMethods()
        {
            var parser = new ScriptParser();
            var compiler = new ScriptCompiler(parser, new ScriptParser(FryScriptLanguageData.LanguageData.Grammar.SnippetRoots.Single(n => n.Name == NodeNames.Expression)));
            var runtime = new AnalysisScriptRuntime();

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
                    message = "FryScript language server started"
                }
            });
            return null;
        }

        [ProtocolMethod("textDocument/completion")]
        public object? TextDocumentCompletion(CompletionParams @params)
        {
            try
            {
                var sourceInfo = _sourceManager.GetInfo(@params.TextDocument?.Uri ?? Uris.Empty);

                var position = sourceInfo.GetPosition(@params.Position.Line, @params.Position.Character);

                var members = sourceInfo.Scope.GetCompletions(position);

                return members.Select(m => new CompletionItem
                {
                    Kind = CompletionItemKind.Field,
                    Label = m.Name
                });
            }
            catch (Exception)
            {
                return Array.Empty<CompletionItem>();
            }
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
}
