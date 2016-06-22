//---------------------------------------------------------------------
// <copyright file="ODataResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Wrapper class around an IODataResponseMessageAsync to isolate our code from the interface implementation.
    /// </summary>
    /// <remarks>
    /// This class also implements the message interface since it is passed to the payload kind
    /// detection logic on the format implementations and manages the buffering read stream.
    /// </remarks>
    internal sealed class ODataResponseMessage : ODataMessage,
#if PORTABLELIB
 IODataResponseMessageAsync
#else
        IODataResponseMessage
#endif
    {
        /// <summary>The response message this class is wrapping.</summary>
        private readonly IODataResponseMessage responseMessage;

        /// <summary>
        /// Constructs an internal wrapper around the <paramref name="responseMessage"/>
        /// that isolates the internal implementation of the ODataLib from the interface.
        /// </summary>
        /// <param name="responseMessage">The response message to wrap.</param>
        /// <param name="writing">true if the message is being written; false when it is read.</param>
        /// <param name="enableMessageStreamDisposal">true if the stream returned should be disposed calls.</param>
        /// <param name="maxMessageSize">The maximum size of the message in bytes (or a negative number if no maximum applies).</param>
        internal ODataResponseMessage(IODataResponseMessage responseMessage, bool writing, bool enableMessageStreamDisposal, long maxMessageSize)
            : base(writing, enableMessageStreamDisposal, maxMessageSize)
        {
            Debug.Assert(responseMessage != null, "responseMessage != null");

            this.responseMessage = responseMessage;
        }

        /// <summary>
        /// The result status code of the response message.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.responseMessage.StatusCode;
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
                return this.responseMessage.Headers;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public override string GetHeader(string headerName)
        {
            return this.responseMessage.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            this.VerifyCanSetHeader();
            this.responseMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Stream GetStream()
        {
            return this.GetStream(this.responseMessage.GetStream, /*isRequest*/ false);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Task<Stream> GetStreamAsync()
        {
            IODataResponseMessageAsync asyncResponseMessage = this.responseMessage as IODataResponseMessageAsync;
            if (asyncResponseMessage == null)
            {
                throw new ODataException(Strings.ODataResponseMessage_AsyncNotAvailable);
            }

            return this.GetStreamAsync(asyncResponseMessage.GetStreamAsync, /*isRequest*/ false);
        }
#endif

        /// <summary>
        /// Queries the message for the specified interface type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface to query for.</typeparam>
        /// <returns>The instance of the interface asked for or null if it was not implemented by the message.</returns>
        internal override TInterface QueryInterface<TInterface>()
        {
            return this.responseMessage as TInterface;
        }
    }
}
