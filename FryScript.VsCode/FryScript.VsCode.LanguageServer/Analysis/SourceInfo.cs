using System;
using FryScript.Ast;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceInfo : ISourceInfo
    {
        private readonly Uri _uri;
        private readonly IRootNode _rootNode;

        public Uri Uri => _uri;

        public SourceInfo(Uri uri, IRootNode rootNode) => (_uri, _rootNode) = (uri, rootNode);
    }
}