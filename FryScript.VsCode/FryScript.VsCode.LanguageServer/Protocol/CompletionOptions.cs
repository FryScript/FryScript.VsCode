namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class CompletionOptions
    {
        public string[]? TriggerCharacters { get; set; }

        public string[]?AllCommitCharacters { get; set; }

        public bool? ResolveProvider { get; set; }
    }
}