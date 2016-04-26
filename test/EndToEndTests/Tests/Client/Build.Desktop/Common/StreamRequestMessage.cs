//---------------------------------------------------------------------
// <copyright file="StreamRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;

    /// An implementation of IODataRequestMessage.
    /// In ODataLibrary, a message is an abstraction which consists of stream and header interfaces that hides the details of stream-reading/writing.
    public class StreamRequestMessage : IODataRequestMessage
    {
        private readonly Stream stream;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        private readonly Uri uri;
        private readonly string method;

        public StreamRequestMessage(Stream stream, Uri uri, string method)
        {
            this.stream = stream;
            this.uri = uri;
            this.method = method;
        }

        public Uri Url
        {
            get
            {
                return this.uri;
            }

            set
            {
                throw new InvalidOperationException("Request Uri cannot be changed");
            }
        }

        public string Method
        {
            get
            {
                return this.method;
            }

            set
            {
                throw new InvalidOperationException("Method cannot be changed");
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers;
            }
        }

        public string GetHeader(string headerName)
        {
            string value;
            return this.headers.TryGetValue(headerName, out value) ? value : null;
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.headers.Add(headerName, headerValue);
        }

        public Stream GetStream()
        {
            return this.stream;
        }
    }
}

