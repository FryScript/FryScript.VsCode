using Newtonsoft.Json.Linq;

namespace LanguageServer2
{
    public interface IProtocol
    {
        void Execute(IClient connection, JToken content);
    }
}
