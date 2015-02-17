//---------------------------------------------------------------------
// <copyright file="AstoriaResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class AstoriaResponse : AstoriaRequestResponseBase
    {
        internal AstoriaResponse(AstoriaRequest originalRequest)
            : base(originalRequest.Workspace)
        {
            Request = originalRequest;
        }

        public AstoriaRequest Request
        {
            get;
            internal set;
        }

        public Exception Exception
        {
            get;
            internal set;
        }

        public HttpStatusCode ActualStatusCode
        {
            get;
            internal set;
        }

        public bool ETagHeaderFound
        {
            get;
            internal set;
        }

        public override string ETagHeader
        {
            get
            {
                if (!ETagHeaderFound)
                    return null;
                return base.ETagHeader;
            }
            set
            {
                base.ETagHeader = value;
            }
        }

        private bool _BytesSet = false;
        private byte[] _Bytes = null;
        public byte[] Bytes
        {
            get
            {
                if (!_BytesSet)
                {
                    if (_PayloadSet)
                        Bytes = new UTF8Encoding().GetBytes(_Payload);
                    else
                        AstoriaTestLog.FailAndThrow("Response payload is unset, cannot retrieve bytes");
                }
                return _Bytes;
            }
            set
            {
                _Bytes = value;
                _BytesSet = true;
            }
        }

        private bool _PayloadSet = false;
        private string _Payload = null;
        public override string Payload
        {
            get
            {
                if (!_PayloadSet)
                {
                    if (_BytesSet)
                        Payload = new UTF8Encoding().GetString(_Bytes);
                    else
                        AstoriaTestLog.FailAndThrow("Response Payload is unset, cannot retrieve string");
                }
                return _Payload;
            }
            set
            {
                _Payload = value;
                _PayloadSet = true;
            }
        }

        public void LogResponse(StringBuilder builder, bool logPayload, bool logHeaders)
        {
            builder.AppendLine((int)ActualStatusCode + " " + ActualStatusCode.ToString());
            
            if (logHeaders)
            {
                foreach (var pair in this.Headers)
                    builder.AppendLine(pair.Key + ": " + pair.Value);
            }

            if (logPayload)
            {
                builder.AppendLine(Payload);
            }
        }

        public void LogResponse()
        {
            StringBuilder builder = new StringBuilder();
            LogResponse(builder, true, false);
            AstoriaTestLog.WriteIgnore(builder.ToString());
        }
    }
}
