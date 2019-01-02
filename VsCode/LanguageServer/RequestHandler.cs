using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageServer2
{
    public class RequestHandler
    {
        private readonly RequestReader _reader;

        public RequestHandler(RequestReader reader)
        {
            _reader = reader;
        }

        public RequestHandler()
            : this(new RequestReader())
        {
        }

        public void Handle(ResponseHandler responseHandler, ProtocolMethods protocolMethods)
        {
            var json = _reader.Read();

            var obj = JsonConvert.DeserializeObject<JObject>(json);

            var id = obj.Value<int>("id");
            var methodName = obj.Value<string>("method");

            var protocol = protocolMethods.GetMethod(methodName);

            var connection = new RawClient(id, responseHandler);

            protocol.Execute(connection, obj["params"]);
        }
    }
}
