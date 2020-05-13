using System;
using System.Collections.Generic;
using System.Linq;
using FryScript.Ast;
using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceInfo : ISourceInfo
    {
        private readonly IRootNode _rootNode;

        public Uri Uri { get; }

        public List<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

        public List<Fragment> Fragments { get; } = new List<Fragment>();

        public bool HasErrors => Diagnostics.Count(i => i.Severity == DiagnosticSeverity.Error) > 0;

        public SourceInfo(Uri uri, IRootNode rootNode) => (Uri, _rootNode) = (uri, rootNode);
    }
}