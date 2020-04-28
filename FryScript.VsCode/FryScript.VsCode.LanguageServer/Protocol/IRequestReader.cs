using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Schema;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IRequestReader
    {
        Task<RequestMessage> Read();
    }
}
