//---------------------------------------------------------------------
// <copyright file="ODataBatchReader.cs" company="Microsoft">
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
    using System.IO;

#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Abstract class for reading OData batch messages; also verifies the proper sequence of read calls on the reader.
    /// </summary>
    public abstract class ODataBatchReader : IODataBatchOperationListener
    {
        /// <summary>The input context to read the content from.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>The batch-specific URL converter that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        private readonly ODataBatchPayloadUriConverter payloadUriConverter;

        /// <summary>The dependency injection container to get related services.</summary>
        private readonly IServiceProvider container;

        /// <summary>The current state of the batch reader.</summary>
        private ODataBatchReaderState batchReaderState;

        /// <summary>The current size of the batch message, i.e., how many query operations and changesets have been read.</summary>
        private uint currentBatchSize;

        /// <summary>The current size of the active changeset, i.e., how many operations have been read for the changeset.</summary>
        private uint currentChangeSetSize;

        /// <summary>An enumeration tracking the state of the current batch operation.</summary>
        private OperationState operationState;

        /// <summary>Whether the reader is currently reading within a changeset.</summary>
        private bool isInChangeset;

        /// <summary>
        /// Content-ID header value for request part with associated entity, which can be referenced by subsequent requests
        /// in the same changeset or other changesets.
        /// </summary>
        private string contentIdToAddOnNextRead;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        protected ODataBatchReader(ODataInputContext inputContext, bool synchronous)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.inputContext = inputContext;
            this.container = inputContext.Container;
            this.synchronous = synchronous;
            this.payloadUriConverter = new ODataBatchPayloadUriConverter(inputContext.PayloadUriConverter);
        }

        /// <summary>
        /// An enumeration to track the state of a batch operation.
        /// </summary>
        private enum OperationState
        {
            /// <summary>No action has been performed on the operation.</summary>
            None,

            /// <summary>The batch message for the operation has been created and returned to the caller.</summary>
            MessageCreated,

            /// <summary>The stream of the batch operation message has been requested.</summary>
            StreamRequested,

            /// <summary>The stream of the batch operation message has been disposed.</summary>
            StreamDisposed,
        }

        /// <summary>Gets the current state of the batch reader.</summary>
        /// <returns>The current state of the batch reader.</returns>
        public ODataBatchReaderState State
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                return this.batchReaderState;
            }

            private set
            {
                this.batchReaderState = value;
            }
        }

        /// <summary>
        /// Public property for the current group id the reader is processing.
        /// The primary usage of this to correlate atomic group id in request and
        /// response operation messages as needed.
        /// </summary>
        public string CurrentGroupId
        {
            get
            {
                return GetCurrentGroupIdImplementation();
            }
        }

        /// <summary>
        /// The input context to read the content from.
        /// </summary>
        protected ODataInputContext InputContext
        {
            get { return this.inputContext; }
        }

        /// <summary>
        /// The reader's Operation state
        /// </summary>
        private OperationState ReaderOperationState
        {
            get { return this.operationState; }
            set { this.operationState = value; }
        }

        /// <summary> Reads the next part from the batch message payload. </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException((Func<bool>)this.ReadSynchronously);
        }

