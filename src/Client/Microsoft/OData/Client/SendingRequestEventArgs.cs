//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
#if ASTORIA_LIGHT
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not used in Silverlight")]
#endif
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
            // In Silverlight the request object is not accesible
#if ASTORIA_LIGHT
            Debug.Assert(null == request, "non-null request in SL.");
#else
            Debug.Assert(null != request, "null request");
#endif
            Debug.Assert(null != requestHeaders, "null requestHeaders");
            this.request = request;
            this.requestHeaders = requestHeaders;
        }

#if !ASTORIA_LIGHT // Data.Services http stack
        /// <summary>Gets or sets the <see cref="T:System.Net.HttpWebRequest" /> instance about to be sent by the client library to the data service.</summary>
        /// <returns><see cref="T:System.Net.HttpWebRequest" />.</returns>
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
#endif

        /// <summary>Gets the collection protocol headers that are associated with the request to the data service.</summary>
        /// <returns>A collection of protocol headers that are associated with the request.</returns>
        public System.Net.WebHeaderCollection RequestHeaders
        {
            get { return this.requestHeaders; }
        }
    }
}
