using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{
    public interface IResponseWriter
    {
        Task WriteAsync(object? response);
    }
}