#if PORTABLELIB
        /// <summary>Asynchronously reads the next part from the batch message payload.</summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>Returns an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A request message for reading the content of a batch operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage()
        {
            this.VerifyCanCreateOperationRequestMessage(/*synchronousCall*/ true);
            ODataBatchOperationRequestMessage result =
                this.InterceptException((Func<ODataBatchOperationRequestMessage>)this.CreateOperationRequestMessageImplementation);
            this.ReaderOperationState = OperationState.MessageCreated;
            this.contentIdToAddOnNextRead = result.ContentId;
            return result;
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a request message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync()
        {
            this.VerifyCanCreateOperationRequestMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                this.CreateOperationRequestMessageImplementation)
                .FollowOnSuccessWithTask(
                    t =>
                    {
                        this.ReaderOperationState = OperationState.MessageCreated;
                        this.contentIdToAddOnNextRead = t.Result.ContentId;
                        return t;
                    })
                .FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>Returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A response message for reading the content of a batch operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ true);
            ODataBatchOperationResponseMessage result =
                this.InterceptException((Func<ODataBatchOperationResponseMessage>)this.CreateOperationResponseMessageImplementation);
            this.ReaderOperationState = OperationState.MessageCreated;
            return result;
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a response message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(
                this.CreateOperationResponseMessageImplementation)
                .FollowOnSuccessWithTask(
                    t =>
                    {
                        this.ReaderOperationState = OperationState.MessageCreated;
                        return t;
                    })
                .FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamRequested()
        {
            this.operationState = OperationState.StreamRequested;
        }

#if PORTABLELIB
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the reader;
        /// null if no such action exists.
        /// </returns>
        Task IODataBatchOperationListener.BatchOperationContentStreamRequestedAsync()
        {
            this.operationState = OperationState.StreamRequested;
            return TaskUtils.CompletedTask;
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamDisposed()
        {
            this.operationState = OperationState.StreamDisposed;
        }

        /// <summary>
        /// Sets the 'Exception' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        protected void ThrowODataException(string errorMessage)
        {
            this.State = ODataBatchReaderState.Exception;
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Gets the group id for the current request.
        /// Default implementation here is provided returning null.
        /// </summary>
        /// <returns>The group id for the current request.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        protected virtual string GetCurrentGroupIdImplementation()
        {
            return null;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation();

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        protected abstract ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected abstract ODataBatchReaderState ReadAtStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'Operation'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected abstract ODataBatchReaderState ReadAtOperationImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'ChangesetStart'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected abstract ODataBatchReaderState ReadAtChangesetStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'ChangesetEnd'.
        /// </summary>
        /// <returns>The batch reader state after the read.</returns>
        protected abstract ODataBatchReaderState ReadAtChangesetEndImplementation();

        /// <summary>
        /// Instantiate an <see cref="ODataBatchOperationRequestMessage"/> instance.
        /// </summary>
        /// <param name="streamCreatorFunc">The function for stream creation.</param>
        /// <param name="method">The HTTP method used for this request message.</param>
        /// <param name="requestUri">The request Url for this request message.</param>
        /// <param name="headers">The headers for this request message.</param>
        /// <param name="contentId">The contentId of this request message.</param>
        /// <param name="groupId">The group id that this request belongs to. Can be null.</param>
        /// <param name="dependsOnRequestIds">
        /// The prerequisite request Ids of this request message that could be specified by caller
        /// explicitly.
        /// </param>
        /// <param name="dependsOnIdsValidationRequired">
        /// Whether the <code>dependsOnIds</code> value needs to be validated.</param>
        /// <returns>The <see cref="ODataBatchOperationRequestMessage"/> instance.</returns>
        protected ODataBatchOperationRequestMessage BuildOperationRequestMessage(
            Func<Stream> streamCreatorFunc,
            string method,
            Uri requestUri,
            ODataBatchOperationHeaders headers,
            string contentId,
            string groupId,
            IEnumerable<string> dependsOnRequestIds,
            bool dependsOnIdsValidationRequired)
        {
            if (dependsOnRequestIds != null && dependsOnIdsValidationRequired)
            {
                foreach (string id in dependsOnRequestIds)
                {
                    if (!this.payloadUriConverter.ContainsContentId(id))
                    {
                        throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound(id, contentId));
                    }
                }
            }

            Uri uri = ODataBatchUtils.CreateOperationRequestUri(
                requestUri, this.inputContext.MessageReaderSettings.BaseUri, this.payloadUriConverter);

            ODataBatchUtils.ValidateReferenceUri(requestUri, dependsOnRequestIds,
                this.inputContext.MessageReaderSettings.BaseUri);

            return new ODataBatchOperationRequestMessage(streamCreatorFunc, method, uri, headers, this,
                contentId, this.payloadUriConverter, /*writing*/ false, this.container, dependsOnRequestIds, groupId);
        }

        /// <summary>
        /// Instantiate an <see cref="ODataBatchOperationResponseMessage"/> instance and set the status code.
        /// </summary>
        /// <param name="streamCreatorFunc">The function for stream creation.</param>
        /// <param name="statusCode">The status code for the response.</param>
        /// <param name="headers">The headers for this response message.</param>
        /// <param name="contentId">The contentId of this request message.</param>
        /// <param name="groupId">The groupId of this request message.</param>
        /// <returns>The <see cref="ODataBatchOperationResponseMessage"/> instance.</returns>
        protected ODataBatchOperationResponseMessage BuildOperationResponseMessage(
            Func<Stream> streamCreatorFunc,
            int statusCode,
            ODataBatchOperationHeaders headers,
            string contentId,
            string groupId)
        {
            ODataBatchOperationResponseMessage responseMessage = new ODataBatchOperationResponseMessage(
                streamCreatorFunc, headers, this,
                contentId,
                this.payloadUriConverter.BatchMessagePayloadUriConverter, /*writing*/ false, this.container, groupId)
            {
                StatusCode = statusCode
            };
            return responseMessage;
        }

        /// <summary>
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseBatchSize()
        {
            if (this.currentBatchSize == this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxBatchSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch));
            }

            this.currentBatchSize++;
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseChangesetSize()
        {
            if (this.currentChangeSetSize == this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxChangeSetSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset));
            }

            this.currentChangeSetSize++;
        }

        /// <summary>
        /// Resets the size of the current change set to 0.
        /// </summary>
        private void ResetChangesetSize()
        {
            this.currentChangeSetSize = 0;
        }

        /// <summary>
        /// Reads the next part from the batch message payload.
        /// </summary>
        /// <returns>true if more information was read; otherwise false.</returns>
        private bool ReadSynchronously()
        {
            return this.ReadImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously reads the next part from the batch message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more information was read.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        private Task<bool> ReadAsynchronously()
        {
            // We are reading from the fully buffered read stream here; thus it is ok
            // to use synchronous reads and then return a completed task
            // NOTE: once we switch to fully async reading this will have to change
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadImplementation);
        }
