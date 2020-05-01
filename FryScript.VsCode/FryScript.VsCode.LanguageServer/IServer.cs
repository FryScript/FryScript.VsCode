using System.Threading;

namespace FryScript.VsCode.LanguageServer
{
    public interface IServer
    {
        CancellationToken Start();
    }
}
