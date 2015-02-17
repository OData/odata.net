//---------------------------------------------------------------------
// <copyright file="HttpRequestData`2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Data structure for representing a single HTTP request with generic uri and body types
    /// </summary>
    /// <typeparam name="TUri">The type of the request uri</typeparam>
    /// <typeparam name="TBody">The type of the request body</typeparam>
    public abstract class HttpRequestData<TUri, TBody> : MimePartData<TBody>, IHttpRequest
    {
        /// <summary>
        /// Gets or sets the http verb/method for the request
        /// </summary>
        public HttpVerb Verb { get; set; }

        /// <summary>
        /// Gets or sets the uri for the request
        /// </summary>
        public TUri Uri { get; set; }

        /// <summary>
        /// Returns the request's URI
        /// </summary>
        /// <returns>The URI of the request</returns>
        public abstract Uri GetRequestUri();

        /// <summary>
        /// Returns the binary body of the request
        /// </summary>
        /// <returns>The binary body of the request</returns>
        public abstract byte[] GetRequestBody();

        /// <summary>
        /// Returns the first line of the message
        /// </summary>
        /// <returns>
        /// The first line of the message
        /// </returns>
        public virtual string GetFirstLine()
        {
            var builder = new StringBuilder();
            builder.Append(this.Verb.ToHttpMethod());
            builder.Append(" ");
            builder.Append(this.GetRequestUri().OriginalString);
            builder.Append(" HTTP/1.1");
            return builder.ToString();
        }
    }
}
