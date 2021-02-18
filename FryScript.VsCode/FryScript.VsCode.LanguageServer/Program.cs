using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var server = Server.Build(new LanguageServerMethods());

            await server.StartAsync();
        }
    }
}
