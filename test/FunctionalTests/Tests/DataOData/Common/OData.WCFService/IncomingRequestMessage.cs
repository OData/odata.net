//---------------------------------------------------------------------
// <copyright file="IncomingRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// A class representing a message that has been received by the service.
    /// </summary>
    public class IncomingRequestMessage : IODataRequestMessage
    {
        private Stream body;
        private List<KeyValuePair<string, string>> headers;

        /// <summary>
        /// Constructor for the IncomingRequestMessage class.
        /// </summary>
        /// <param name="body">The message body.</param>
        /// <param name="headers">The web headers.</param>
        /// <param name="uri">The request URI.</param>
        /// <param name="method">The Http method.</param>
        public IncomingRequestMessage(Stream body, WebHeaderCollection headers, Uri uri, string method)
        {
            this.body = body;
            this.Url = uri;
            this.Method = method;

            this.headers = new List<KeyValuePair<string, string>>();
            foreach(var key in headers.Keys)
            {
                this.headers.Add(new KeyValuePair<string, string>(key.ToString(), headers[key.ToString()]));
            }
        }

        /// <summary>
        /// Retrieves the value of the specified web header.
        /// </summary>
        /// <param name="headerName">The name of the header.</param>
        /// <returns>The value of the header, or empty string if missing.</returns>
        public string GetHeader(string headerName)
        {
            var header = this.headers.Where(h => h.Key == headerName);
            return header.Any() ? header.Single().Value : string.Empty;
        }

        /// <summary>
        /// Sets the value of the specified web header.
        /// </summary>
        /// <param name="headerName">The name of the header.</param>
        /// <param name="headerValue">The new value of the header.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.headers.RemoveAll(h => h.Key == headerValue);
            this.headers.Add(new KeyValuePair<string, string>(headerName, headerValue));
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

        public string Method { get; set;}
    }
}