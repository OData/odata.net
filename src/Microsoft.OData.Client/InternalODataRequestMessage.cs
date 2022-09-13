//---------------------------------------------------------------------
// <copyright file="InternalODataRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// This is a just a pass through implementation of IODataRequestMessage. This class is used
    /// for wrapping the inner batch requests or in silverlight when we are using the
    /// non-silverlight http stack, we need to fire IODataRequestMessage which throws
    /// when GetStream is called.
    /// </summary>
    internal class InternalODataRequestMessage : DataServiceClientRequestMessage
    {
        /// <summary>
        /// IODataRequestMessage implementation that this class wraps.
        /// </summary>
        private readonly IODataRequestMessage requestMessage;

        /// <summary>
        /// Boolean flag to allow calls to GetStream() on this instance
        /// We want to allow this because WritingRequest and ReadingResponse events on the Windows Phone platform
        /// requires that we pass a readable stream to user code as arguments.
        /// </summary>
        private readonly bool allowGetStream;

        /// <summary>
        /// request stream
        /// </summary>
        private Stream cachedRequestStream;

        /// <summary>
        /// dictionary containing http headers.
        /// </summary>
        private HeaderCollection headers;

        /// <summary>
        /// Creates a new instance of InternalODataRequestMessage.
        /// </summary>
        /// <param name="requestMessage">IODataRequestMessage that needs to be wrapped.</param>
        /// <param name="allowGetStream">boolean flag to allow calls to GetStream() on this instance</param>
        internal InternalODataRequestMessage(IODataRequestMessage requestMessage, bool allowGetStream) : base(requestMessage.Method)
        {
            this.requestMessage = requestMessage;
            this.allowGetStream = allowGetStream;
        }

        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.HeaderCollection.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets or Sets the request url.
        /// </summary>
        public override Uri Url
        {
            get
            {
                return this.requestMessage.Url;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or Sets the http method for this request.
        /// </summary>
        public override string Method
        {
            get
            {
                return this.requestMessage.Method;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or set the credentials for this request.
        /// </summary>
        public override ICredentials Credentials
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
        public override int Timeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the read and write timeout (in seconds) for this request.
        /// </summary>
        public override int ReadWriteTimeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments to the
        ///  Internet resource.
        /// </summary>
        public override bool SendChunked
        {
            get
            {
                throw new NotImplementedException();
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
                    this.headers = new HeaderCollection(this.requestMessage.Headers);
                }

                return this.headers;
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public override string GetHeader(string headerName)
        {
            return this.HeaderCollection.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            this.requestMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Gets the stream to be used to write the request payload.
        /// </summary>
        /// <returns>Stream to which the request payload needs to be written.</returns>
        public override Stream GetStream()
        {
            if (!this.allowGetStream)
            {
                throw new NotImplementedException();
            }

            if (this.cachedRequestStream == null)
            {
                this.cachedRequestStream = this.requestMessage.GetStream();
            }

            return this.cachedRequestStream;
        }

        /// <summary>
        /// Abort the current request.
        /// </summary>
        public override void Abort()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ends an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request for a response.</returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage GetResponse()
        {
            throw new NotImplementedException();
        }
    }
}
