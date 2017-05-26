//---------------------------------------------------------------------
// <copyright file="DataServiceHostRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class DataServiceHostRequestMessage : IODataRequestMessage
    {
        private readonly TestServiceHost2 host;

        public DataServiceHostRequestMessage(TestServiceHost2 host)
        {
            this.host = host;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                foreach (var headerName in this.host.RequestHeaders.AllKeys)
                {
                    yield return new KeyValuePair<string, string>(headerName, this.host.RequestHeaders[headerName]);
                }
            }
        }

        public Uri Url
        {
            get
            {
                return this.host.AbsoluteRequestUri;
            }
            set
            {
                Assert.IsTrue(value.IsAbsoluteUri, "request url must always be absolute");
                this.host.AbsoluteRequestUri = value;
            }
        }

        public string Method
        {
            get
            {
                return this.host.RequestHttpMethod;
            }
            set
            {
                this.host.RequestHttpMethod = value;
            }
        }

        public string GetHeader(string headerName)
        {
            return this.host.RequestHeaders[headerName];
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.host.RequestHeaders[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.host.RequestStream;
        }
    }
}
