//---------------------------------------------------------------------
// <copyright file="ODataBatchReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
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

        /// <summary>The value of the content ID header of the current part.</summary>
        /// <remarks>
        /// The content ID header of the current part should only be visible to subsequent parts
        /// so we can only add it to the URL resolver once we are done with the current part.
        /// </remarks>
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
        protected enum OperationState
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

            protected set
            {
                this.batchReaderState = value;
            }
        }

        /// <summary>
        /// >The batch-specific URL converter that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.
        /// </summary>
        internal ODataBatchPayloadUriConverter PayloadUriConverter
        {
            get { return this.payloadUriConverter; }
        }

        /// <summary>
        /// Reader's input context.
        /// </summary>
        protected ODataInputContext InputContext
        {
            get { return this.inputContext; }
        }

        /// <summary>
        /// Previously cache contentId that should be applied to the next message read.
        /// </summary>
        protected string ContentIdToAddOnNextRead
        {
            get { return this.contentIdToAddOnNextRead; }
            set { this.contentIdToAddOnNextRead = value; }
        }

        /// <summary>
        /// The reader's Operation state
        /// </summary>
        protected OperationState ReaderOperationState
        {
            get { return this.operationState; }
            set { this.operationState = value; }
        }

        /// <summary>
        /// The dependency injection container to get related services.
        /// </summary>
        protected IServiceProvider Container
        {
            get { return this.container; }
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
            return this.InterceptException((Func<ODataBatchOperationRequestMessage>)this.CreateOperationRequestMessageImplementation);
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a request message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync()
        {
            this.VerifyCanCreateOperationRequestMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                this.CreateOperationRequestMessageImplementation)
                .FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>Returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A response message for reading the content of a batch operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ true);
            return this.InterceptException((Func<ODataBatchOperationResponseMessage>)this.CreateOperationResponseMessageImplementation);
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a response message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(
                this.CreateOperationResponseMessageImplementation)
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
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        protected void IncreaseBatchSize()
        {
            this.currentBatchSize++;

            if (this.currentBatchSize > this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxBatchSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch));
            }
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        protected void IncreaseChangesetSize()
        {
            this.currentChangeSetSize++;

            if (this.currentChangeSetSize > this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxChangeSetSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset));
            }
        }

        /// <summary>
        /// Resets the size of the current change set to 0.
        /// </summary>
        protected void ResetChangesetSize()
        {
            this.currentChangeSetSize = 0;
        }

        /// <summary>
        /// Continues reading from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected abstract bool ReadImplementation();

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
        /// Sets the 'Exception' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.State = ODataBatchReaderState.Exception;
            throw new ODataException(errorMessage);
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
