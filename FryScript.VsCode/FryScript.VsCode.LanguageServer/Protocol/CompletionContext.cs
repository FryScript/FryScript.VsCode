namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class CompletionContext
    {
        public CompletionTriggerKind TriggerKind { get; set; }

        public string? TriggerCharacter { get; set; }
    }
}