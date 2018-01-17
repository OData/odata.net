//---------------------------------------------------------------------
// <copyright file="ODataRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.OData;

    class ODataRequestMessage : IODataRequestMessage, IContainerProvider
    {
        private Stream body;
        private Dictionary<string, string> headers;

        /// <summary>
        /// Constructor for the IncomingRequestMessage class.
        /// </summary>
        /// <param name="body">The message body.</param>
        /// <param name="headers">The web headers.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="method">The Http method.</param>
        public ODataRequestMessage(Stream body, Dictionary<string, string> headers, Uri uri, string method)
        {
            this.body = body;
            this.Url = uri;
            this.Method = method;

            this.headers = headers;
        }

        /// <summary>
        /// Retrieves the value of the specified web header.
        /// </summary>
        /// <param name="headerName">The name of the header.</param>
        /// <returns>The value of the header, or empty string if missing.</returns>
        public string GetHeader(string headerName)
        {
            if( this.headers.ContainsKey(headerName))
            {
                return this.headers[headerName];
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the value of the specified web header.
        /// </summary>
        /// <param name="headerName">The name of the header.</param>
        /// <param name="headerValue">The new value of the header.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.headers[headerName] = headerValue;
        }

        /// <summary>
        /// Retrieves the message body.
        /// </summary>
        /// <returns>The message body.</returns>
        public Stream GetStream()
        {
            return this.body;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        public Uri Url { get; set; }

        public string Method { get; set; }

        public IServiceProvider Container { get; set; }
    }
}