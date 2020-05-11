using System;
using System.Collections.Generic;
using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceInfo
    {
        Uri Uri { get; }

        List<Diagnostic> Diagnostics { get; }

        bool HasErrors { get; }
    }
}