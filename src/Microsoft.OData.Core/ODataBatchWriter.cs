//---------------------------------------------------------------------
// <copyright file="ODataBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;

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
    /// Abstract class for writing OData batch messages; also verifies the proper sequence of write calls on the writer.
    /// </summary>
    public abstract class ODataBatchWriter : IODataBatchOperationListener, IODataOutputInStreamErrorListener
    {
        /// <summary>The output context to write to.</summary>
        private readonly ODataOutputContext outputContext;

        /// <summary>The batch-specific URL converter that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        private readonly ODataBatchPayloadUriConverter payloadUriConverter;

        /// <summary>The dependency injection container to get related services.</summary>
        private readonly IServiceProvider container;

        /// <summary>The state the writer currently is in.</summary>
        private BatchWriterState state;

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
        /// <param name="outputContext">The output context to write to.</param>
        internal ODataBatchWriter(ODataOutputContext outputContext)
        {
            Debug.Assert(outputContext != null, "outputContext != null");
            Debug.Assert(
                outputContext.MessageWriterSettings.BaseUri == null || outputContext.MessageWriterSettings.BaseUri.IsAbsoluteUri,
                "We should have validated that baseUri is absolute.");

            this.outputContext = outputContext;
            this.container = outputContext.Container;
            this.payloadUriConverter = new ODataBatchPayloadUriConverter(outputContext.PayloadUriConverter);
        }

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        protected enum BatchWriterState
        {
            /// <summary>The writer is in initial state; nothing has been written yet.</summary>
            Start,

            /// <summary>WriteStartBatch has been called.</summary>
            BatchStarted,

            /// <summary>WriteStartChangeSet has been called.</summary>
            ChangesetStarted,

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
            ChangesetCompleted,

            /// <summary>WriteEndBatch has been called.</summary>
            BatchCompleted,

            /// <summary>The writer is in error state; nothing can be written anymore except the error payload.</summary>
            Error
        }

        /// <summary>
        /// Gets or Sets the content Id of the current operation.
        /// </summary>
        protected string CurrentOperationContentId
        {
            get { return this.currentOperationContentId; }
        }

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no operation is written right now or it's a response operation.</summary>
        protected ODataBatchOperationRequestMessage CurrentOperationRequestMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || !this.outputContext.WritingResponse, "Request message can only be filled when writing request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or response message can be set, not both.");
                return this.currentOperationRequestMessage;
            }

            set
            {
                Debug.Assert(value == null || !this.outputContext.WritingResponse, "Can only set the request message if we're writing a request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or response message can be set, not both.");
                this.currentOperationRequestMessage = value;
            }
        }

        /// <summary>The response message for the operation that is currently written if it's a response;
        /// or null if no operation is written right now or it's a request operation.</summary>
        protected ODataBatchOperationResponseMessage CurrentOperationResponseMessage
        {
            get
            {
                Debug.Assert(this.currentOperationResponseMessage == null || this.outputContext.WritingResponse, "Response message can only be filled when writing response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or response message can be set, not both.");
                return this.currentOperationResponseMessage;
            }

            set
            {
                Debug.Assert(value == null || this.outputContext.WritingResponse, "Can only set the response message if we're writing a response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or response message can be set, not both.");
                this.currentOperationResponseMessage = value;
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

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);
        }

#if PORTABLELIB
        /// <summary>Asynchronously starts a new change set; can only be called after WriteStartBatch and if no other active operation or change set exists.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartChangesetAsync()
        {
            this.VerifyCanWriteStartChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteStartChangesetImplementation)
                .FollowOnSuccessWith(t =>
                    {
                        // reset the size of the current changeset and increase the size of the batch
                        this.ResetChangeSetSize();
                        this.InterceptException(this.IncreaseBatchSize);
                    });
        }
