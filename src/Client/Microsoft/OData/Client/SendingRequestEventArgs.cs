//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
