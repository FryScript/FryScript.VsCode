using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace LanguageServer2
{
    public class ResponseWriter
    {
        private readonly TextWriter _writer;

        public ResponseWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void Write(object response)
        {
            var sb = new StringBuilder();

            var json = JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            sb.Append($"Content-Length: {json.Length}\r\n");
            sb.Append("\r\n");
            sb.Append(json);

            _writer.Write(sb.ToString());
        }
    }
}
