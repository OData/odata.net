//---------------------------------------------------------------------
// <copyright file="AstoriaRequestSender.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria.Util;

namespace System.Data.Test.Astoria
{
    public abstract class AstoriaRequestSender
    {
        private static AstoriaRequestSender _sender;
        public static AstoriaRequestSender GetSenderByTestProperties()
        {
            if (_sender == null)
            {
                switch (AstoriaTestProperties.Client)
                {
                    case ClientEnum.XMLHTTP:
                        _sender = new XmlHttpRequestSender();
                        break;
                    case ClientEnum.HTTP:
                    default:
                        _sender = new HttpRequestSender();
                        break;
                }
            }
            return _sender;
        }

        protected abstract bool SendRequest_Internal(AstoriaRequest request, out AstoriaResponse response);

        public AstoriaResponse SendRequest(AstoriaRequest request)
        {
            AstoriaResponse response;
            SendRequest(request, out response);
            return response;
        }

        public void SendRequest(AstoriaRequest request, out AstoriaResponse response)
        {
            // doing it this way so the compiler knows that response got initialized
            if (!SendRequest_Internal(request, out response))
            {
                bool valid = false;
                for (int retry = 0; retry < 4; retry++)
                {
                    valid = SendRequest_Internal(request, out response);
                    if (valid)
                        break;
                }

                if (!valid)
                    throw new Microsoft.Test.ModuleCore.TestFailedException("Could not get a valid response despite retrying");
            }
        }
    }

    public abstract class AstoriaRequestSender<TRequest, TResponse> : AstoriaRequestSender
    {
        protected abstract TRequest CreateRequest(AstoriaRequest request);
        protected abstract AstoriaResponse CreateResponse(AstoriaRequest request, TResponse underlyingResponse);
        protected abstract bool GetResponse(TRequest underlyingRequest, out TResponse underlyingResponse);
        
        protected override sealed bool SendRequest_Internal(AstoriaRequest request, out AstoriaResponse response)
        {
            TRequest underlyingRequest = CreateRequest(request);

            TResponse underlyingResponse;
            if (!GetResponse(underlyingRequest, out underlyingResponse))
            {
                response = null;
                return false;
            }

            // This should not be possible
            if (underlyingResponse == null)
                AstoriaTestLog.FailAndThrow("Somehow got a null underlying response");

            response = CreateResponse(request, underlyingResponse);
            return true;
        }
    }
}
