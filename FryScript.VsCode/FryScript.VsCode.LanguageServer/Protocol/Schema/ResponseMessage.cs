namespace FryScript.VsCode.LanguageServer.Protocol.Schema
{
    public class ResponseMessage : Message
    {
        public int Id { get; set; }

        public object? Result { get; set; }
    }
}
