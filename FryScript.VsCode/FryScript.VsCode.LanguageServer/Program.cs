using FryScript.VsCode.LanguageServer.Protocol;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public class LSPMethods : ProtocolMethodsBase
    {
        [ProtocolMethod("initialize")]
        public InitializeResult Initialize(InitializeParams @params)
        {
            return new InitializeResult(new ServerCapabilities
            {
                CompletionProvider = new CompletionOptions
                {
                    TriggerCharacters = new[] {"."},
                }
            })
            {
                ServerInfo = new ServerInfo("Test language server"),
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
                    message = "Text document completion reponse"
                }
            });

            return new object();
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
