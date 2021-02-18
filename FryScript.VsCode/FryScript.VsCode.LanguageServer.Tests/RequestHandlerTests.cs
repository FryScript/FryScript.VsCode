using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests
{
    [TestClass]
    public class RequestHandlerTests
    {
        private RequestHandler _requestHandler;
        private IRequestReader _requestReader;
        private IProtocolMethods _protocolMethods;
        private IResponseWriter _responseWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            _requestReader = Substitute.For<IRequestReader>();
            _protocolMethods = Substitute.For<IProtocolMethods>();
            _responseWriter = Substitute.For<IResponseWriter>();
            _requestHandler = new RequestHandler(_requestReader, _protocolMethods, _responseWriter);
        }

        [TestMethod]
        public async Task HandleAsync_Success()
        {
            var requestMessage = new RequestMessage();
            var expectedResponse = new ResponseMessage();

            _requestReader.ReadAsync().Returns(Task.FromResult(requestMessage));
            _protocolMethods.ExecuteAsync(requestMessage).Returns(Task.FromResult(expectedResponse));

            await _requestHandler.HandleAsync();

            await _responseWriter.Received().WriteAsync(expectedResponse);
        }
    }
}