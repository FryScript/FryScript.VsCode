using FryScript.Debugging;
using System;

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

        public IScriptObject Import(string name, IScriptObject instance)
        {
            throw new NotImplementedException();
        }

        public IScriptObject New(string name, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}