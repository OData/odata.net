//---------------------------------------------------------------------
// <copyright file="ODataBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Abstract class for writing OData batch messages; also verifies the proper sequence of write calls on the writer.
    /// </summary>
    public abstract class ODataBatchWriter : IODataBatchOperationListener, IODataOutputInStreamErrorListener
    {
        /// <summary>The batch-specific URL resolver that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        internal readonly ODataBatchUrlResolver UrlResolver;

        /// <summary>The output context to write to.</summary>
        private ODataOutputContext outputContext;

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
            Debug.Assert(outputContext != null, "rawOutputContext != null");
            Debug.Assert(
                outputContext.MessageWriterSettings.PayloadBaseUri == null || outputContext.MessageWriterSettings.PayloadBaseUri.IsAbsoluteUri,
                "We should have validated that PayloadBaseUri is absolute.");


            this.outputContext = outputContext;
            this.UrlResolver = new ODataBatchUrlResolver(outputContext.UrlResolver);
        }

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        protected internal enum BatchWriterState
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

        /// <summary>The message for the operation that is currently written; or null if no operation is written right now.</summary>
        internal ODataBatchOperationMessage CurrentOperationMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                if (this.currentOperationRequestMessage != null)
                {
                    Debug.Assert(!this.outputContext.WritingResponse, "Request message can only be set when writing request.");
                    return this.currentOperationRequestMessage.OperationMessage;
                }
                else if (this.currentOperationResponseMessage != null)
                {
                    Debug.Assert(this.outputContext.WritingResponse, "Response message can only be set when writing response.");
                    return this.currentOperationResponseMessage.OperationMessage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the content Id of the current operation.
        /// </summary>
        protected string CurrentOperationContentId
        {
            get { return this.currentOperationContentId; }
            set { this.currentOperationContentId = value; }
        }

        /// <summary>
        /// Gets or Sets the batch writer's state.
        /// </summary>
        protected BatchWriterState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        /// <summary>
        /// Gets the writer's output context.
        /// </summary>
        protected ODataOutputContext OutputContext
        {
            get { return this.outputContext; }
        }

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no operation is written right now or it's a response operation.</summary>
        protected ODataBatchOperationRequestMessage CurrentOperationRequestMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || !this.outputContext.WritingResponse, "Request message can only be filled when writing request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationRequestMessage;
            }

            set
            {
                Debug.Assert(value == null || !this.outputContext.WritingResponse, "Can only set the request message if we're writing a request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
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
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationResponseMessage;
            }

            set
            {
                Debug.Assert(value == null || this.outputContext.WritingResponse, "Can only set the response message if we're writing a response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                this.currentOperationResponseMessage = value;
            }
        }

        /// <summary>Starts a new batch; can be only called once and as first call.</summary>
        public void WriteStartBatch()
        {
            this.VerifyCanWriteStartBatch(true);
            this.WriteStartBatchImplementation();
        }

#if ODATALIB_ASYNC
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

#if ODATALIB_ASYNC
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

#if ODATALIB_ASYNC
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

#if ODATALIB_ASYNC
        /// <summary>Asynchronously ends an active change set; this can only be called after WriteStartChangeset and only once for each change set.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndChangesetAsync()
        {
            this.VerifyCanWriteEndChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndChangesetImplementation);
        }
#endif

        /// <summary>Creates an <see cref="T:Microsoft.OData.Core.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to write the request operation.</returns>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId)
        {
            this.VerifyCanCreateOperationRequestMessage(true, method, uri, contentId);
            return this.CreateOperationRequestMessageImplementation(method, uri, contentId);
        }

#if ODATALIB_ASYNC
        /// <summary>Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head, would be ignored if <paramref name="method"/> is "GET".</param>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId)
        {
            this.VerifyCanCreateOperationRequestMessage(false, method, uri, contentId);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                () => this.CreateOperationRequestMessageImplementation(method, uri, contentId));
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

#if ODATALIB_ASYNC
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.OData.Core.ODataBatchOperationResponseMessage" /> for writing an operation of a batch response.</summary>
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
        public abstract void Flush();

#if ODATALIB_ASYNC
        /// <summary>Flushes the write buffer to the underlying stream asynchronously.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public abstract void BatchOperationContentStreamRequested();

#if ODATALIB_ASYNC
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
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        internal void CanCreateOperationRequestMessageVerifierCommon(bool synchronousCall, string method, Uri uri, string contentId)
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
        internal void VerifyCallAllowed(bool synchronousCall)
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
#if ODATALIB_ASYNC
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
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then rethrow the exception.
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
        /// Wrapper method to validate state transition with optional customized validation.
        /// </summary>
        /// <param name="newState">Teh new writer state to transition into.</param>
        /// <param name="customizedValidationAction">Optional validation action.</param>
        internal void ValidateTransition(BatchWriterState newState, Action customizedValidationAction)
        {
            if (customizedValidationAction != null)
            {
                customizedValidationAction();
            }

            CommonTransitionValidation(newState);
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
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        internal void IncreaseBatchSize()
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
        internal void IncreaseChangeSetSize()
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
        internal void ResetChangeSetSize()
        {
            this.currentChangeSetSize = 0;
        }

        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        protected abstract void WriteStartChangesetImplementation();

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method,
            Uri uri, string contentId);

        /// <summary>
        /// Ends a batch.
        /// </summary>
        protected abstract void WriteEndBatchImplementation();

        /// <summary>
        /// Ends an active changeset.
        /// </summary>
        protected abstract void WriteEndChangesetImplementation();

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        protected abstract void VerifyCanCreateOperationRequestMessage(bool synchronousCall, string method, Uri uri,
            string contentId);

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        protected abstract ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(
            string contentId);

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        protected abstract void StartBatchOperationContent();

        /// <summary>
        /// Disposes the batch writer and set the 'OperationStreamRequested' batch writer state;
        /// called after the flush operation(s) have completed.
        /// </summary>
        protected abstract void DisposeBatchWriterAndSetContentStreamRequestedState();

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        protected abstract void VerifyCanFlush(bool synchronousCall);

        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected abstract void SetState(BatchWriterState newState);

        /// <summary>
        /// Validates that the batch writer is ready to process a new write request.
        /// </summary>
        protected abstract void ValidateWriterReady();

        /// <summary>
        /// Write any pending data for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        protected abstract void WritePendingMessageData(bool reportMessageCompleted);

        /// <summary>
        /// Writes the start boundary for an operation. This is either the batch or the changeset boundary.
        /// </summary>
        protected abstract void WriteStartBoundaryForOperation();

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
        /// Starts a new batch.
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
