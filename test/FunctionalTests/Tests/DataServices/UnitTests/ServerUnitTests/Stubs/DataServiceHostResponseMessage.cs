//---------------------------------------------------------------------
// <copyright file="DataServiceHostResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.IO;
    using Microsoft.OData;

    public class DataServiceHostResponseMessage : IODataResponseMessage
    {
        private readonly IDataServiceHost2 host;
        public DataServiceHostResponseMessage(IDataServiceHost2 request)
        {
            this.host = request;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                foreach (var headerName in this.host.ResponseHeaders.AllKeys)
                {
                    yield return new KeyValuePair<string, string>(char.IsUpper(headerName[0]) ? headerName.ToLowerInvariant() : headerName.ToUpperInvariant(), this.host.ResponseHeaders[headerName]);
                }
            }
        }

        public int StatusCode
        {
            get
            {
                return this.host.ResponseStatusCode;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public string GetHeader(string headerName)
        {
            string headerValue = this.host.ResponseHeaders[headerName];
            if (string.IsNullOrEmpty(headerValue))
            {
                // Since the unintialized value of ContentLength header is -1, we need to return
                // -1 if the content length header is not present
                if (string.Equals(headerName, "Content-Length", StringComparison.OrdinalIgnoreCase))
                {
                    headerValue = "-1";
                }
            }
            return headerValue;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.host.ResponseHeaders[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.host.ResponseStream;
        }
    }
}
