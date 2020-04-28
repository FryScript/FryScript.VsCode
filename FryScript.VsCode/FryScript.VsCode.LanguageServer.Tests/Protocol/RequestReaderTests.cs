using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FryScript.VsCode.LanguageServer.Tests.Protocol
{
    [TestClass]
    public class RequestReaderTests
    {
        private RequestReader _requestReader;
        private IHeaderReader _headerReader;
        private IContentReader _contentReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _headerReader = Substitute.For<IHeaderReader>();
            _contentReader = Substitute.For<IContentReader>();
            _requestReader = new RequestReader(_headerReader, _contentReader);
        }   
    }
}