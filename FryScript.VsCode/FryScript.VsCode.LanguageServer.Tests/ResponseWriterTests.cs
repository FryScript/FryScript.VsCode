using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Constants;
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
        public async Task WriteAsync_Success()
        {
            var response = new ResponseMessage
            {
                Id = 99,
                Result = "Result"
            };

            var expectedJson = JsonConvert.SerializeObject(response, JsonOptions.CamelCase);

            await _responseWriter.WriteAsync(response);

            await _textWriter.Received().WriteLineAsync($"Content-Length: {expectedJson.Length}");
            await _textWriter.Received().WriteLineAsync();
            await _textWriter.Received().WriteAsync(expectedJson);
        }

        [TestMethod]
        public async Task WriteAsync_Handles_Null_Response()
        {
            await _responseWriter.WriteAsync(null);

            await _textWriter.DidNotReceive().WriteLineAsync();
            await _textWriter.DidNotReceive().WriteAsync(Arg.Any<string>());
        }
    }
}