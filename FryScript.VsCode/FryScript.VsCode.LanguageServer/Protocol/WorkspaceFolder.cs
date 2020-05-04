using System;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class WorkspaceFolder
    {
        public Uri? DocumentUri { get; set; }

        public string Name { get; set; }  = string.Empty;
    }
}