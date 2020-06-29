using System;
using System.Collections.Generic;
using System.Linq;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceInfo : ISourceInfo
    {
        private readonly List<string> _sourceLines;

        private readonly IRootNode _rootNode;

        public Uri Uri { get; }

        public string Source { get; }

        public List<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

        public List<Fragment> Fragments { get; } = new List<Fragment>();

        public bool HasErrors => Diagnostics.Count(i => i.Severity == DiagnosticSeverity.Error) > 0;

        public SourceInfo(Uri uri, string source, IRootNode rootNode) 
            => (Uri, Source, _rootNode, _sourceLines) = (uri, source, rootNode, source.Split("\r\n").Select(s => $"{s}\r\n").ToList());

        public Scope Scope { get; set; } = new Scope();

        public int GetPosition(int line, int column)
        {
            if (line < 0 || line >= Source.Length)
                throw new ArgumentOutOfRangeException(nameof(line));

            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column));

            var sum = 0;

            if (line >= _sourceLines.Count)
                return Source.Length;

            for(var i = 0; i < line; i++)
            {
                sum += _sourceLines[i].Length;
            }

            sum += column;

            return sum;
        }
    }
}