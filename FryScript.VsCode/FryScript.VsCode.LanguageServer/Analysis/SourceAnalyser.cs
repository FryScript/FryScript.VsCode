using FryScript.Ast;
using FryScript.Compilation;
using FryScript.VsCode.LanguageServer.Protocol;
using Irony.Parsing;
using System;
using System.Linq;
using Range = FryScript.VsCode.LanguageServer.Protocol.Range;

namespace FryScript.VsCode.LanguageServer.Analysis
{
    public class SourceAnalyser : ISourceAnalyser
    {
        private readonly IScriptRuntime _runtime;
        private readonly IScriptCompiler _compiler;
        private readonly Func<Uri, string, IRootNode, ISourceInfo> _sourceInfoFactory;

        public SourceAnalyser(IScriptRuntime runtime, IScriptCompiler compiler, Func<Uri, string, IRootNode, ISourceInfo> sourceInfoFactory)
            => (_runtime, _compiler, _sourceInfoFactory) = (runtime, compiler, sourceInfoFactory);

        public ISourceInfo GetInfo(Uri uri, string source)
        {
            if(string.IsNullOrWhiteSpace(source))
                return _sourceInfoFactory(uri, string.Empty, new ScriptNode());
                
            try
            {
                var context = new CompilerContext(_runtime, uri);

                _compiler.Compile(source, uri.AbsolutePath, context);

                var info = _sourceInfoFactory(uri, source, context.RootNode);

                info.Scope = context.Scope;

                return info;
            }
            catch (FryScriptException ex)
            {
                var info = _sourceInfoFactory(uri, source, new ScriptNode());

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