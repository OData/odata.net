//---------------------------------------------------------------------
// <copyright file="HttpResponseBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a single HTTP response
    /// </summary>
    public static class HttpResponseBuilder
    {
        // use a 16kb buffer when copying from the response stream
        private const int BufferSize = 16 * 1024;

        /// <summary>
        /// Initializes a new instance of the HttpResponse class using the status code and headers given, and by reading from the given stream
        /// </summary>
        /// <param name="statusCode">The status code for the response</param>
        /// <param name="headers">The headers for the response</param>
        /// <param name="bodyStream">The underlying response body stream</param>
        /// <returns>The http response</returns>
        public static HttpResponseData BuildResponse(HttpStatusCode statusCode, IDictionary<string, string> headers, Stream bodyStream)
        {
            ExceptionUtilities.CheckArgumentNotNull(headers, "headers");
            ExceptionUtilities.CheckArgumentNotNull(bodyStream, "bodyStream");

            var response = new HttpResponseData();

            response.StatusCode = statusCode;
            foreach (var header in headers)
            {
                response.Headers.Add(header.Key, header.Value);
            }

            // get the content length from the headers, using -1 to indicate that it is not known
            int contentLength = -1;
            if (headers.ContainsKey(HttpHeaders.ContentLength))
            {
                // note that numbers greater than int.MaxValue will not be parsed, 
                // and so we will read as if we did not know the length
                if (!int.TryParse(headers[HttpHeaders.ContentLength], out contentLength))
                {
                    contentLength = -1;
                }
            }

            // Empty could also be null, but this is nicer to work with
            if (contentLength == 0)
            {
                response.Body = new byte[0];
            }
            else
            {
                // create an in-memory stream to store the body. if the length is known, pre-allocate the space
                MemoryStream tempStream;
                if (contentLength > 0)
                {
                    tempStream = new MemoryStream(contentLength);
                }
                else
                {
                    tempStream = new MemoryStream();
                }

                using (tempStream)
                {
                    // copy blocks into the in-memory stream until read() returns -1
                    int read;
                    byte[] buffer = new byte[BufferSize];
                    while ((read = bodyStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        tempStream.Write(buffer, 0, read);
                    }

                    // save the bytes in the in-memory stream
                    response.Body = tempStream.ToArray();
                }
            }

            return response;
        }
    }
}
