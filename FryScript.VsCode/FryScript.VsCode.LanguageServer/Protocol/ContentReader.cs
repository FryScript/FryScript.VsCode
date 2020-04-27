using System;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ContentReader : IRequestReader
    {
        public Task<string> Read()
        {
            throw new NotImplementedException();
        }
    }
}
