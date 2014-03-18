//---------------------------------------------------------------------
// <copyright file="InternalODataResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
#if WINDOWS_PHONE
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.OData;

    /// <summary>
    /// This is a just a pass through implementation of IODataResponseMessage. This is required
    /// so that when we fire ReadingResponse event for inner batch requests or in silverlight when
    /// we are using the non-silverlight http stack, we need to fire IODataRequestMessage
    /// which throws when GetStream is called.
    /// </summary>
    internal class InternalODataResponseMessage : IODataResponseMessage
    {
        /// <summary>
        /// IODataResponseMessage implementation that this class wraps.
        /// </summary>
        private readonly IODataResponseMessage responseMessage;

        /// <summary>Buffer used for caching operation response body streams.</summary>
        private byte[] streamCopyBuffer;

        /// <summary>
        /// request stream 
        /// </summary>
        private Stream cachedResponseStream;

        /// <summary>
        /// Boolean flag to allow calls to GetStream() on this instance
        /// </summary>
        private bool allowGetStream;

        /// <summary>
        /// Request information to fire events.
        /// </summary>
        private RequestInfo requestInfo;

        /// <summary>
        /// dictionary containing http headers.
        /// </summary>
        private HeaderCollection headers;

        /// <summary>
        /// Creates a new instance of InternalODataRequestMessage.
        /// </summary>
        /// <param name="responseMessage">IODataResponseMessage that needs to be wrapped.</param>
        /// <param name="allowGetStream">boolean flag to allow calls to GetStream() on this instance</param>
        /// <param name="requestInfo"> Request information to fire events. </param>
        internal InternalODataResponseMessage(IODataResponseMessage responseMessage, bool allowGetStream, RequestInfo requestInfo)
        {
            this.responseMessage = responseMessage;
            this.requestInfo = requestInfo;
            this.allowGetStream = allowGetStream;
        }

        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.HeaderCollection.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets or Sets the StatusCode.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.responseMessage.StatusCode;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// internal headers dictionary
        /// </summary>
        private HeaderCollection HeaderCollection
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HeaderCollection(this.responseMessage);
                }

                return this.headers;
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public string GetHeader(string headerName)
        {
            return this.HeaderCollection.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.HeaderCollection.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Gets the stream to be used to write the request payload.
        /// </summary>
        /// <returns>Stream to which the request payload needs to be written.</returns>
        public Stream GetStream()
        {
            if (!this.allowGetStream)
            {
                throw new NotImplementedException();
            }

            if (this.cachedResponseStream == null)
            {
                this.cachedResponseStream = this.responseMessage.GetStream();
            }

            return this.cachedResponseStream;
        }

        /// <summary>
        /// Sets the stream for the request payload to write.
        /// </summary>
        /// <param name="requestStream">request payload stream.</param>
        public void SetStream(Stream requestStream)
        {
            this.cachedResponseStream = requestStream;
        }

        /// <summary>
        /// Fires the ReadingResponse event
        /// </summary>
        /// <param name="isBatchPart">Boolean flag indicating if this request is part of a batch request..</param>
        internal void FireReadingResponse(bool isBatchPart)
        {
            var readableStream = new MemoryStream();
            var requestStream = this.GetStream();
            try
            {
                WebUtil.CopyStream(requestStream, readableStream, ref this.streamCopyBuffer);
                readableStream.Position = 0;
            }
            finally
            {
                requestStream.Dispose();
            }

            ReadingWritingHttpMessageEventArgs args = new ReadingWritingHttpMessageEventArgs(this.HeaderCollection, readableStream, isBatchPart);
            var rewrittenArgs = this.requestInfo.FireReadingResponseEvent(args);
            this.cachedResponseStream = rewrittenArgs.Content;
            ApplyHeadersToResponse(rewrittenArgs.HeaderCollection, this);
        }

        /// <summary>
        /// Applies headers in the dictionary to a response message.
        /// </summary>
        /// <param name="headers">The dictionary with the headers to apply.</param>
        /// <param name="responseMessage">The request message to apply the headers to.</param>
        private static void ApplyHeadersToResponse(HeaderCollection headers, IODataResponseMessage responseMessage)
        {
            // NetCF bug with how the enumerators for dictionaries work.
            foreach (KeyValuePair<string, string> header in headers.AsEnumerable().ToList())
            {
                responseMessage.SetHeader(header.Key, header.Value);
            }
        }
    }
#endif
}