#endif

        /// <summary>Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.</summary>
        public void WriteEndChangeset()
        {
            this.VerifyCanWriteEndChangeset(true);
            this.WriteEndChangesetImplementation();

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.payloadUriConverter.Reset();
            this.currentOperationContentId = null;
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

            if (!this.IsInsideSubBatch())
            {
                this.InterceptException(this.IncreaseBatchSize);
            }
            else
            {
                this.InterceptException(this.IncreaseChangeSetSize);
            }

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

            if (!this.IsInsideSubBatch())
            {
                this.InterceptException(this.IncreaseBatchSize);
            }
            else
            {
                this.InterceptException(this.IncreaseChangeSetSize);
            }

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
                this.FlushSynchronously();
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

            // Make sure we switch to writer state Error if an exception is thrown during flushing.
            return this.FlushAsynchronously().FollowOnFaultWith(t => this.SetState(BatchWriterState.Error));
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public abstract void BatchOperationContentStreamRequested();

#if PORTABLELIB
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the operation;
        /// null if no such action exists.
        /// </returns>
        public abstract Task BatchOperationContentStreamRequestedAsync();
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        public abstract void BatchOperationContentStreamDisposed();

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        public abstract void OnInStreamError();

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then re-throw the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        internal void InterceptException(Action action)
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
        /// Sets the 'Error' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        internal void ThrowODataException(string errorMessage)
        {
            this.SetState(BatchWriterState.Error);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected abstract void FlushSynchronously();

#if PORTABLELIB
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected abstract Task FlushAsynchronously();
#endif

        /// <summary>
        /// Ends a batch.
        /// </summary>
        protected abstract void WriteEndBatchImplementation();

        /// <summary>
        /// Starts a new changeset.
        /// </summary>
        protected abstract void WriteStartChangesetImplementation();

        /// <summary>
        /// Ends an active changeset.
        /// </summary>
        protected abstract void WriteEndChangesetImplementation();

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        protected abstract ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId);

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        protected abstract void StartBatchOperationContent();

        /// <summary>
        /// Whether the writer is currently processing inside a sub-batch (changeset or atomic group).
        /// </summary>
        /// <returns>True if the writer processing is inside a sub-batch.</returns>
        protected abstract bool IsInsideSubBatch();

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method, Uri uri,
            string contentId, BatchPayloadUriOption payloadUriOption);


        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected void SetState(BatchWriterState newState)
        {
            this.InterceptException(() => this.ValidateTransition(
                newState,
                () => ValidateTransitionImplementation(newState)));

            SetStateImplementation(newState);

            this.state = newState;
        }

        /// <summary>
        /// Additional processing required when setting a new writer state.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected virtual void SetStateImplementation(BatchWriterState newState)
        {
        }

        /// <summary>
        /// Additional validation required when setting a new writer state.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected virtual void ValidateTransitionImplementation(BatchWriterState newState)
        {
        }

        /// <summary>
        /// Verifies that the writer is not disposed.
        /// </summary>
        protected abstract void VerifyNotDisposed();

        /// <summary>
        /// Starts a new batch.
        /// </summary>
        protected abstract void WriteStartBatchImplementation();

        /// <summary>
        /// Writer specific implementation to verify that CreateOperationRequestMessage is valid.
        /// Default implementation is no-op, and derived class can override as needed.
        /// </summary>
        /// <param name="method">The HTTP method to be validated.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The content Id string to be validated.</param>
        protected virtual void VerifyCanCreateOperationRequestMessageImplementation(string method, Uri uri, string contentId)
        {
        }

        /// <summary>
        /// Wrapper method to create an operation request message that can be used to write the operation content to, utilizing
        /// private members <see cref="ODataBatchPayloadUriConverter"/> and <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <param name="method">The HTTP method to use for the message to create.</param>
        /// <param name="uri">The request URL for the message to create.</param>
        /// <returns>An <see cref="ODataBatchOperationRequestMessage"/> to write the request content to.</returns>
        protected ODataBatchOperationRequestMessage BuildOperationRequestMessage(Stream outputStream, string method, Uri uri)
        {
            return ODataBatchOperationRequestMessage.CreateWriteMessage(
                outputStream,
                method,
                uri,
                this,
                this.payloadUriConverter,
                this.container);
        }

        /// <summary>
        /// Wrapper method to create an operation response message that can be used to write the operation content to, utilizing
        /// private members <see cref="ODataBatchPayloadUriConverter"/> and <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <returns>An <see cref="ODataBatchOperationResponseMessage"/> that can be used to write the operation content.</returns>
        protected ODataBatchOperationResponseMessage BuildOperationResponseMessage(Stream outputStream)
        {
            return ODataBatchOperationResponseMessage.CreateWriteMessage(
                outputStream,
                this,
                this.payloadUriConverter.BatchMessagePayloadUriConverter,
                this.container);
        }

        /// <summary>
        /// Creates the URI for a batch request operation, utilizing private member <see cref="ODataBatchPayloadUriConverter"/>.
        /// </summary>
        /// <param name="uri">The uri to process.</param>
        /// <param name="baseUri">The base Uri to use.</param>
        /// <returns>An URI to be used in the request line of a batch request operation. </returns>
        protected Uri CreateOperationRequestUriWrapper(Uri uri, Uri baseUri)
        {
            return ODataBatchUtils.CreateOperationRequestUri(uri, baseUri, this.payloadUriConverter);
        }

        /// <summary>
        /// Add the content id to the <see cref="ODataBatchPayloadUriConverter"/>.
        /// </summary>
        /// <param name="contentId">The (non-null) content ID to add.</param>
        protected void AddToPayloadUriConverter(string contentId)
        {
            this.payloadUriConverter.AddContentId(contentId);
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
        protected void RememberContentIdHeader(string contentId)
        {
            // The Content-ID header is only supported in request messages and inside of changeSets.
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");

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
        /// Wrapper method to validate state transition with optional customized validation.
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        /// <param name="customizedValidationAction">Optional validation action.</param>
        private void ValidateTransition(BatchWriterState newState, Action customizedValidationAction)
        {
            customizedValidationAction();
            CommonTransitionValidation(newState);
        }

        /// <summary>
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseBatchSize()
        {
            this.currentBatchSize++;

            if (this.currentBatchSize > this.outputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxBatchSizeExceeded(this.outputContext.MessageWriterSettings.MessageQuotas.MaxPartsPerBatch));
            }
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseChangeSetSize()
        {
            this.currentChangeSetSize++;

            if (this.currentChangeSetSize > this.outputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(this.outputContext.MessageWriterSettings.MessageQuotas.MaxOperationsPerChangeset));
            }
        }

        /// <summary>
        /// Resets the size of the current change set to 0.
        /// </summary>
        private void ResetChangeSetSize()
        {
            this.currentChangeSetSize = 0;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        private void CanCreateOperationRequestMessageVerifierCommon(bool synchronousCall, string method, Uri uri, string contentId)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.outputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }

            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(method, "method");

            ExceptionUtils.CheckArgumentNotNull(uri, "uri");
        }

        /// <summary>
        /// Verifies that a call is allowed to the writer.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.outputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataBatchWriter_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.outputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataBatchWriter_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        private void VerifyCanCreateOperationRequestMessage(bool synchronousCall, string method, Uri uri,
            string contentId)
        {
            this.CanCreateOperationRequestMessageVerifierCommon(synchronousCall, method, uri, contentId);
            this.VerifyCanCreateOperationRequestMessageImplementation(method, uri, contentId);
        }

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanFlush(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
            if (this.state == BatchWriterState.OperationStreamRequested)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// Validates that the batch writer is ready to process a new write request.
        /// </summary>
        private void ValidateWriterReady()
        {
            VerifyNotDisposed();

            // If the operation stream was requested but not yet disposed, the writer can't be used to do anything.
            if (this.state == BatchWriterState.OperationStreamRequested)
            {
                throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
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
        /// Verifies that calling WriteEndBatch is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEndBatch(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
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
        /// Verifies that calling WriteEndChangeset is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEndChangeset(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Verifies that calling CreateOperationResponseMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.outputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// Verifies that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Validating the transition in the state machine should stay in a single method.")]
        private void CommonTransitionValidation(BatchWriterState newState)
        {
            if (!IsErrorState(this.state) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
                return;
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
                    if (newState != BatchWriterState.ChangesetStarted && newState != BatchWriterState.OperationCreated && newState != BatchWriterState.BatchCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted);
                    }

                    break;
                case BatchWriterState.ChangesetStarted:
                    if (newState != BatchWriterState.OperationCreated && newState != BatchWriterState.ChangesetCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted);
                    }

                    break;
                case BatchWriterState.OperationCreated:
                    if (newState != BatchWriterState.OperationCreated &&
                        newState != BatchWriterState.OperationStreamRequested &&
                        newState != BatchWriterState.ChangesetStarted &&
                        newState != BatchWriterState.ChangesetCompleted &&
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
                        newState != BatchWriterState.ChangesetStarted &&
                        newState != BatchWriterState.ChangesetCompleted &&
                        newState != BatchWriterState.BatchCompleted)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
                    }

                    break;
                case BatchWriterState.ChangesetCompleted:
                    if (newState != BatchWriterState.BatchCompleted &&
                        newState != BatchWriterState.ChangesetStarted &&
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
    }
}
