using System;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol;

namespace FryScript.VsCode.LanguageServer
{
    public interface IProtocolMethods
    {
        Task<ResponseMessage> ExecuteAsync(RequestMessage requestMessage);

        event Action<object> OnClientRequest;
    }
}
