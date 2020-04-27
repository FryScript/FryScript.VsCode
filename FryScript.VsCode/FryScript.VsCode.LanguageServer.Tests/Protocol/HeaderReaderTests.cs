using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests.Protocol
{
    [TestClass]
    public class HeaderReaderTests
    {
        private HeaderReader _headerReader;
        private TextReader _textReader;

        [TestInitialize]
        public void Initialize()
        {
            _textReader = Substitute.For<TextReader>();

            _headerReader = new HeaderReader(_textReader);
        }

        [TestMethod]
        [ExpectedException(typeof(HeaderException))]
        public async Task Read_Header_Null_Content_Length_Header_Exception()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult<string>(null));

            await _headerReader.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(HeaderException))]
        public async Task Read_Header_Malformed_Content_Length_Exception()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult<string>("Content-Length 100"));

            await _headerReader.Read();
        }

        [TestMethod]
        public async Task Read_Header_Success()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult<string>("Content-Length: 100\r\n"));

            var result = await _headerReader.Read();

            Assert.AreEqual(100, result);

            await _textReader.Received(2).ReadLineAsync();
        }
    }
}
