using System;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceAnalyser : ISourceAnalyser
    {
        public SourceInfo GetInfo(Uri uri, string source)
        {
            return new SourceInfo();
        }
    }
}