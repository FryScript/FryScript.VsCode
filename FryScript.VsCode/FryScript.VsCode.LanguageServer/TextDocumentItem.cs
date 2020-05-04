using System;

namespace FryScript.VsCode.LanguageServer
{
    public class TextDocumentItem
    {
        public Uri? Uri { get; set; }

        public string LanguageId { get; set; } = string.Empty;

        public int Version { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}