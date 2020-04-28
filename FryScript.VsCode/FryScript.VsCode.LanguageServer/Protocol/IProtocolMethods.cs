﻿using FryScript.VsCode.LanguageServer.Protocol.Schema;
using System.Threading.Tasks;

namespace FryScript.VsCode.LanguageServer.Protocol
{
    public interface IProtocolMethods
    {
        Task<ResponseMessage> Execute(RequestMessage requestMessage);
    }
}
