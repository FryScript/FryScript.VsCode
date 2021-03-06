﻿using FryScript.VsCode.LanguageServer.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Tests
{
    [TestClass]
    public class RequestReaderTests
    {
        private RequestReader _requestReader;
        private TextReader _textReader;

        [TestInitialize]
        public void Initialize()
        {
            _textReader = Substitute.For<TextReader>();

            _requestReader = new RequestReader(_textReader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ReadAsync_Header_Null_Content_Length_Header_Exception()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult<string>(null));

            await _requestReader.ReadAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ReadAsync_Header_Malformed_Content_Length_Exception()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult("Content-Length 100"));

            await _requestReader.ReadAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ReadAsync_Header_Non_Int_Content_Length_Exception()
        {
            _textReader
                .ReadLineAsync()
                .Returns(Task.FromResult("Content-Length: error"));

            await _requestReader.ReadAsync();
        }

        [TestMethod]
        public async Task ReadAsync_Header_Success()
        {
            var expectedRequest = new RequestMessage
            {
                Id = 100,
                Method = "test",
                Params = new JObject()
            };

            var requestJson = JsonConvert.SerializeObject(expectedRequest);

            _textReader
                .ReadLineAsync()
                .Returns(
                    x => Task.FromResult($"Content-Length: {requestJson.Length}\r\n"),
                    x => Task.FromResult(string.Empty));

            _textReader
                .ReadAsync(Arg.Do<char[]>(c =>
                {
                    var chars = requestJson.ToCharArray();
                    Array.Copy(chars, 0, c, 0, chars.Length);
                }), 0, requestJson.Length)
                .Returns(Task.FromResult(requestJson.Length));

            var result = await _requestReader.ReadAsync();

            Assert.AreEqual(expectedRequest.Id, result.Id);
            Assert.AreEqual(expectedRequest.Method, result.Method);
            Assert.IsInstanceOfType(result.Params, typeof(JObject));
        }
    }
}
