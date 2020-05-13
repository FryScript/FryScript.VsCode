using System;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Protocol;
using Range = FryScript.VsCode.LanguageServer.Protocol.Range;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceAnalyser : ISourceAnalyser
    {
        private readonly IScriptRuntime _runtime;
        private readonly IScriptParser _parser;
        private readonly Func<Uri, IRootNode, ISourceInfo> _sourceInfoFactory;

        public SourceAnalyser(IScriptRuntime runtime, IScriptParser parser, Func<Uri, IRootNode, ISourceInfo> sourceInfoFactory)
            => (_runtime, _parser, _sourceInfoFactory) = (runtime, parser, sourceInfoFactory);

        public ISourceInfo GetInfo(Uri uri, string source)
        {
            try
            {
                var rootNode = _parser.Parse(source, uri.AbsoluteUri, new CompilerContext(_runtime, uri, false));

                return _sourceInfoFactory(uri, rootNode);
            }
            catch(ParserException ex)
            {
                var info = _sourceInfoFactory(uri, new ScriptNode());
                info.Diagnostics.Add(new Diagnostic
                {
                    Message = ex.Message,
                    Severity = DiagnosticSeverity.Error,
                    Range = new Range
                    {
                        Start = new Position
                        {
                            Line = ex.Line ?? -1,
                            Character = ex.Column ?? -1
                        },

                        End = new Position
                        {
                            Line = ex.Line ?? -1,
                            Character = ex.Column + ex.TokenLength ?? -1,
                        }
                    }
                });

                return info;
            }
        }
    }
}