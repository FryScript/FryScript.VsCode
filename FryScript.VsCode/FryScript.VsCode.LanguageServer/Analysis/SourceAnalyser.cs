using System;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;

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
                info.Diagnostics.Add(new DiagnosticInfo(
                    ex.Line ?? 0,
                    ex.Column ?? 0,
                    ex.Message,
                    DiagnosticType.Error
                ));

                return info;
            }
        }
    }
}