using FryScript.VsCode.LanguageServer.Protocol.Schema;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IRequestReader
    {
        Task<RequestMessage> Read();
    }
}
