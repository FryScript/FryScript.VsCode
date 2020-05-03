namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class InitializeResult
    {
        public ServerCapabilities Capabilities { get; set; }

        public ServerInfo? ServerInfo { get; set; }

        public InitializeResult(ServerCapabilities capabilities) => Capabilities = capabilities;
    }
}
