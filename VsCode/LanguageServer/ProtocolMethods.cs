using FryScript;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.ScriptProviders;
using LanguageServer2.LanguageProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LanguageServer2
{
    public class ProtocolMethods
    {
        private ScriptAnalyser _analyser;
        private readonly Dictionary<string, IProtocol> _protocols = new Dictionary<string, IProtocol>();

        private class ProtocolMethodAttribute : Attribute
        {
            private readonly string _name;

            public string Name => _name;

            public ProtocolMethodAttribute(string name)
            {
                _name = name;
            }
        }

        public ProtocolMethods()
        {
            _protocols["initialize"] = new Protocol<InitializeParams, object>(new Action<InitializeParams, Client<object>>(Initialize));
            _protocols["initialized"] = new Protocol<ProtocolVoid, ProtocolVoid>((p, c) => Initialized(p, c));
            _protocols["textDocument/didChange"] = new Protocol<DidChangeTextDocumentParams, object>(new Action<DidChangeTextDocumentParams, Client<object>>(TextDocumentDidChange));
            _protocols["textDocument/completion"] = new Protocol<CompletionParams, CompletionItem[]>(new Action<CompletionParams, Client<CompletionItem[]>>(Completion));
        }

        public class ProtocolVoid
        {
        }

        public virtual void Initialize(InitializeParams @params, Client<object> client)
        {
            _analyser = new ScriptAnalyser(new ScriptEngine(
                new ScriptCompiler(new ScriptParser()), ".fry", new List<IScriptProvider> {
                new DirectoryScriptProvider(@params.RootPath)
            }));

            client.Response(new
            {
                capabilities = new
                {
                    textDocumentSync = 1,
                    completionProvider = new
                    {
                        resolveProvider = true,
                        triggerCharacters = new[]
                        {
                            "."
                        }
                    }
                }
            });
        }

        public void Initialized(ProtocolVoid @params, Client<ProtocolVoid> client)
        {
            client.ShowMessage("Server initialized");
        }

        public virtual void TextDocumentDidChange(DidChangeTextDocumentParams @params, Client<object> client)
        {
            var script = @params.ContentChanges.First().Text;

            if (script != null)
            {
                client.Diagnostics(new PublishDiagnosticsParams
                {
                    Uri = @params.TextDocument.Uri,
                    Diagnostics = _analyser.GetDiagnostics(script, @params.TextDocument.Uri)
                });

            }
   
            client.Response(null);
        }

        public virtual void Completion(CompletionParams @params, Client<CompletionItem[]> client)
        {
            var completionItems = _analyser.GetIdentifiers(@params.TextDocument.Uri).Select(i => new CompletionItem
            {
                Label = i,
                Kind = CompletionItemKind.Variable
                
            }).ToArray();

            client.Response(completionItems);
            //client.Response(new[]
            //{
            //    new CompletionItem
            //    {
            //        Label = "Mr"
            //    },
            //    new CompletionItem
            //    {
            //        Label = "McGoo"
            //    }
            //});
        }

        public virtual void Initialize(ProtocolVoid @params, Client<ProtocolVoid> client)
        {
            client.ShowMessage("Server initialized correctly");
        }

        //[ProtocolMethod("initialized")]
        //public virtual void Initialized(RequestContext<object> context)
        //{
        //    context.Connection.ShowMessage("Server was initialized!");
        //}

        //[ProtocolMethod("shutdown")]
        //public virtual void Shutdown(RequestContext<object> context)
        //{
        //    context.Connection.Response(null);
        //}

        //[ProtocolMethod("exit")]
        //public virtual void Exit(RequestContext<object> context)
        //{
        //    context.Connection.ShowMessage("The language server is shutting down!");

        //    Environment.Exit(0);
        //}

        //[ProtocolMethod("textDocument/didChange")]
        //public virtual void TextDocumentDidChange(RequestContext<DidChangeTextDocumentParams> context)
        //{
        //    context.Connection.ShowMessage("Change!");
        //}

        public IProtocol GetMethod(string name)
        {
            IProtocol protocol;
            if (!_protocols.TryGetValue(name, out protocol))
            {
                //var methodInfo = GetType().GetTypeInfo().DeclaredMethods.FirstOrDefault(m => m.GetCustomAttribute<ProtocolMethodAttribute>()?.Name == name);

                //var protocolType = methodInfo?.GetParameters().SingleOrDefault()?.ParameterType.GetTypeInfo().GetGenericArguments()?.FirstOrDefault();

                //if (protocolType == null)
                //{
                return _protocols[name] = new Protocol<ProtocolVoid, ProtocolVoid>((p, c) => { });
                //}

                //var requestType = typeof(RequestContext<>).MakeGenericType(protocolType);
                //var actionType = typeof(Action<>).MakeGenericType(requestType);
                //var del = Delegate.CreateDelegate(actionType, this, methodInfo);

                //_protocols[name] = protocol = Activator.CreateInstance(typeof(Protocol<>).MakeGenericType(protocolType), del) as IProtocol;
            }

            return protocol;
        }
    }
}
