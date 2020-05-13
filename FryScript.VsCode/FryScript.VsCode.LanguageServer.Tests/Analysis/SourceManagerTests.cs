using System;
using System.Collections.Generic;
using FryScript.VsCode.LanguageServer.Analysis;
using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FryScript.VsCode.LanguageServer.Tests.Analysis
{
    [TestClass]
    public class SourceManagerTests
    {
        private SourceManager _sourceManager;
        private ISourceAnalyser _sourceAnalyser;
        private Uri _uri;
        private string _source;
        private ISourceInfo _sourceInfo;

        [TestInitialize]
        public void TestInitialize()
        {
            _sourceAnalyser = Substitute.For<ISourceAnalyser>();
            _sourceManager = new SourceManager(_sourceAnalyser);
            _uri = new Uri("test://empty");
            _source = "source";
            _sourceInfo = Substitute.For<ISourceInfo>();
        }

        [TestMethod]
        public void Open_Uri_Can_Be_Opened()
        {
            Assert.IsTrue(_sourceManager.Open(_uri, _source));
            _sourceAnalyser.Received().GetInfo(_uri, _source);
        }

        [TestMethod]
        public void Open_Uri_Already_Open()
        {
            _sourceManager.Open(_uri, _source);
            Assert.IsFalse(_sourceManager.Open(_uri, _source));
            _sourceAnalyser.Received(1).GetInfo(Arg.Any<Uri>(), Arg.Any<string>());
        }

        [TestMethod]
        public void Close_Uri_Is_Not_Open()
        {
            Assert.IsFalse(_sourceManager.Close(_uri));
        }

        [TestMethod]
        public void Close_Uri_Is_Open()
        {
            _sourceManager.Open(_uri, _source);

            Assert.IsTrue(_sourceManager.Close(_uri));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Update_Uri_Is_Not_Open()
        {
            _sourceManager.Update(_uri, _source);
        }

        [TestMethod]
        public void Update_Success()
        {
            _sourceManager.Open(_uri, _source);

            _sourceAnalyser.GetInfo(_uri, _source).Returns(_sourceInfo);

             var result = _sourceManager.Update(_uri, _source);

            Assert.AreEqual(_sourceInfo, result);
        }

        [TestMethod]
        public void Update_Handles_Info_With_Errors()
        {
            var sourceWithErrors = Substitute.For<ISourceInfo>();
            var expectedFragments = new List<Fragment>
            {
                new Fragment()
            };

            var expectedDiagnostics = new List<Diagnostic>
            {
                new Diagnostic()
            };
            
            sourceWithErrors.HasErrors.Returns(true);
            sourceWithErrors.Fragments.Returns(expectedFragments);
            sourceWithErrors.Diagnostics.Returns(expectedDiagnostics);

            _sourceAnalyser.GetInfo(_uri, _source)
                .Returns(c => _sourceInfo, c => sourceWithErrors);

            _sourceManager.Open(_uri, _source);

            _sourceInfo.HasErrors.Returns(true);
            _sourceInfo.Fragments.Returns(new List<Fragment>());
            _sourceInfo.Diagnostics.Returns(new List<Diagnostic>());

            var result = _sourceManager.Update(_uri, _source);

            Assert.AreEqual(1, result.Fragments.Count);
            Assert.AreEqual(1, result.Diagnostics.Count);
        }
    }
}