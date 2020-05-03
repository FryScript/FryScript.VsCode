using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FryScript.VsCode.LanguageServer.Protocol.Constants
{
    public static class JsonOptions
    {
        public readonly static JsonSerializerSettings CamelCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
