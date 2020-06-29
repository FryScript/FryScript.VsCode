using FryScript.Ast;
using FryScript.VsCode.LanguageServer.Analysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;

namespace FryScript.VsCode.LanguageServer.Tests.Analysis
{
    [TestClass]
    public class SourceInfoTests
    {
        private SourceInfo _sourceInfo;
        private Uri _uri;
        private string _source;
        private IRootNode _rootNode;

        [TestInitialize]
        public void TestInitialize()
        {
            _uri = new Uri("test://source-info-tests");
            _source = "this is the\r\ntest source\r\ntext!";
            _rootNode = Substitute.For<IRootNode>();
            _sourceInfo = new SourceInfo(_uri, _source, _rootNode);
        }

        [TestMethod]
        public void GetPosition_Success()
        {
            var result = _sourceInfo.GetPosition(1, 3);

            Assert.AreEqual(16, result);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(90000)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetPosition_Line_Out_Of_Range(int line)
        {
            _sourceInfo.GetPosition(line, 0);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetPosition_Column_Out_Of_Range()
        {
            _sourceInfo.GetPosition(0, -1);
        }
    }
}
