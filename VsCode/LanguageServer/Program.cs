using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LanguageServer2
{

    public class LanguageServer
    {
        private readonly RequestHandler _requestHandler;
        private readonly ResponseHandler _responseHandler;
        private readonly ProtocolMethods _protocolMethods;

        public LanguageServer(RequestHandler requestHandler, ResponseHandler responseHandler, ProtocolMethods protocolMethods)
        {
            _requestHandler = requestHandler;
            _responseHandler = responseHandler;
            _protocolMethods = protocolMethods;
        }

        public LanguageServer()
            : this(new RequestHandler(), new ResponseHandler(), new ProtocolMethods())
        {
        }

        public LanguageServer(ProtocolMethods protocolMethods)
            : this(new RequestHandler(), new ResponseHandler(), protocolMethods)
        {
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    _responseHandler.Handle();
                }
            });

            while (true)
            {
                _requestHandler.Handle(_responseHandler, _protocolMethods);
            }
        }
    }


    class Program
    {
        
        static void Main(string[] args)
        {
            //while (!Debugger.IsAttached)
            //{

            //}

            //Debugger.Break();

            var languageServer = new LanguageServer();

            while (true)
            {
                languageServer.Start();
            }
        }
    }
}
