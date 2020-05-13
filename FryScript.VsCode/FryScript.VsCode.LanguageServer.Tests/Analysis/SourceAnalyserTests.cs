using System;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.Protocol;
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
        private IScriptParser _scriptParser;
        private Func<Uri, IRootNode, ISourceInfo> _sourceInfoFactory;
        private string _source;
        private Uri _uri;

        [TestInitialize]
        public void TestInitialize()
        {
            _source = "source";
            _uri = new Uri("source://info");
            _runtime = Substitute.For<IScriptRuntime>();
            _scriptParser = Substitute.For<IScriptParser>();
            _sourceInfoFactory = Substitute.For<Func<Uri, IRootNode, ISourceInfo>>();
            _sourceAnalyser = new SourceAnalyser(_runtime, _scriptParser, _sourceInfoFactory);
        }

        [TestMethod]
        public void GetInfo_Success()
        {
            var expectedRoot = Substitute.For<IRootNode>();
            var expectedSourceInfo = Substitute.For<ISourceInfo>();

            _scriptParser.Parse(_source, _uri.AbsoluteUri, Arg.Any<CompilerContext>()).Returns(expectedRoot);

            _sourceInfoFactory.Invoke(_uri, expectedRoot).Returns(expectedSourceInfo);

            var result = _sourceAnalyser.GetInfo(_uri, _source);

            Assert.AreEqual(expectedSourceInfo, result);
        }

        [TestMethod]
        public void GetInfo_Handles_Parser_Exception()
        {
            var expectedMessage = "There was an error";
            var expectedLine = 1;
            var expectedColumn = 2;
            var expectedTokenLength = 3;

            _scriptParser.Parse(_source, _uri.AbsoluteUri, Arg.Any<CompilerContext>())
                .Throws(new ParserException(
                    expectedMessage,
                    _uri.AbsoluteUri,
                    expectedLine,
                    expectedColumn,
                    expectedTokenLength));

            _sourceInfoFactory.Invoke(_uri, Arg.Any<IRootNode>()).Returns(new SourceInfo(_uri, new ScriptNode()));

            var result = _sourceAnalyser.GetInfo(_uri, _source);

            Assert.AreEqual(1, result.Diagnostics.Count);
            Assert.AreEqual(1, result.Diagnostics[0].Range.Start.Line);
            Assert.AreEqual(1, result.Diagnostics[0].Range.End.Line);
            Assert.AreEqual(2, result.Diagnostics[0].Range.Start.Character);
            Assert.AreEqual(2 + 3, result.Diagnostics[0].Range.End.Character);
            Assert.AreEqual(expectedMessage, result.Diagnostics[0].Message);
            Assert.AreEqual(DiagnosticSeverity.Error, result.Diagnostics[0].Severity);
        }
    }
}