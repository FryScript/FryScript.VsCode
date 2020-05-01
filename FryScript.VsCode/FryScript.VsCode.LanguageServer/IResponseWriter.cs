using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public interface IResponseWriter
    {
        Task Write(object? response);
    }
}
