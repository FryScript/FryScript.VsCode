using System;
using FryScript.Ast;
using FryScript.Compilation;
using FryScript.Parsing;
using FryScript.VsCode.LanguageServer.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

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
    }
}