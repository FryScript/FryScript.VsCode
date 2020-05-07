using System;
using System.Collections.Generic;
using FryScript.Ast;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceInfo : ISourceInfo
    {
        private readonly Uri _uri;
        private readonly IRootNode _rootNode;

        public Uri Uri => _uri;

        public List<DiagnosticInfo> Diagnostics { get; } = new List<DiagnosticInfo>();

        public SourceInfo(Uri uri, IRootNode rootNode) => (_uri, _rootNode) = (uri, rootNode);
    }
}