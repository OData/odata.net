//---------------------------------------------------------------------
// <copyright file="SendingRequest2EventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

        /// <summary>The web request reported through this event. The handler may modify it.</summary>
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
