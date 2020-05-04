using System;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceAnalyser
    {
        bool TryOpen(Uri uri, out object? obj);

        bool Close(Uri uri);
    }
}