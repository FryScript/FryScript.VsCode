namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ServerInfo
    {
        public string Name { get; set; }

        public string? Version { get; set; }

        public ServerInfo(string name) => Name = name;
    }
}
