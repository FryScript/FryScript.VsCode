using System;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class ProtocolMethodAttribute : Attribute
    {
        public string Method { get; set; }

        public ProtocolMethodAttribute(string method) => (Method) = method;
    }
}
