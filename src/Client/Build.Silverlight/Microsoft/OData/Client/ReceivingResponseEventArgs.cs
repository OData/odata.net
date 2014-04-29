//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Core;

    /// <summary>
    /// <see cref="EventArgs"/> class for the <see cref="DataServiceContext.ReceivingResponse"/> event.
    /// Exposes the ResponseMessage to the user.
    /// </summary>
    public class ReceivingResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingResponseEventArgs"/> class for a
        /// non-batch or top level $batch response.
        /// </summary>
        /// <param name="responseMessage">The response message the client is receiving.</param>
        /// <param name="descriptor">Descriptor for the request that the client is receiving the response for.</param>
        public ReceivingResponseEventArgs(IODataResponseMessage responseMessage, Descriptor descriptor)
            : this(responseMessage, descriptor, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingResponseEventArgs"/> class.
        /// </summary>
        /// <param name="responseMessage">The response message the client is receiving.</param>
        /// <param name="descriptor">Descriptor for the request that the client is receiving the response for.</param>
        /// <param name="isBatchPart">Indicates if this response is to an inner batch query or operation.</param>
        public ReceivingResponseEventArgs(IODataResponseMessage responseMessage, Descriptor descriptor, bool isBatchPart)
        {
            this.ResponseMessage = responseMessage;
            this.Descriptor = descriptor;
            this.IsBatchPart = isBatchPart;
        }

        /// <summary>
        /// Gets the response message that the client is receiving.
        /// </summary>
        public IODataResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// True if the response is an inner batch operation or query; false otherwise.
        /// </summary>
        public bool IsBatchPart { get; private set; }

        /// <summary>
        /// Descriptor for the request that the client is receiving the response for.
        /// The descriptor may be null for certain types of requests, like most GET requests
        /// and the top level $batch request. 
        /// </summary>
        public Descriptor Descriptor { get; private set; }
    }
}
