using System;
using System.Collections.Generic;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceInfo
    {
        Uri Uri { get; }

        List<DiagnosticInfo> Diagnostics { get; }
    }
}