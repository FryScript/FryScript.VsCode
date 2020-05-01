namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ResponseMessage : Message
    {
        public int Id { get; set; }

        public object? Result { get; set; }
    }
}
