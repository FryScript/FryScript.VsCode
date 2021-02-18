using System;

namespace FryScript.VsCode.LanguageServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProtocolMethodAttribute : Attribute
    {
        public string Method { get; set; }

        public ProtocolMethodAttribute(string method) => Method = method;
    }
}
