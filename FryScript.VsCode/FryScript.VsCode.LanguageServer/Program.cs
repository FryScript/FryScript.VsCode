using FryScript.VsCode.LanguageServer.Protocol;
using System;
using System.Diagnostics;
using System.IO;

namespace FryScript.VsCode.LanguageServer
{
    public class LSPMethods : ProtocolMethodsBase
    {
        [ProtocolMethod("initialize")]
        public object Initialize(object @params)
        {
            SendClient(new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = "Initialize received 1"
                }
            });

            SendClient(new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = "Initialize received 2"
                }
            });

            SendClient(new
            {
                method = "window/showMessage",
                @params = new
                {
                    type = 3,
                    message = "Initialize received 3"
                }
            });
            return new
            {
                capabilities = new { },
                serverInfo = new
                {
                    name = "Server test",
                    version = "0.0.1"
                }
            };
        }

        [ProtocolMethod("initialized")]
        public object? Initialized(object @params)
        {
            SendClient(new
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            var server = Server.Build(new LSPMethods());

            server.Start();
        }
    }
}
