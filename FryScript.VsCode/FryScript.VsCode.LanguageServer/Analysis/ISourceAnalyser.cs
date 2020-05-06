using System;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceAnalyser
    {
        ISourceInfo GetInfo(Uri uri, string source);
    }
}