#endif

        /// <summary>
        /// Continues reading from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        private bool ReadImplementation()
        {
            Debug.Assert(this.ReaderOperationState != OperationState.StreamRequested, "Should have verified that no operation stream is still active.");

            switch (this.State)
            {
                case ODataBatchReaderState.Initial:
                    // The stream should be positioned at the beginning of the batch content,
                    // that is before the first boundary (or the preamble if there is one).
                    this.State = this.ReadAtStartImplementation();
                    break;

                case ODataBatchReaderState.Operation:
                    // When reaching this state we already read the MIME headers of the operation.
                    // Clients MUST call CreateOperationRequestMessage
                    // or CreateOperationResponseMessage to read at least the headers of the operation.
                    // This is important since we need to read the ContentId header (if present) and
                    // add it to the URL resolver.
                    if (this.ReaderOperationState == OperationState.None)
                    {
                        // No message was created; fail
                        throw new ODataException(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
                    }

                    // Reset the operation state; the operation state only
                    // tracks the state of a batch operation while in state Operation.
                    this.ReaderOperationState = OperationState.None;

                    // Also add a pending ContentId header to the URL resolver now. We ensured above
                    // that a message has been created for this operation and thus the headers (incl.
                    // a potential content ID header) have been read.
                    if (this.contentIdToAddOnNextRead != null)
                    {
                        if (this.payloadUriConverter.ContainsContentId(this.contentIdToAddOnNextRead))
                        {
                            throw new ODataException(
                                Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(this.contentIdToAddOnNextRead));
                        }

                        this.payloadUriConverter.AddContentId(this.contentIdToAddOnNextRead);
                        this.contentIdToAddOnNextRead = null;
                    }

                    // When we are done with an operation, we have to skip ahead to the next part
                    // when Read is called again. Note that if the operation stream was never requested
                    // and the content of the operation has not been read, we'll skip it here.
                    Debug.Assert(this.ReaderOperationState == OperationState.None, "Operation state must be 'None' at the end of the operation.");

                    if (this.isInChangeset)
                    {
                        this.IncreaseChangesetSize();
                    }
                    else
                    {
                        this.IncreaseBatchSize();
                    }

                    this.State = this.ReadAtOperationImplementation();

                    break;

                case ODataBatchReaderState.ChangesetStart:
                    // When at the start of a changeset, skip ahead to the first operation in the
                    // changeset (or the end boundary of the changeset).
                    if (this.isInChangeset)
                    {
                        ThrowODataException(Strings.ODataBatchReaderStream_NestedChangesetsAreNotSupported);
                    }

                    // Increment the batch size at the start of the changeset since we haven't counted it yet
                    // when this state was transitioned into upon detection of this sub-batch.
                    this.IncreaseBatchSize();

                    this.State = this.ReadAtChangesetStartImplementation();
                    if (this.inputContext.MessageReaderSettings.MaxProtocolVersion <= ODataVersion.V4)
                    {
                        this.payloadUriConverter.Reset();
                    }

                    this.isInChangeset = true;
                    break;

                case ODataBatchReaderState.ChangesetEnd:
                    // When at the end of a changeset, reset the changeset boundary and the
                    // changeset size and then skip to the next part.
                    this.ResetChangesetSize();
                    this.isInChangeset = false;
                    this.State = this.ReadAtChangesetEndImplementation();
                    break;

                case ODataBatchReaderState.Exception:    // fall through
                case ODataBatchReaderState.Completed:
                    Debug.Assert(false, "Should have checked in VerifyCanRead that we are not in one of these states.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));
            }

            return this.State != ODataBatchReaderState.Completed && this.State != ODataBatchReaderState.Exception;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationRequestMessage(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.inputContext.ReadingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse);
            }

            if (this.State != ODataBatchReaderState.Operation)
            {
                this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationRequestMessage(this.State));
            }

            if (this.operationState != OperationState.None)
            {
                this.ThrowODataException(Strings.ODataBatchReader_OperationRequestMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// Verifies that calling CreateOperationResponseMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.inputContext.ReadingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest);
            }

            if (this.State != ODataBatchReaderState.Operation)
            {
                this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationResponseMessage(this.State));
            }

            if (this.operationState != OperationState.None)
            {
                this.ThrowODataException(Strings.ODataBatchReader_OperationResponseMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// Verifies that calling Read is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanRead(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State == ODataBatchReaderState.Exception || this.State == ODataBatchReaderState.Completed)
            {
                throw new ODataException(Strings.ODataBatchReader_ReadOrReadAsyncCalledInInvalidState(this.State));
            }
        }

        /// <summary>
        /// Validates that the batch reader is ready to process a new read or create message request.
        /// </summary>
        private void VerifyReaderReady()
        {
            this.inputContext.VerifyNotDisposed();

            // If the operation stream was requested but not yet disposed, the batch reader can't be used to do anything.
            if (this.operationState == OperationState.StreamRequested)
            {
                throw new ODataException(Strings.ODataBatchReader_CannotUseReaderWhileOperationStreamActive);
            }
        }

        /// <summary>
        /// Verifies that a call is allowed to the reader.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchReader_SyncCallOnAsyncReader);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchReader_AsyncCallOnSyncReader);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Exception and then rethrow the exception.
        /// </summary>
        /// <typeparam name="T">The type of the result returned from the <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the <paramref name="action"/>.</returns>
        private T InterceptException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.State = ODataBatchReaderState.Exception;
                }

                throw;
            }
        }
    }
}
