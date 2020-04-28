using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests.Protocol
{
    [TestClass]
    public class RequestHandlerTests
    {
        private RequestHandler _requestHandler;
        private IHeaderReader _headerReader;
        private IContentReader _contentReader;
        private IProtocolMethods _protocolMethods;
        private TextWriter _textWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            _headerReader = Substitute.For<IHeaderReader>();
            _contentReader = Substitute.For<IContentReader>();
            _protocolMethods = Substitute.For<IProtocolMethods>();
            _textWriter = Substitute.For<TextWriter>();
            _requestHandler = new RequestHandler(_headerReader, _contentReader, _protocolMethods, _textWriter);
        }

        [TestMethod]
        public async Task Handle_Success()
        {
            var expectedRequestJson = "json-request";
            var expectedResponseJson = "json-response";

            _headerReader.Read().Returns(Task.FromResult(expectedRequestJson.Length));
            _contentReader.Read(expectedRequestJson.Length).Returns(Task.FromResult(expectedRequestJson));
            _protocolMethods.Execute(expectedRequestJson).Returns(expectedResponseJson);

            await _textWriter.Received().WriteAsync(expectedResponseJson);
        }
    }
}