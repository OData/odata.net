//---------------------------------------------------------------------
// <copyright file="BuildingRequestEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// EventArgs for the BuildingRequest event.
    /// </summary>
    public class BuildingRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Uri of the outgoing request.
        /// </summary>
        private Uri requestUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingRequestEventArgs"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="headers">The request headers.</param>
        /// <param name="descriptor">Descriptor for this request; or null if there isn't one.</param>
        /// <param name="httpStack">The http stack.</param>
        internal BuildingRequestEventArgs(string method, Uri requestUri, HeaderCollection headers, Descriptor descriptor, HttpStack httpStack)
        {
            this.Method = method;
            this.RequestUri = requestUri;
            this.HeaderCollection = headers ?? new HeaderCollection();
            this.ClientHttpStack = httpStack;
            this.Descriptor = descriptor;
        }

        /// <summary>
        /// Gets the Request HTTP Method that the outgoing request will use.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The Uri of the outgoing request. The Uri may be altered. No error checking will be performed against any changes made.
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                return this.requestUri;
            }

            set
            {
                this.requestUri = value;
            }
        }

        /// <summary>
        /// The headers for this request. Adding new custom headers is supported. Behavior is undefined for changing existing headers or adding
        /// system headers.  No error checking will be performed against any changes made.
        /// </summary>
        public IDictionary<string, string> Headers
        {
            get
            {
                return this.HeaderCollection.UnderlyingDictionary;
            }
        }

        /// <summary>
        /// Descriptor for this request if there is one; null otherwise.
        /// </summary>
        public Descriptor Descriptor { get; private set; }

        /// <summary>
        /// Gets the http stack.
        /// </summary>
        /// <remarks>
        /// The reason for having this property is that this is request specific
        /// and cannot be taken from the context. For e.g. In silverlight, irrespective
        /// of the value of HttpStack property, for stream requests (get or update), we
        /// use ClientHttp.
        /// </remarks>
        internal HttpStack ClientHttpStack { get; private set; }

        /// <summary>
        /// Returns the set of headers as HeaderCollection instance.
        /// </summary>
        internal HeaderCollection HeaderCollection { get; private set; }

        /// <summary>
        /// Retrieves a new RequestMessageArgs with any custom query parameters added.
        /// </summary>
        /// <returns>A new RequestMessageArgs instance that takes new custom query options into account.</returns>
        internal BuildingRequestEventArgs Clone()
        {
            return new BuildingRequestEventArgs(this.Method, this.RequestUri, this.HeaderCollection, this.Descriptor, this.ClientHttpStack);
        }
    }
}
