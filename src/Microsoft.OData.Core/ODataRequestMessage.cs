//---------------------------------------------------------------------
// <copyright file="ODataRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Wrapper class around an IODataRequestMessageAsync to isolate our code from the interface implementation.
    /// </summary>
    internal sealed class ODataRequestMessage : ODataMessage,
#if PORTABLELIB
        IODataRequestMessageAsync
#else
        IODataRequestMessage
#endif
    {
        /// <summary>The request message this class is wrapping.</summary>
        private readonly IODataRequestMessage requestMessage;

        /// <summary>
        /// Constructs an internal wrapper around the <paramref name="requestMessage"/>
        /// that isolates the internal implementation of the ODataLib from the interface.
        /// </summary>
        /// <param name="requestMessage">The request message to wrap.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        /// <param name="enableMessageStreamDisposal">true if the stream returned should be disposed calls.</param>
        /// <param name="maxMessageSize">The maximum size of the message in bytes (or a negative value if no maximum applies).</param>
        internal ODataRequestMessage(IODataRequestMessage requestMessage, bool writing, bool enableMessageStreamDisposal, long maxMessageSize)
            : base(writing, enableMessageStreamDisposal, maxMessageSize)
        {
            Debug.Assert(requestMessage != null, "requestMessage != null");

            this.requestMessage = requestMessage;
        }

        /// <summary>
        /// The request Url for this request message.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this.requestMessage.Url;
            }

            set
            {
                throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// The HTTP method used for this request message.
        /// </summary>
        public string Method
        {
            get
            {
                return this.requestMessage.Method;
            }

            set
            {
                throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.requestMessage.Headers;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public override string GetHeader(string headerName)
        {
            return this.requestMessage.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            this.VerifyCanSetHeader();
            this.requestMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Synchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Stream GetStream()
        {
            return this.GetStream(this.requestMessage.GetStream, /*isRequest*/ true);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Task<Stream> GetStreamAsync()
        {
            IODataRequestMessageAsync asyncRequestMessage = this.requestMessage as IODataRequestMessageAsync;
            if (asyncRequestMessage == null)
            {
                throw new ODataException(Strings.ODataRequestMessage_AsyncNotAvailable);
            }

            return this.GetStreamAsync(asyncRequestMessage.GetStreamAsync, /*isRequest*/ true);
        }
#endif

        /// <summary>
        /// Queries the message for the specified interface type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface to query for.</typeparam>
        /// <returns>The instance of the interface asked for or null if it was not implemented by the message.</returns>
        internal override TInterface QueryInterface<TInterface>()
        {
            return this.requestMessage as TInterface;
        }
    }
}
