//---------------------------------------------------------------------
// <copyright file="ODataAsynchronousWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for writing OData async messages; also verifies the proper count of write calls on the writer.
    /// </summary>
    public sealed class ODataAsynchronousWriter : IODataOutputInStreamErrorListener
    {
        /// <summary>
        /// The output context to write to.
        /// </summary>
        private readonly ODataRawOutputContext rawOutputContext;

        /// <summary>
        /// The optional dependency injection container to get related services for message writing.
        /// </summary>
        private readonly IServiceProvider container;

        /// <summary>
        /// Prevent the response message from being created more than one time since an async response message can only contain one inner message.
        /// </summary>
        private bool responseMessageCreated;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawOutputContext">The output context to write to.</param>
        internal ODataAsynchronousWriter(ODataRawOutputContext rawOutputContext)
        {
            Debug.Assert(rawOutputContext != null, "rawOutputContext != null");

            this.rawOutputContext = rawOutputContext;
            this.container = rawOutputContext.Container;
            this.rawOutputContext.InitializeRawValueWriter();
        }

        /// <summary>
        /// Creates a message for writing an async response.
        /// </summary>
        /// <returns>The message that can be used to write the async response.</returns>
        public ODataAsynchronousResponseMessage CreateResponseMessage()
        {
            this.VerifyCanCreateResponseMessage(true);

            return this.CreateResponseMessageImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates a message for writing an async response.
        /// </summary>
        /// <returns>The message that can be used to write the async response.</returns>
        public Task<ODataAsynchronousResponseMessage> CreateResponseMessageAsync()
        {
            this.VerifyCanCreateResponseMessage(false);

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateResponseMessageImplementation());
        }
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public void Flush()
        {
            this.VerifyCanFlush(true);

            this.rawOutputContext.Flush();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public Task FlushAsync()
        {
            this.VerifyCanFlush(false);

            return this.rawOutputContext.FlushAsync();
        }
#endif

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        void IODataOutputInStreamErrorListener.OnInStreamError()
        {
            this.rawOutputContext.VerifyNotDisposed();
            this.rawOutputContext.TextWriter.Flush();

            throw new ODataException(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync);
        }

        /// <summary>
        /// Validates that the async writer is not disposed.
        /// </summary>
        private void ValidateWriterNotDisposed()
        {
            this.rawOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Verifies that a call is allowed to the writer.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.rawOutputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataAsyncWriter_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.rawOutputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataAsyncWriter_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanFlush(bool synchronousCall)
        {
            this.rawOutputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Verifies that calling CreateResponseMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateResponseMessage(bool synchronousCall)
        {
            this.ValidateWriterNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.rawOutputContext.WritingResponse)
            {
                throw new ODataException(Strings.ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse);
            }

            if (responseMessageCreated)
            {
                throw new ODataException(Strings.ODataAsyncWriter_CannotCreateResponseMoreThanOnce);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataAsynchronousResponseMessage"/> for writing an operation of an async response - implementation of the actual functionality.
        /// </summary>
        /// <returns>The message that can be used to write the response.</returns>
        private ODataAsynchronousResponseMessage CreateResponseMessageImplementation()
        {
            var responseMessage = ODataAsynchronousResponseMessage.CreateMessageForWriting(rawOutputContext.OutputStream, this.WriteInnerEnvelope, this.container);

            responseMessageCreated = true;

            return responseMessage;
        }

        /// <summary>
        /// Writes the envelope for the inner HTTP message.
        /// </summary>
        /// <param name="responseMessage">The response message to write the envelope.</param>
        private void WriteInnerEnvelope(ODataAsynchronousResponseMessage responseMessage)
        {
            // Write response line.
            string statusMessage = HttpUtils.GetStatusMessage(responseMessage.StatusCode);
            this.rawOutputContext.TextWriter.WriteLine("{0} {1} {2}", ODataConstants.HttpVersionInAsync, responseMessage.StatusCode, statusMessage);

            // Write headers.
            if (responseMessage.Headers != null)
            {
                foreach (var headerPair in responseMessage.Headers)
                {
                    string headerName = headerPair.Key;
                    string headerValue = headerPair.Value;
                    this.rawOutputContext.TextWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", headerName, headerValue));
                }
            }

            // Write CRLF.
            this.rawOutputContext.TextWriter.WriteLine();

            // Flush the writer since we won't be able to access it anymore.
            this.rawOutputContext.TextWriter.Flush();
        }
    }
}
