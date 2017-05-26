//---------------------------------------------------------------------
// <copyright file="ODataRequestMessageSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;

    public class ODataRequestMessageSimulator : IODataRequestMessage
    {
        private readonly IDictionary<string, string> headers = new Dictionary<string, string>();

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers.AsEnumerable(); }
        }

        public virtual System.Uri Url
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public virtual string Method
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        internal Stream Stream { get; set; }

        public string GetHeader(string headerName)
        {
            string headerValue;
            if (!this.headers.TryGetValue(headerName, out headerValue))
            {
                return null;
            }

            return headerValue;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.headers[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.Stream;
        }
    }
}