//   OData .NET Libraries ver. 6.9
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
