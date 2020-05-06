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
            var rootNode = _parser.Parse(source, uri.AbsoluteUri, new CompilerContext(_runtime, uri, false));

            return _sourceInfoFactory(uri, rootNode);
        }
    }
}