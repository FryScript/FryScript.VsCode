using System.IO;
using System.Threading.Tasks;
using FryScript.VsCode.LanguageServer.Protocol.Schema;

namespace  FryScript.VsCode.LanguageServer.Protocol
{
    public class ResponseWriter : IResponseWriter
    {
        private readonly TextWriter _textWriter;

        public ResponseWriter(TextWriter textWriter) => (_textWriter) = (textWriter);

        public Task Write(ResponseMessage responseMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}