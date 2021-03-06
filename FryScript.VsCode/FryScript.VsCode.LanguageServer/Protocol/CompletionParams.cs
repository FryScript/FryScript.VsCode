﻿namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class CompletionParams :
        ITextDocumentPositionParams,
        IWorkDoneProgressParams,
        IPartialResultParams
    {
        public CompletionContext? Context { get; set; }

        public TextDocumentIdentifier TextDocument { get; set; } = new TextDocumentIdentifier();

        public Position Position { get; set; } = new Position();

        public string? WorkDoneToken { get; set; }

        public string? PartialResultToken { get; set; }
    }
}