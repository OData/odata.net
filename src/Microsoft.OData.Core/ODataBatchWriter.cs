//---------------------------------------------------------------------
// <copyright file="ODataBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for writing OData batch messages; also verifies the proper sequence of write calls on the writer.
    /// </summary>
    public sealed class ODataBatchWriter : IODataBatchOperationListener, IODataOutputInStreamErrorListener
    {
        /// <summary>The output context to write to.</summary>
        private readonly ODataRawOutputContext rawOutputContext;

        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

        /// <summary>The batch-specific URL converter that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        private readonly ODataBatchPayloadUriConverter payloadUriConverter;

        /// <summary>The dependency injection container to get related services.</summary>
        private readonly IServiceProvider container;

        /// <summary>The state the writer currently is in.</summary>
        private BatchWriterState state;

        /// <summary>
        /// The boundary string for the current changeset (only set when writing a changeset,
        /// e.g., after WriteStartChangeSet has been called and before WriteEndChangeSet is called).
        /// </summary>
        /// <remarks>When not writing a changeset this field is null.</remarks>
        private string changeSetBoundary;

        /// <summary>
        /// A flag to indicate whether the batch start boundary has been written or not; important to support writing of empty batches.
        /// </summary>
        private bool batchStartBoundaryWritten;

        /// <summary>
        /// A flags to indicate whether the current changeset start boundary has been written or not.
        /// This is false if a changeset has been started by no changeset boundary was written, and true once the first changeset
        /// boundary for the current changeset has been written.
        /// </summary>
        private bool changesetStartBoundaryWritten;

        /// <summary>The request message for the operation that is currently written if it's a request;
        /// or null if no part is written right now or it's a response part.</summary>
        private ODataBatchOperationRequestMessage currentOperationRequestMessage;

        /// <summary>The response message for the operation that is currently written if it's a response;
        /// or null if no part is written right now or it's a request part.</summary>
        private ODataBatchOperationResponseMessage currentOperationResponseMessage;

        /// <summary>
        /// The value of the Content-ID header of the current operation (or null if no Content-ID header exists).
        /// </summary>
        /// <remarks>
        /// Note that the current Content-ID header is not included immediately in the content ID cache
        /// since the current content ID will only be visible to subsequent operations.
        /// </remarks>
        private string currentOperationContentId;

        /// <summary>The current size of the batch message, i.e., how many query operations and changesets have been written.</summary>
        private uint currentBatchSize;

        /// <summary>The current size of the active changeset, i.e., how many request have been written for the changeset.</summary>
        private uint currentChangeSetSize;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawOutputContext">The output context to write to.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        internal ODataBatchWriter(ODataRawOutputContext rawOutputContext, string batchBoundary)
        {
            Debug.Assert(rawOutputContext != null, "rawOutputContext != null");
            Debug.Assert(
                rawOutputContext.MessageWriterSettings.BaseUri == null || rawOutputContext.MessageWriterSettings.BaseUri.IsAbsoluteUri,
                "We should have validated that baseUri is absolute.");

            ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary");

            this.rawOutputContext = rawOutputContext;
            this.container = rawOutputContext.Container;
            this.batchBoundary = batchBoundary;
            this.payloadUriConverter = new ODataBatchPayloadUriConverter(rawOutputContext.PayloadUriConverter);
            this.rawOutputContext.InitializeRawValueWriter();
        }

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        private enum BatchWriterState
        {
            /// <summary>The writer is in initial state; nothing has been written yet.</summary>
            Start,

            /// <summary>WriteStartBatch has been called.</summary>
            BatchStarted,

            /// <summary>WriteStartChangeSet has been called.</summary>
            ChangeSetStarted,

            /// <summary>CreateOperationRequestMessage/CreateOperationResponseMessage has been called.</summary>
            OperationCreated,

            /// <summary>
            /// ODataMessage.GetStreamAsync() has been called on an operation which caused a <see cref="ODataBatchOperationStream"/> to be created;
            /// the batch writer is unusable while an operation is being written.
            /// </summary>
            OperationStreamRequested,

            /// <summary>The stream for writing the content of an operation has been disposed. The batch writer can now be used again.</summary>
            OperationStreamDisposed,

            /// <summary>WriteEndChangeSet has been called.</summary>
            ChangeSetCompleted,

            /// <summary>WriteEndBatch has been called.</summary>
            BatchCompleted,

            /// <summary>The writer is in error state; nothing can be written anymore except the error payload.</summary>
            Error
        }

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no operation is written right now or it's a response operation.</summary>
        private ODataBatchOperationRequestMessage CurrentOperationRequestMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || !this.rawOutputContext.WritingResponse, "Request message can only be filled when writing request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationRequestMessage;
            }

            set
            {
                Debug.Assert(value == null || !this.rawOutputContext.WritingResponse, "Can only set the request message if we're writing a request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                this.currentOperationRequestMessage = value;
            }
        }

        /// <summary>The response message for the operation that is currently written if it's a response;
        /// or null if no operation is written right now or it's a request operation.</summary>
        private ODataBatchOperationResponseMessage CurrentOperationResponseMessage
        {
            get
            {
                Debug.Assert(this.currentOperationResponseMessage == null || this.rawOutputContext.WritingResponse, "Response message can only be filled when writing response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationResponseMessage;
            }

            set
            {
                Debug.Assert(value == null || this.rawOutputContext.WritingResponse, "Can only set the response message if we're writing a response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                this.currentOperationResponseMessage = value;
            }
        }

        /// <summary>The message for the operation that is currently written; or null if no operation is written right now.</summary>
        private ODataBatchOperationMessage CurrentOperationMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                if (this.currentOperationRequestMessage != null)
                {
                    Debug.Assert(!this.rawOutputContext.WritingResponse, "Request message can only be set when writing request.");
                    return this.currentOperationRequestMessage.OperationMessage;
                }
                else if (this.currentOperationResponseMessage != null)
                {
                    Debug.Assert(this.rawOutputContext.WritingResponse, "Response message can only be set when writing response.");
                    return this.currentOperationResponseMessage.OperationMessage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>Starts a new batch; can be only called once and as first call.</summary>
        public void WriteStartBatch()
        {
            this.VerifyCanWriteStartBatch(true);
            this.WriteStartBatchImplementation();
        }

#if PORTABLELIB
        /// <summary>Asynchronously starts a new batch; can be only called once and as first call.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartBatchAsync()
        {
            this.VerifyCanWriteStartBatch(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteStartBatchImplementation);
        }
#endif

        /// <summary>Ends a batch; can only be called after WriteStartBatch has been called and if no other active changeset or operation exist.</summary>
        public void WriteEndBatch()
        {
            this.VerifyCanWriteEndBatch(true);
            this.WriteEndBatchImplementation();

            // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
            this.Flush();
        }

#if PORTABLELIB
        /// <summary>Asynchronously ends a batch; can only be called after WriteStartBatch has been called and if no other active change set or operation exist.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndBatchAsync()
        {
            this.VerifyCanWriteEndBatch(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndBatchImplementation)

                // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                .FollowOnSuccessWithTask(task => this.FlushAsync());
        }
#endif

        /// <summary>Starts a new changeset; can only be called after WriteStartBatch and if no other active operation or changeset exists.</summary>
        public void WriteStartChangeset()
        {
            this.VerifyCanWriteStartChangeset(true);
            this.WriteStartChangesetImplementation();
        }

#if PORTABLELIB
        /// <summary>Asynchronously starts a new change set; can only be called after WriteStartBatch and if no other active operation or change set exists.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartChangesetAsync()
        {
            this.VerifyCanWriteStartChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteStartChangesetImplementation);
        }
#endif

        /// <summary>Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.</summary>
        public void WriteEndChangeset()
        {
            this.VerifyCanWriteEndChangeset(true);
            this.WriteEndChangesetImplementation();
        }

#if PORTABLELIB
        /// <summary>Asynchronously ends an active change set; this can only be called after WriteStartChangeset and only once for each change set.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndChangesetAsync()
        {
            this.VerifyCanWriteEndChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndChangesetImplementation);
        }
#endif

        /// <summary>Creates an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to write the request operation.</returns>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId)
        {
            return CreateOperationRequestMessage(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
        }

        /// <summary>Creates an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to write the request operation.</returns>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption)
        {
            this.VerifyCanCreateOperationRequestMessage(true, method, uri, contentId);
            return this.CreateOperationRequestMessageImplementation(method, uri, contentId, payloadUriOption);
        }

#if PORTABLELIB
        /// <summary>Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId)
        {
            return CreateOperationRequestMessageAsync(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
        }

        /// <summary>Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption)
        {
            this.VerifyCanCreateOperationRequestMessage(false, method, uri, contentId);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                () => this.CreateOperationRequestMessageImplementation(method, uri, contentId, payloadUriOption));
        }
#endif

        /// <summary>Creates a message for writing an operation of a batch response.</summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage(string contentId)
        {
            this.VerifyCanCreateOperationResponseMessage(true);
            return this.CreateOperationResponseMessageImplementation(contentId);
        }

#if PORTABLELIB
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for writing an operation of a batch response.</summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>A task that when completed returns the newly created operation response message.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync(string contentId)
        {
            this.VerifyCanCreateOperationResponseMessage(false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(
                () => this.CreateOperationResponseMessageImplementation(contentId));
        }
#endif

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public void Flush()
        {
            this.VerifyCanFlush(true);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.rawOutputContext.Flush();
            }
            catch
            {
                this.SetState(BatchWriterState.Error);
                throw;
            }
        }

#if PORTABLELIB
        /// <summary>Flushes the write buffer to the underlying stream asynchronously.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public Task FlushAsync()
        {
            this.VerifyCanFlush(false);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            return this.rawOutputContext.FlushAsync().FollowOnFaultWith(t => this.SetState(BatchWriterState.Error));
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.rawOutputContext.FlushBuffers();

            // Dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            this.DisposeBatchWriterAndSetContentStreamRequestedState();
        }

#if PORTABLELIB
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the operation;
        /// null if no such action exists.
        /// </returns>
        Task IODataBatchOperationListener.BatchOperationContentStreamRequestedAsync()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Asynchronously flush the async buffered stream to the underlying message stream (if there's any);
            // then dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            return this.rawOutputContext.FlushBuffersAsync()
                .FollowOnSuccessWith(task => this.DisposeBatchWriterAndSetContentStreamRequestedState());
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamDisposed()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");

            this.SetState(BatchWriterState.OperationStreamDisposed);
            this.CurrentOperationRequestMessage = null;
            this.CurrentOperationResponseMessage = null;
            this.rawOutputContext.InitializeRawValueWriter();
        }

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        void IODataOutputInStreamErrorListener.OnInStreamError()
        {
            this.rawOutputContext.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            this.rawOutputContext.TextWriter.Flush();

            // The OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        private static bool IsErrorState(BatchWriterState state)
        {
            return state == BatchWriterState.Error;
        }

        /// <summary>
        /// Verifies that calling WriteStartBatch is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteStartBatch(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Starts a new batch - implementation of the actual functionality.
        /// </summary>
        private void WriteStartBatchImplementation()
        {
            this.SetState(BatchWriterState.BatchStarted);
        }

        /// <summary>
        /// Verifies that calling WriteEndBatch is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEndBatch(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        private void WriteEndBatchImplementation()
        {
            Debug.Assert(
                this.batchStartBoundaryWritten || this.CurrentOperationMessage == null,
                "If not batch boundary was written we must not have an active message.");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // write the end boundary for the batch
            ODataBatchWriterUtils.WriteEndBoundary(this.rawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);

            // For compatibility with WCF DS we write a newline after the end batch boundary.
            // Technically it's not needed, but it doesn't violate anything either.
            this.rawOutputContext.TextWriter.WriteLine();
        }

        /// <summary>
        /// Verifies that calling WriteStartChangeset is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteStartChangeset(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        private void WriteStartChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangeSetStarted);
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);

            // write the boundary string
            ODataBatchWriterUtils.WriteStartBoundary(this.rawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
            this.batchStartBoundaryWritten = true;

            // write the change set headers
            ODataBatchWriterUtils.WriteChangeSetPreamble(this.rawOutputContext.TextWriter, this.changeSetBoundary);
            this.changesetStartBoundaryWritten = false;
        }

        /// <summary>
        /// Verifies that calling WriteEndChangeset is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEndChangeset(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        private void WriteEndChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            string currentChangeSetBoundary = this.changeSetBoundary;

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangeSetCompleted);

            // In the case of an empty changeset the start changeset boundary has not been written yet
            // we will leave it like that, since we want the empty changeset to be represented only as
            // the end changeset boundary.
            // Due to WCF DS V2 compatiblity we must not write the start boundary in this case
            // otherwise WCF DS V2 won't be able to read it (it fails on the start-end boundary empty changeset).

            // write the end boundary for the change set
            ODataBatchWriterUtils.WriteEndBoundary(this.rawOutputContext.TextWriter, currentChangeSetBoundary, !this.changesetStartBoundaryWritten);

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.payloadUriConverter.Reset();
            this.currentOperationContentId = null;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        private void VerifyCanCreateOperationRequestMessage(bool synchronousCall, string method, Uri uri, string contentId)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.rawOutputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }

            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(method, "method");
            if (this.changeSetBoundary != null)
            {
                if (HttpUtils.IsQueryMethod(method))
                {
                    this.ThrowODataException(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest(method));
                }

                if (string.IsNullOrEmpty(contentId))
                {
                    this.ThrowODataException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(ODataConstants.ContentIdHeader));
                }
            }

            ExceptionUtils.CheckArgumentNotNull(uri, "uri");
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        private ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption)
        {
            if (this.changeSetBoundary == null)
            {
                this.InterceptException(this.IncreaseBatchSize);
            }
            else
            {
                this.InterceptException(this.IncreaseChangeSetSize);
            }

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // Add a potential Content-ID header to the URL resolver so that it will be available
            // to subsequent operations.
            // Note that what we add here is the Content-ID header of the previous operation (if any).
            // This also means that the Content-ID of the last operation in a changeset will never get
            // added to the cache which is fine since we cannot reference it anywhere.
            if (this.currentOperationContentId != null)
            {
                this.payloadUriConverter.AddContentId(this.currentOperationContentId);
            }

            this.InterceptException(() => uri = ODataBatchUtils.CreateOperationRequestUri(uri, this.rawOutputContext.MessageWriterSettings.BaseUri, this.payloadUriConverter));

            // create the new request operation
            this.CurrentOperationRequestMessage = ODataBatchOperationRequestMessage.CreateWriteMessage(
                this.rawOutputContext.OutputStream,
                method,
                uri,
                /*operationListener*/ this,
                this.payloadUriConverter,
                this.container);

            if (this.changeSetBoundary != null)
            {
                this.RememberContentIdHeader(contentId);
            }

            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request line
            ODataBatchWriterUtils.WriteRequestPreamble(this.rawOutputContext.TextWriter, method, uri,
                this.rawOutputContext.MessageWriterSettings.BaseUri, changeSetBoundary != null, contentId,
                payloadUriOption);

            return this.CurrentOperationRequestMessage;
        }

        /// <summary>
        /// Verifies that calling CreateOperationResponseMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.rawOutputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        private ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            this.CurrentOperationResponseMessage = ODataBatchOperationResponseMessage.CreateWriteMessage(
                this.rawOutputContext.OutputStream,
                /*operationListener*/ this,
                this.payloadUriConverter.BatchMessagePayloadUriConverter,
                this.container);
            this.SetState(BatchWriterState.OperationCreated);

            Debug.Assert(this.currentOperationContentId == null, "The Content-ID header is only supported in request messages.");

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request separator line
            ODataBatchWriterUtils.WriteResponsePreamble(this.rawOutputContext.TextWriter, changeSetBoundary != null, contentId);

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        private void StartBatchOperationContent()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.rawOutputContext.TextWriter != null, "Must have a batch writer!");

            // write the pending headers (if any)
            this.WritePendingMessageData(false);

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.rawOutputContext.TextWriter.Flush();
        }

        /// <summary>
        /// Disposes the batch writer and set the 'OperationStreamRequested' batch writer state;
        /// called after the flush operation(s) have completed.
        /// </summary>
        private void DisposeBatchWriterAndSetContentStreamRequestedState()
        {
            this.rawOutputContext.CloseWriter();

            this.SetState(BatchWriterState.OperationStreamRequested);
        }

        /// <summary>
        /// Remember a non-null Content-ID header for change set request operations.
        /// If a non-null content ID header is specified for a change set request operation, record it in the URL resolver.
        /// </summary>
        /// <param name="contentId">The Content-ID header value read from the message.</param>
        /// <remarks>
        /// Note that the content ID of this operation will only
        /// become visible once this operation has been written
        /// and OperationCompleted has been called on the URL resolver.
        /// </remarks>
        private void RememberContentIdHeader(string contentId)
        {
            // The Content-ID header is only supported in request messages and inside of changesets.
            Debug.Assert(this.currentOperationRequestMessage != null, "this.currentOperationRequestMessage != null");
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // Set the current content ID. If no Content-ID header is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.currentOperationContentId = contentId;

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.payloadUriConverter.ContainsContentId(contentId))
            {
                throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(contentId));
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
            if (this.state == BatchWriterState.OperationStreamRequested)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
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
                    throw new ODataException(Strings.ODataBatchWriter_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.rawOutputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataBatchWriter_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void InterceptException(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                if (!IsErrorState(this.state))
                {
                    this.SetState(BatchWriterState.Error);
                }

                throw;
            }
        }

        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        private void SetState(BatchWriterState newState)
        {
            this.InterceptException(() => this.ValidateTransition(newState));

            switch (newState)
            {
                case BatchWriterState.BatchStarted:
                    Debug.Assert(!this.batchStartBoundaryWritten, "The batch boundary must not be written before calling WriteStartBatch.");
                    break;
                case BatchWriterState.ChangeSetStarted:
                    Debug.Assert(this.changeSetBoundary == null, "this.changeSetBoundary == null");
                    this.changeSetBoundary = ODataBatchWriterUtils.CreateChangeSetBoundary(this.rawOutputContext.WritingResponse);
                    break;
                case BatchWriterState.ChangeSetCompleted:
                    Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");
                    this.changeSetBoundary = null;
                    break;
            }

            this.state = newState;
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Validating the transition in the state machine should stay in a single method.")]
        private void ValidateTransition(BatchWriterState newState)
        {
            if (!IsErrorState(this.state) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
                return;
            }

            // make sure that we are not starting a changeset when one is already active
            if (newState == BatchWriterState.ChangeSetStarted)
            {
                if (this.changeSetBoundary != null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
                }
            }

            // make sure that we are not completing a changeset without an active changeset
            if (newState == BatchWriterState.ChangeSetCompleted)
            {
                if (this.changeSetBoundary == null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
                }
            }

            // make sure that we are not completing a batch while a changeset is still active
            if (newState == BatchWriterState.BatchCompleted)
            {
                if (this.changeSetBoundary != null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
                }
            }

            switch (this.state)
            {
                case BatchWriterState.Start:
                    if (newState != BatchWriterState.BatchStarted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromStart);
                    }

                    break;
                case BatchWriterState.BatchStarted:
                    if (newState != BatchWriterState.ChangeSetStarted && newState != BatchWriterState.OperationCreated && newState != BatchWriterState.BatchCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted);
                    }

                    break;
                case BatchWriterState.ChangeSetStarted:
                    if (newState != BatchWriterState.OperationCreated && newState != BatchWriterState.ChangeSetCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
                    }

                    break;
                case BatchWriterState.OperationCreated:
                    if (newState != BatchWriterState.OperationCreated &&
                        newState != BatchWriterState.OperationStreamRequested &&
                        newState != BatchWriterState.ChangeSetStarted &&
                        newState != BatchWriterState.ChangeSetCompleted &&
                        newState != BatchWriterState.BatchCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationCreated);
                    }

                    Debug.Assert(newState != BatchWriterState.OperationStreamDisposed, "newState != BatchWriterState.OperationStreamDisposed");

                    break;
                case BatchWriterState.OperationStreamRequested:
                    if (newState != BatchWriterState.OperationStreamDisposed)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
                    }

                    break;
                case BatchWriterState.OperationStreamDisposed:
                    if (newState != BatchWriterState.OperationCreated &&
                        newState != BatchWriterState.ChangeSetStarted &&
                        newState != BatchWriterState.ChangeSetCompleted &&
                        newState != BatchWriterState.BatchCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
                    }

                    break;
                case BatchWriterState.ChangeSetCompleted:
                    if (newState != BatchWriterState.BatchCompleted &&
                        newState != BatchWriterState.ChangeSetStarted &&
                        newState != BatchWriterState.OperationCreated)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
                    }

                    break;
                case BatchWriterState.BatchCompleted:
                    // no more state transitions should happen once in completed state
                    throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
                case BatchWriterState.Error:
                    if (newState != BatchWriterState.Error)
                    {
                        // No more state transitions once we are in error state except for the fatal error
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.state.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchWriter_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Validates that the batch writer is ready to process a new write request.
        /// </summary>
        private void ValidateWriterReady()
        {
            this.rawOutputContext.VerifyNotDisposed();

            // If the operation stream was requested but not yet disposed, the writer can't be used to do anything.
            if (this.state == BatchWriterState.OperationStreamRequested)
            {
                throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
        }

        /// <summary>
        /// Write any pending headers for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        private void WritePendingMessageData(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage != null)
            {
                Debug.Assert(this.rawOutputContext.TextWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.rawOutputContext.WritingResponse, "If the response message is available we must be writing response.");
                    int statusCode = this.CurrentOperationResponseMessage.StatusCode;
                    string statusMessage = HttpUtils.GetStatusMessage(statusCode);
                    this.rawOutputContext.TextWriter.WriteLine("{0} {1} {2}", ODataConstants.HttpVersionInBatching, statusCode, statusMessage);
                }

                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        string headerName = headerPair.Key;
                        string headerValue = headerPair.Value;
                        this.rawOutputContext.TextWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", headerName, headerValue));
                    }
                }

                // write CRLF after the headers (or request/response line if there are no headers)
                this.rawOutputContext.TextWriter.WriteLine();

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;
                }
            }
        }

        /// <summary>
        /// Writes the start boundary for an operation. This is either the batch or the changeset boundary.
        /// </summary>
        private void WriteStartBoundaryForOperation()
        {
            if (this.changeSetBoundary == null)
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.rawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.rawOutputContext.TextWriter, this.changeSetBoundary, !this.changesetStartBoundaryWritten);
                this.changesetStartBoundaryWritten = true;
            }
        }

        /// <summary>
        /// Sets the 'Error' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.SetState(BatchWriterState.Error);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseBatchSize()
        {
            this.currentBatchSize++;

            if (this.currentBatchSize > this.rawOutputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxBatchSizeExceeded(this.rawOutputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch));
            }
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseChangeSetSize()
        {
            this.currentChangeSetSize++;

            if (this.currentChangeSetSize > this.rawOutputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(this.rawOutputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset));
            }
        }

        /// <summary>
        /// Resets the size of the current change set to 0.
        /// </summary>
        private void ResetChangeSetSize()
        {
            this.currentChangeSetSize = 0;
        }
    }
}
