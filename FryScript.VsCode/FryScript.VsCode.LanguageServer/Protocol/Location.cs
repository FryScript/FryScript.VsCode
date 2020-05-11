using System;
using FryScript.VsCode.LanguageServer.Protocol.Constants;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public class Location
    {
        public Uri Uri { get; set; } = Uris.Empty;

        public Range Range { get; set; } = new Range();
    }
}