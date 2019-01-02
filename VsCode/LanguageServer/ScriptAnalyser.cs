using FryScript;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;
using LanguageServer2.LanguageProtocol;
using System;
using System.Collections.Generic;

namespace LanguageServer2
{
    public class ScriptAnalyser
    {
        private readonly ScriptEngine _scriptEngine;
        private readonly IScriptCompiler _compiler;
        private readonly Dictionary<string, HashSet<string>> _identifiers = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public ScriptAnalyser(ScriptEngine scriptEngine)
        {
            _scriptEngine = scriptEngine;
            _compiler = scriptEngine.Compiler;
        }

        public Diagnostic[] GetDiagnostics(string script, string fileName)
        {
            var context = new CompilerContext(_scriptEngine, fileName);
            AstNode root;
            Diagnostic[] diagnostics = new Diagnostic[0];
            
            try
            {
                root = _compiler.Parser.Parse(script, fileName, context);

                FindIdentifiers(fileName, root);

                _compiler.Compile(script, fileName, context);
            }
            catch(ParserException ex)
            {
                diagnostics = new[]
                {
                    new Diagnostic
                    {
                        Message = ex.Message,
                        Severity = DiagnosticSeverity.Error,
                        Source = "Fry Script",
                        Range = new Range
                        {
                            Start = new Position
                            {
                                Line = ex.Line ?? 0,
                                Character = ex.Column.HasValue ? ex.Column.Value : 0
                            },
                            End = new Position
                            {
                                Line = ex.Line ?? 0,
                                Character = ex.Column.HasValue ? ex.Column.Value + 1: 0
                            }
                        }
                    }
                };
            }
            catch(CompilerException ex)
            {
                diagnostics = new[]
                {
                    new Diagnostic
                    {
                        Message = ex.Message,
                        Severity = DiagnosticSeverity.Error,
                        Source = "Fry Script",
                        Range = new Range
                        {
                            Start = new Position
                            {
                                Line = ex.Line ?? 0,
                                Character = ex.Column.HasValue ? ex.Column.Value : 0
                            },
                            End = new Position
                            {
                                Line = ex.Line ?? 0,
                                Character = ex.Column.HasValue ? ex.Column.Value + 1: 0
                            }
                        }
                    }
                };
            }
            catch(Exception)
            {
                // Do something here!
            }

            return diagnostics;
        }

        public HashSet<string> GetIdentifiers(string fileName)
        {
            if (!_identifiers.ContainsKey(fileName))
                return new HashSet<string>();

            return _identifiers[fileName];
        }

        private void FindIdentifiers(string fileName, AstNode root)
        {
            _identifiers[fileName] = FindIdentifiersRecurse(root);
        }

        private HashSet<string> FindIdentifiersRecurse(AstNode root, HashSet<string> identifiers = null)
        {
            identifiers = identifiers ?? new HashSet<string>();

            if (root is IdentifierNode identifierNode && !identifiers.Contains(identifierNode.ValueString))
                identifiers.Add(root.ValueString);

            foreach(var childNode in root?.ChildNodes ?? new AstNode[0])
            {
                FindIdentifiersRecurse(childNode, identifiers);
            }

            return identifiers;
        }
    }
}
