using System;

namespace FryScript.VsCode.LanguageServer
{
    public class ProtocolMethodAttribute : Attribute
    {
        public string Method { get; set; }

        public ProtocolMethodAttribute(string method) => Method = method;
    }
}
