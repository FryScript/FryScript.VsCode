using FryScript.VsCode.LanguageServer.Protocol;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public interface IRequestReader
    {
        Task<RequestMessage> ReadAsync();
    }
}
