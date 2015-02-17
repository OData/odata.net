//---------------------------------------------------------------------
// <copyright file="HttpResponseData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Data structure for representing a single HTTP response
    /// </summary>
    public class HttpResponseData : MimePartData<byte[]>, IHttpMessage
    {
        /// <summary>
        /// Gets or sets the status code of the response
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Returns the first line of the message
        /// </summary>
        /// <returns>
        /// The first line of the message
        /// </returns>
        public virtual string GetFirstLine()
        {
            var builder = new StringBuilder();
            builder.Append("HTTP/1.1");
            builder.Append(" ");
            builder.Append((int)this.StatusCode);
            builder.Append(" ");
            builder.Append(this.StatusCode);
            return builder.ToString();
        }
    }
}
