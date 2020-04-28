using FryScript.VsCode.LanguageServer.Protocol.Schema;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IResponseWriter
    {
        Task Write(ResponseMessage responseMessage);
    }
}
