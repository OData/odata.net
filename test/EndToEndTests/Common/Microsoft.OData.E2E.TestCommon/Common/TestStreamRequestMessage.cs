﻿//-----------------------------------------------------------------------------
// <copyright file="TestStreamRequestMessage.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common
{
    /// <summary>
    /// Represents an OData request message that encapsulates an underlying <see cref="Stream"/>.
    /// </summary>
    public class TestStreamRequestMessage : IODataRequestMessageAsync
    {
        private readonly Stream stream;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        private readonly Uri uri;
        private readonly string method;

        public TestStreamRequestMessage(Stream stream, Uri uri, string method)
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
            this.headers[headerName] = headerValue;
        }

        public Stream GetStream()
        {
            return this.stream;
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(this.stream);
        }
    }
}
