namespace FryScript.VsCode.LanguageServer.Protocol.Schema
{
    public class RequestMessage : Message
    {
        public int Id { get; set; }

        public string Method {get;set;} = string.Empty;

        public object? Params {get;set;}
    }
}