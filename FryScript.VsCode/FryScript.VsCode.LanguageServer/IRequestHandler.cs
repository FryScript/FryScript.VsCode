using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public interface IRequestHandler
    {
        Task HandleAsync();
    }
}
