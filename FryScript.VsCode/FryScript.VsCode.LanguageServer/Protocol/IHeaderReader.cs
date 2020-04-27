using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IHeaderReader
    {
        Task<int> Read();
    }
}
