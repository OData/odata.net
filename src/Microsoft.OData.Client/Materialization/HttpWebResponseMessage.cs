//---------------------------------------------------------------------
// <copyright file="HttpWebResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.OData;

    /// <summary>
    /// IODataResponseMessage interface implementation
    /// </summary>
    public class HttpWebResponseMessage : IODataResponseMessage, IDisposable
    {
        /// <summary>Cached headers.</summary>
        private readonly HeaderCollection headers;

        /// <summary>A delegate that returns the response stream.</summary>
        private readonly Func<Stream> getResponseStream;

        /// <summary>A delegate that returns the response stream asynchronously.</summary>
        private readonly Func<CancellationToken, Task<Stream>> getResponseStreamAsync;

        /// <summary>The response status code.</summary>
        private readonly int statusCode;

        /// <summary>HttpWebResponse instance.</summary>
        private HttpWebResponse httpWebResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebResponseMessage"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="getResponseStream">
        /// Optional delegate that returns the response stream synchronously.
        /// At least one of <paramref name="getResponseStream"/> or <paramref name="getResponseStreamAsync"/> must be provided.
        /// </param>
        /// <param name="getResponseStreamAsync">
        /// Optional delegate that returns the response stream asynchronously.
        /// At least one of <paramref name="getResponseStream"/> or <paramref name="getResponseStreamAsync"/> must be provided.
        /// </param>
        /// <remarks>
        /// Provide either or both stream access delegates depending on whether synchronous,
        /// asynchronous, or both types of stream access are required.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="headers"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when both <paramref name="getResponseStream"/> and <paramref name="getResponseStreamAsync"/> are null.
        /// </exception>
        public HttpWebResponseMessage(
            IDictionary<string, string> headers,
            int statusCode,
            Func<Stream> getResponseStream = null,
            Func<CancellationToken, Task<Stream>> getResponseStreamAsync = null)
            : this(
                  headers != null ? new HeaderCollection(headers) : throw Error.ArgumentNull(nameof(headers)),
                  statusCode,
                  getResponseStream,
                  getResponseStreamAsync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebResponseMessage"/> class.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="getResponseStream">
        /// Optional delegate that returns the response stream synchronously.
        /// At least one of <paramref name="getResponseStream"/> or <paramref name="getResponseStreamAsync"/> must be provided.
        /// </param>
        /// <param name="getResponseStreamAsync">
        /// Optional delegate that returns the response stream asynchronously.
        /// At least one of <paramref name="getResponseStream"/> or <paramref name="getResponseStreamAsync"/> must be provided.
        /// </param>
        /// <remarks>
        /// Provide either or both stream access delegates depending on whether synchronous,
        /// asynchronous, or both types of stream access are required.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="headers"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when both <paramref name="getResponseStream"/> and <paramref name="getResponseStreamAsync"/> are null.
        /// </exception>
        internal HttpWebResponseMessage(
            HeaderCollection headers,
            int statusCode,
            Func<Stream> getResponseStream = null,
            Func<CancellationToken, Task<Stream>> getResponseStreamAsync = null)
        {
            if (headers == null)
            {
                throw Error.ArgumentNull(nameof(headers));
            }

            if (getResponseStream == null && getResponseStreamAsync == null)
            {
                throw new ArgumentException(
                    $"At least one of the parameters '{nameof(getResponseStream)}'or '{nameof(getResponseStreamAsync)}' must be provided.");
            }

            this.headers = headers;
            this.statusCode = statusCode;
            this.getResponseStream = getResponseStream;
            this.getResponseStreamAsync = getResponseStreamAsync;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="getResponseStream">A function returning the response stream.</param>
        [Obsolete("Use the constructor overload that accepts Func<CancellationToken, Task<Stream>> " +
            "for proper async stream retrieval and cancellation support. " +
            "This overload will be removed in a future version.", error: false)]
        public HttpWebResponseMessage(IDictionary<string, string> headers, int statusCode, Func<Stream> getResponseStream)
            : this(
                  headers != null ? new HeaderCollection(headers) : throw Error.ArgumentNull(nameof(headers)),
                  statusCode,
                  getResponseStream)
        {
        }

        /// <summary>
        /// Returns the collection of response headers.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers.AsEnumerable();
            }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public virtual int StatusCode
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public virtual string GetHeader(string headerName)
        {
            Util.CheckArgumentNullAndEmpty(headerName, "headerName");
            string result;
            if (this.headers.TryGetHeader(headerName, out result))
            {
                return result;
            }

            // Since the uninitialized value of ContentLength header is -1, we need to return
            // -1 if the content length header is not present
            if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.Ordinal))
            {
                return "-1";
            }

            return null;
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public virtual void SetHeader(string headerName, string headerValue)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the stream to be used to read the response payload.
        /// </summary>
        /// <remarks>
        /// If the synchronous delegate was not provided during construction,
        /// this method will invoke the async delegate synchronously, which may block.
        /// </remarks>
        /// <returns>Stream from which the response payload can be read.</returns>
        public virtual Stream GetStream()
            => getResponseStream != null
            ? getResponseStream()
            : getResponseStreamAsync!(default).GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously gets the stream to be used to read the response payload.
        /// </summary>
        /// <remarks>
        /// If the asynchronous delegate was not provided during construction,
        /// this method will wrap the synchronous delegate result in a completed task.
        /// </remarks>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the stream from which the response payload can be read.
        /// </returns>
        public virtual Task<Stream> GetStreamAsync()
            => getResponseStreamAsync != null
            ? getResponseStreamAsync(default)
            : Task.FromResult(getResponseStream!());

        /// <summary>
        /// Close the underlying HttpWebResponse.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected virtual void Dispose(bool disposing)
        {
            HttpWebResponse response = this.httpWebResponse;
            this.httpWebResponse = null;
            if (response != null)
            {
                ((IDisposable)response).Dispose();
            }
        }
    }
}
