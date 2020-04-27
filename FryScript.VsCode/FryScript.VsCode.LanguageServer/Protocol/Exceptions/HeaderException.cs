using System;

namespace FryScript.VsCode.LanguageServer.Protocol.Exceptions
{
    public class HeaderException : Exception
    {
        public HeaderException(string message) : base(message) { }
    }
}
