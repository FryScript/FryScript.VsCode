using System;
using System.Linq;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Debugging;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Protocol;
using Irony.Parsing;
using Range = FryScript.VsCode.LanguageServer.Protocol.Range;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class AnalysisScriptRuntime : IScriptRuntime
    {
        public bool DetailedExceptions { get; set; }
        public DebugHook DebugHook { get; set; } = null!;

        public object Eval(string script)
        {
            throw new NotImplementedException();
        }

        public IScriptObject Get(string name, Uri relativeTo = null!)
        {
            return new ScriptObject();
        }

        public IScriptObject Import(Type type)
        {
            throw new NotImplementedException();
        }

        public IScriptObject New(string name, params object[] args)
        {
            throw new NotImplementedException();
        }
    }

    public class SourceAnalyser : ISourceAnalyser
    {
        private readonly IScriptRuntime _runtime;
        private readonly IScriptParser _parser;
        private readonly Func<Uri, IRootNode, ISourceInfo> _sourceInfoFactory;

        public SourceAnalyser(IScriptRuntime runtime, IScriptParser parser, Func<Uri, IRootNode, ISourceInfo> sourceInfoFactory)
            => (_runtime, _parser, _sourceInfoFactory) = (runtime, parser, sourceInfoFactory);

        public ISourceInfo GetInfo(Uri uri, string source)
        {
            if(string.IsNullOrWhiteSpace(source))
                return _sourceInfoFactory(uri, new ScriptNode());
                
            try
            {
                var rootNode = _parser.Parse(source, uri.AbsoluteUri, new CompilerContext(_runtime, uri, false));

                var compiler = new ScriptCompiler(_parser, _parser);

                var func = compiler.Compile(source, uri.AbsolutePath, new CompilerContext(new AnalysisScriptRuntime(), uri));

                return _sourceInfoFactory(uri, rootNode);
            }
            catch (FryScriptException ex)
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

                if(ex.InternalData != null)
                    info.Fragments.AddRange(((TokenList)ex.InternalData)
                        .Select(t => new Fragment(t.Terminal.Name, t.ValueString, t.Location.Line, t.Location.Column)));

                return info;
            }
        }
    }
}