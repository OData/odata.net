//---------------------------------------------------------------------
// <copyright file="SendingRequestEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Event args for the event fired before executing a web request. Gives a
    /// chance to customize or replace the request object to be used.
    /// </summary>
    public class SendingRequestEventArgs : EventArgs
    {
        /// <summary>The web request reported through this event</summary>
        private System.Net.WebRequest request;

        /// <summary>The request header collection.</summary>
        private System.Net.WebHeaderCollection requestHeaders;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request">The request reported through this event</param>
        /// <param name="requestHeaders">The request header collection.</param>
        internal SendingRequestEventArgs(System.Net.WebRequest request, System.Net.WebHeaderCollection requestHeaders)
        {
            // In Silverlight the request object is not accessible
            Debug.Assert(request != null, "null request");
            Debug.Assert(requestHeaders != null, "null requestHeaders");
            this.request = request;
            this.requestHeaders = requestHeaders;
        }

        /// <summary>Gets or sets the <see cref="System.Net.HttpWebRequest" /> instance about to be sent by the client library to the data service.</summary>
        /// <returns><see cref="System.Net.HttpWebRequest" />.</returns>
        public System.Net.WebRequest Request
        {
            get
            {
                return this.request;
            }

            set
            {
                Util.CheckArgumentNull(value, "value");
                if (!(value is System.Net.HttpWebRequest))
                {
                    throw Error.Argument(Strings.Context_SendingRequestEventArgsNotHttp, "value");
                }

                this.request = value;
                this.requestHeaders = value.Headers;
            }
        }

        /// <summary>Gets the collection protocol headers that are associated with the request to the data service.</summary>
        /// <returns>A collection of protocol headers that are associated with the request.</returns>
        public System.Net.WebHeaderCollection RequestHeaders
        {
            get { return this.requestHeaders; }
        }
    }
}
