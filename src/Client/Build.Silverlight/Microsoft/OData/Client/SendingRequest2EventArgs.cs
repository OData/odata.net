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

    /// <summary> Event args for the SendingRequest2 event. </summary>
    public class SendingRequest2EventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of SendingRequest2EventsArgs
        /// </summary>
        /// <param name="requestMessage">request message.</param>
        /// <param name="descriptor">Descriptor that represents this change.</param>
        /// <param name="isBatchPart">True if this args represents a request within a batch, otherwise false.</param>
        internal SendingRequest2EventArgs(IODataRequestMessage requestMessage, Descriptor descriptor, bool isBatchPart)
        {
            this.RequestMessage = requestMessage;
            this.Descriptor = descriptor;
            this.IsBatchPart = isBatchPart;
        }

        /// <summary>The web request reported through this event. The handler may modify or replace it.</summary>
        public IODataRequestMessage RequestMessage
        {
            get;
            private set;
        }

        /// <summary>The request header collection.</summary>
        public Descriptor Descriptor
        {
            get;
            private set;
        }

        /// <summary> Returns true if this event is fired for request within a batch, otherwise returns false. </summary>
        public bool IsBatchPart
        {
            get;
            private set;
        }
    }
}
