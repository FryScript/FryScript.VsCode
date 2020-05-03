using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Constants;
using Newtonsoft.Json;

namespace FryScript.VsCode.LanguageServer
{
    public class ResponseWriter : IResponseWriter
    {
        private readonly TextWriter _textWriter;

        public ResponseWriter(TextWriter textWriter) => _textWriter = textWriter;

        public async Task Write(object? response)
        {
            if (response == null)
                return;

            var content = JsonConvert.SerializeObject(response, JsonOptions.CamelCase);

            await _textWriter.WriteLineAsync($"Content-Length: {content.Length}");
            await _textWriter.WriteLineAsync();
            await _textWriter.WriteAsync(content);
        }
    }
}