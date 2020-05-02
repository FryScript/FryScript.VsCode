using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests
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

            [ProtocolMethod("method/missingParameter")]
            public TestResponse MissingParameter()
            {
                return new TestResponse();
            }

            [ProtocolMethod("method/voidReturn")]
            public void VoidReturn(object val)
            {
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Execute_Method_With_No_Parameters()
        {
            await _testProtocolMethods.Execute(new RequestMessage
            {
                Method = "method/missingParameter"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Execute_Method_Void_Return()
        {
            await _testProtocolMethods.Execute(new RequestMessage
            {
                Method = "method/voidReturn"
            });
        }
    }
}
