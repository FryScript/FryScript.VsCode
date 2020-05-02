using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;

namespace FryScript.VsCode.LanguageServer.Tests
{
    [TestClass]
    public class ResponseWriterTests
    {
        private ResponseWriter _responseWriter;
        private TextWriter _textWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            _textWriter = Substitute.For<TextWriter>();
            _responseWriter = new ResponseWriter(_textWriter);
        }

        [TestMethod]
        public async Task Write_Success()
        {
            var response = new ResponseMessage
            {
                Id = 99,
                Result = "Result"
            };

            var expectedJson = JsonConvert.SerializeObject(response);

            await _responseWriter.Write(response);

            await _textWriter.Received().WriteLineAsync($"Content-Length: {expectedJson.Length}");
            await _textWriter.Received().WriteLineAsync();
            await _textWriter.Received().WriteAsync(expectedJson);
        }

        [TestMethod]
        public async Task Write_Handles_Null_Response()
        {
            await _responseWriter.Write(null);

            await _textWriter.DidNotReceive().WriteLineAsync();
            await _textWriter.DidNotReceive().WriteAsync(Arg.Any<string>());
        }
    }
}