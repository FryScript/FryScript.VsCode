using System;
using System.Collections.Generic;
using FryScript.Compilation;
using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceInfo
    {
        Uri Uri { get; }

        List<Diagnostic> Diagnostics { get; }

        List<Fragment> Fragments { get; }
        
        bool HasErrors { get; }

        Scope Scope { get; set; }

        int GetPosition(int line, int column);
    }
}