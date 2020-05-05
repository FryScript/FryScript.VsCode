using System;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public interface ISourceManager
    {
        bool Open(Uri uri, string source);

        bool Close(Uri uri);

        void Update(Uri uri, string source);
    }
}