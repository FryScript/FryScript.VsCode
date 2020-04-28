using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IContentReader
    {
        Task<string> Read(int contentLength);
    }
}