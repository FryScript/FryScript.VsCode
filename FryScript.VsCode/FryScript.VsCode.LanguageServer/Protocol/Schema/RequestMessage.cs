using Newtonsoft.Json.Linq;

namespace FryScript.VsCode.LanguageServer.Protocol.Schema
{
    public class RequestMessage : Message
    {
        public int Id { get; set; }

        public string Method {get;set;} = string.Empty;

        public JObject? Params {get;set;}
    }
}