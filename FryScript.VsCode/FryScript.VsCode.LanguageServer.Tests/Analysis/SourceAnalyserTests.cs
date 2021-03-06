using System;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.Protocol;
using Irony.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FryScript.VsCode.LanguageServer.Tests.Analysis
{
    [TestClass]
    public class SourceAnalyserTests
    {
        private SourceAnalyser _sourceAnalyser;
        private IScriptRuntime _runtime;
        private IScriptCompiler _scriptCompiler;
        private Func<Uri, string, IRootNode, ISourceInfo> _sourceInfoFactory;
        private string _source;
        private Uri _uri;

        [TestInitialize]
        public void TestInitialize()
        {
            _source = "source";
            _uri = new Uri("source://info");
            _runtime = Substitute.For<IScriptRuntime>();
            _scriptCompiler = Substitute.For<IScriptCompiler>();

            _sourceInfoFactory = Substitute.For<Func<Uri, string, IRootNode, ISourceInfo>>();
            _sourceAnalyser = new SourceAnalyser(_runtime, _scriptCompiler, _sourceInfoFactory);
        }

        [TestMethod]
        public void GetInfo_Success()
        {
            var expectedRoot = Substitute.For<IRootNode>();
            var expectedSourceInfo = Substitute.For<ISourceInfo>();

            _scriptCompiler
                .Compile(
                _source,
                _uri.AbsolutePath,
                Arg.Do<CompilerContext>(c => c.RootNode = expectedRoot));

            _sourceInfoFactory.Invoke(_uri, _source, expectedRoot).Returns(expectedSourceInfo);

            var result = _sourceAnalyser.GetInfo(_uri, _source);

            Assert.AreEqual(expectedSourceInfo, result);
        }

        [TestMethod]
        public void GetInfo_Handles_FryScript_Exception()
        {
            var expectedMessage = "There was an error";
            var expectedLine = 1;
            var expectedColumn = 2;
            var expectedTokenLength = 3;
            var expectedTokens = new TokenList
            {
                new Token(new Terminal("Test"), new SourceLocation(), "Test", null)
            };

            _scriptCompiler.Compile(_source, _uri.AbsolutePath, Arg.Any<CompilerContext>())
                .Throws(new ParserException(
                    expectedMessage,
                    _uri.AbsoluteUri,
                    expectedLine,
                    expectedColumn,
                    expectedTokenLength));

            _sourceInfoFactory.Invoke(_uri, _source, Arg.Any<IRootNode>()).Returns(new SourceInfo(_uri, string.Empty, new ScriptNode()));

            var result = _sourceAnalyser.GetInfo(_uri, _source);

            Assert.AreEqual(1, result.Diagnostics.Count);
            Assert.AreEqual(expectedLine, result.Diagnostics[0].Range.Start.Line);
            Assert.AreEqual(expectedLine, result.Diagnostics[0].Range.End.Line);
            Assert.AreEqual(expectedColumn, result.Diagnostics[0].Range.Start.Character);
            Assert.AreEqual(expectedLine + expectedTokenLength + 1, result.Diagnostics[0].Range.End.Character);
            Assert.AreEqual(expectedMessage, result.Diagnostics[0].Message);
            Assert.AreEqual(DiagnosticSeverity.Error, result.Diagnostics[0].Severity);
            Assert.AreEqual(0, result.Fragments.Count);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("  ")]
        public void GetInfo_Handles_Empty_Source(string source)
        {
            var expectedSourceInfo = new SourceInfo(_uri, string.Empty, new ScriptNode());
            _sourceInfoFactory.Invoke(_uri, string.Empty, Arg.Any<IRootNode>()).Returns(expectedSourceInfo);

            var result = _sourceAnalyser.GetInfo(_uri, source);

            Assert.AreEqual(expectedSourceInfo, result);
        }
    }
}