using System;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceAnalyser
    {
        SourceInfo GetInfo(Uri uri, string source);
    }
}