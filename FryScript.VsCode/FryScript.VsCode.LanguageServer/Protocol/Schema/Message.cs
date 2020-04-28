using FryScript.VsCode.LanguageServer.Protocol.Constants;

namespace FryScript.VsCode.LanguageServer.Protocol.Schema
{
    public abstract class Message
    {
        public string jsonrpc => JsonRpc.Version;
    }
}