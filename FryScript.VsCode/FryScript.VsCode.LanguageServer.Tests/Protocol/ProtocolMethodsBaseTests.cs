using FryScript.VsCode.LanguageServer.Protocol;
using FryScript.VsCode.LanguageServer.Protocol.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests.Protocol
{
    [TestClass]
    public class ProtocolMethodsBaseTests
    {
        private class TestResponse
        {
            public string Data { get; set; } = string.Empty;
        }

        private class TestRequest
        {
            public string Data { get; set; } = string.Empty;
        }

        private class TestProtocolMethods : ProtocolMethodsBase
        {
            [ProtocolMethod("method/test")]
            public TestResponse Test(TestRequest request)
            {
                return new TestResponse
                {
                    Data = request.Data
                };
            }
        }

        private ProtocolMethodsBase _testProtocolMethods;

        [TestInitialize]
        public void TestInitialize()
        {
            _testProtocolMethods = new TestProtocolMethods();
        }

        [TestMethod]
        public async Task Execute_Success()
        {
            
            var requestMessage = new RequestMessage
            {
                Method = "method/test",
                Params = JObject.FromObject(new TestRequest
                {
                    Data = "test data"
                }),
                Id = 100
            };

            var response = await _testProtocolMethods.Execute(requestMessage);

            Assert.IsInstanceOfType(response.Result, typeof(TestResponse));
            Assert.AreEqual(100, response.Id);
        }

        [TestMethod]
        public async Task Execute_Unhandled_Request()
        {
            var requestMessage = new RequestMessage
            {
                Method = "method/unregistered",
            };

            var response = await _testProtocolMethods.Execute(requestMessage);

            Assert.IsNull(response.Result);
        }
    }
}
