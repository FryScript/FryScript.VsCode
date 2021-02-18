using System.Threading;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public interface IServer
    {
        Task StartAsync();
    }
}
