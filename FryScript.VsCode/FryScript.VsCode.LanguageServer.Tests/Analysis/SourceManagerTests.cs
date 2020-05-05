using System;
using FryScript.VsCode.LanguageServer.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FryScript.VsCode.LanguageServer.Tests.Analysis
{
    [TestClass]
    public class SourceManagerTests
    {
        private SourceManager _sourceAnalyser;
        private Uri _uri;

        [TestInitialize]
        public void TestInitialize()
        {
            _sourceAnalyser = new SourceManager();
            _uri = new Uri("test://empty");
        }

        [TestMethod]
        public void TryOpen_Uri_Can_Be_Opened()
        {
            Assert.IsTrue(_sourceAnalyser.TryOpen(_uri, out object obj));
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void TryOpen_Uri_Already_Open()
        {
            _sourceAnalyser.TryOpen(_uri, out object obj);
            Assert.IsFalse(_sourceAnalyser.TryOpen(_uri, out obj));
            Assert.IsNull(obj);
        }

        [TestMethod]
        public void Close_Uri_Is_Not_Open()
        {
            Assert.IsFalse(_sourceAnalyser.Close(_uri));
        }

        [TestMethod]
        public void Close_Uri_Is_Open()
        {
            _sourceAnalyser.TryOpen(_uri, out object obj);

            Assert.IsTrue(_sourceAnalyser.Close(_uri));
        }
    }
}