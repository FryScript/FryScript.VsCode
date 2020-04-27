using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IRequestReader
    {
        Task<string> Read();
    }
}
