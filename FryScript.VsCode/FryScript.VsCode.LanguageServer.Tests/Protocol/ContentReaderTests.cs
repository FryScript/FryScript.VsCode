using System;
using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FryScript.VsCode.LanguageServer.Tests.Protocol
{
    [TestClass]
    public class ContentReaderTests
    {
        private ContentReader _contentReader;
        private TextReader _textReader;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _textReader = Substitute.For<TextReader>();
            _contentReader = new ContentReader(_textReader);
        }

        [TestMethod]
        public async Task Read_Success()
        {
            int contentLength = 10;

            _textReader
                .ReadAsync(Arg.Do<char[]>(c => Array.Fill(c, 'T', 0, contentLength)), 0, contentLength)
                .Returns(Task.FromResult(contentLength));

            var result = await _contentReader.Read(contentLength);

            Assert.AreEqual("TTTTTTTTTT", result);
        }
    }
}