using System;
using System.Collections.Generic;
using System.Text;

namespace FryScript.VsCode.LanguageServer.Protocol.Schema
{
    public class ResponseMessage : Message
    {
        public int Id { get; set; }

        public object? Result { get; set; }
    }
}
