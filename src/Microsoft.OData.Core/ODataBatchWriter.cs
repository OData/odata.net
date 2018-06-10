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
    using System.IO;
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
        /// Whether the writer is currently processing inside a changeset or atomic group.
        /// </summary>
        private bool isInChangset;

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

        /// <summary>
        /// The output context to write to.
        /// </summary>
        protected ODataOutputContext OutputContext
        {
            get { return this.outputContext; }
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

        /// <summary>
        /// Starts a new changeset without specifying group id.
        /// This can only be called after WriteStartBatch and if no other active operation or changeset exists.
        /// </summary>
        public void WriteStartChangeset()
        {
            this.WriteStartChangeset(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Starts a new atomic group or changeset with the specified group id or changeset GUID corresponding to change set boundary.
        /// This can only be called after WriteStartBatch and if no other active operation or changeset exists.</summary>
        /// <param name="changesetId"> The change set Id of the batch request. Cannot be null.</param>
        /// <exception cref="ODataException">Thrown if the <paramref name="changesetId"/> is null.</exception>
        public void WriteStartChangeset(string changesetId)
        {
            ExceptionUtils.CheckArgumentNotNull(changesetId, "changesetId");

            this.VerifyCanWriteStartChangeset(true);
            this.WriteStartChangesetImplementation(changesetId);
            this.FinishWriteStartChangeset();
        }

#if PORTABLELIB
        /// <summary>Asynchronously starts a new change set without specifying group id;
        /// This can only be called after WriteStartBatch and if no other active operation or change set exists.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartChangesetAsync()
        {
            return WriteStartChangesetAsync(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Asynchronously starts a new change set; can only be called after WriteStartBatch and if no other active operation or change set exists.
        /// </summary>
        /// <param name="changesetId"> The change set Id of the batch request. Cannot be null.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <exception cref="ODataException">Thrown if the <paramref name="changesetId"/> is null.</exception>
        public Task WriteStartChangesetAsync(string changesetId)
        {
            ExceptionUtils.CheckArgumentNotNull(changesetId, "changesetId");

            this.VerifyCanWriteStartChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartChangesetImplementation(changesetId))
                .FollowOnSuccessWith(t => this.FinishWriteStartChangeset());
        }
#endif

        /// <summary>Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.</summary>
        public void WriteEndChangeset()
        {
            this.VerifyCanWriteEndChangeset(true);
            this.WriteEndChangesetImplementation();
            FinishWriteEndChangeset();
        }

#if PORTABLELIB
        /// <summary>Asynchronously ends an active change set; this can only be called after WriteStartChangeset and only once for each change set.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndChangesetAsync()
        {
            this.VerifyCanWriteEndChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndChangesetImplementation)
                .FollowOnSuccessWith(t => this.FinishWriteEndChangeset());
        }
#endif

        /// <summary>Creates an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.</summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId)
        {
            return CreateOperationRequestMessage(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
        }

        /// <summary>Creates an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.</summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId,
            BatchPayloadUriOption payloadUriOption)
        {
            return CreateOperationRequestMessage(method, uri, contentId, payloadUriOption, /*dependsOnIds*/null);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">
        /// The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(string method, Uri uri, string contentId,
            BatchPayloadUriOption payloadUriOption, IEnumerable<string> dependsOnIds)
        {
            this.VerifyCanCreateOperationRequestMessage(true, method, uri, contentId);
            return CreateOperationRequestMessageInternal(method, uri, contentId, payloadUriOption, dependsOnIds);
        }

#if PORTABLELIB
        /// <summary>Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">
        /// The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <returns>A task that when completed returns the newly created operation request message.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId)
        {
            return CreateOperationRequestMessageAsync(method, uri, contentId, BatchPayloadUriOption.AbsoluteUri);
        }

        /// <summary>Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">
        /// The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>A task that when completed returns the newly created operation request message.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId,
            BatchPayloadUriOption payloadUriOption)
        {
            return CreateOperationRequestMessageAsync(method, uri, contentId, payloadUriOption, /*dependsOnIds*/null);
        }

        /// <summary>
        /// Creates a message for asynchronously writing an operation of a batch request.</summary>
        /// <returns>The message that can be used to asynchronously write the request operation.</returns>
        /// <param name="method">The HTTP method to be used for this request operation.</param>
        /// <param name="uri">The URI to be used for this request operation.</param>
        /// <param name="contentId">
        /// The Content-ID value to write in ChangeSet header, would be ignored if <paramref name="method"/> is "GET".</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request.</param>
        /// <returns>A task that when completed returns the newly created operation request message.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(string method, Uri uri, string contentId,
            BatchPayloadUriOption payloadUriOption, IList<string> dependsOnIds)
        {
            this.VerifyCanCreateOperationRequestMessage(false, method, uri, contentId);

            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(() =>
                CreateOperationRequestMessageInternal(method, uri, contentId, payloadUriOption, dependsOnIds));
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
        /// <param name="groupOrChangesetId">
        /// The atomic group id, aka changeset GUID of the batch request.
        /// </param>
        protected abstract void WriteStartChangesetImplementation(string groupOrChangesetId);

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
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected abstract ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method, Uri uri,
            string contentId, BatchPayloadUriOption payloadUriOption, IEnumerable<string> dependsOnIds);


        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected void SetState(BatchWriterState newState)
        {
            this.InterceptException(() => this.ValidateTransition(newState));

            this.state = newState;
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
        /// Given an enumerable of dependsOn ids containing request ids and group ids, return an enumeration
        /// of equivalent request ids.
        /// </summary>
        /// <param name="dependsOnIds">The dependsOn ids specifying current request's prerequisites.</param>
        /// <returns>An enumerable consists of request ids.</returns>
        protected abstract IEnumerable<string> GetDependsOnRequestIds(IEnumerable<string> dependsOnIds);

        /// <summary>
        /// Wrapper method to create an operation request message that can be used to write the operation content to, utilizing
        /// private members <see cref="ODataBatchPayloadUriConverter"/> and <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <param name="method">The HTTP method to use for the message to create.</param>
        /// <param name="uri">The request URL for the message to create.</param>
        /// <param name="contentId">The contentId of this request message.</param>
        /// <param name="groupId">The group id that this request belongs to. Can be null.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request.</param>
        /// <returns>An <see cref="ODataBatchOperationRequestMessage"/> to write the request content to.</returns>
        protected ODataBatchOperationRequestMessage BuildOperationRequestMessage(Stream outputStream, string method, Uri uri,
            string contentId, string groupId, IEnumerable<string> dependsOnIds)
        {
            IEnumerable<string> convertedDependsOnIds = GetDependsOnRequestIds(dependsOnIds);
            Debug.Assert(convertedDependsOnIds != null, "convertedDependsOnIds != null");

            if (dependsOnIds != null)
            {
                // Validate explicit dependsOnIds cases.
                foreach (string id in convertedDependsOnIds)
                {
                    if (!this.payloadUriConverter.ContainsContentId(id))
                    {
                        throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound(id, contentId));
                    }
                }
            }

            // If dependsOnIds is not specified, use the <code>payloadUrlConverter</code>; otherwise use the dependOnIds converted
            // from specified value.
            IEnumerable<string> requestIdsForUrlReferenceValidation =
                dependsOnIds == null ? this.payloadUriConverter.ContentIdCache : convertedDependsOnIds;

            ODataBatchUtils.ValidateReferenceUri(uri, requestIdsForUrlReferenceValidation, this.outputContext.MessageWriterSettings.BaseUri);

            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, this);
            ODataBatchOperationRequestMessage requestMessage =
                new ODataBatchOperationRequestMessage(streamCreatorFunc, method, uri, /*headers*/ null, this, contentId,
                this.payloadUriConverter, /*writing*/ true, this.container, dependsOnIds, groupId);

            return requestMessage;
        }

        /// <summary>
        /// Wrapper method to create an operation response message that can be used to write the operation content to, utilizing
        /// private members <see cref="ODataBatchPayloadUriConverter"/> and <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <param name="contentId">The contentId of this response message.</param>
        /// <param name="groupId">The group id of the response message, should be the same as the group id
        /// in the corresponding request message.</param>
        /// <returns>An <see cref="ODataBatchOperationResponseMessage"/> that can be used to write the operation content.</returns>
        protected ODataBatchOperationResponseMessage BuildOperationResponseMessage(Stream outputStream,
            string contentId, string groupId)
        {
            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, this);
            return new ODataBatchOperationResponseMessage(streamCreatorFunc, /*headers*/ null, this, contentId,
                this.payloadUriConverter.BatchMessagePayloadUriConverter, /*writing*/ true, this.container, groupId);
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then re-throw the exception.
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
        /// Sets the 'Error' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.SetState(BatchWriterState.Error);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Internal method to create an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for writing
        /// an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">
        /// For batch in multipart format, the Content-ID value to write in ChangeSet header, would be ignored if
        /// <paramref name="method"/> is "GET".
        /// For batch in Json format, if the value passed in is null, an GUID will be generated and used as the request id.
        /// </param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        private ODataBatchOperationRequestMessage CreateOperationRequestMessageInternal(string method, Uri uri, string contentId,
            BatchPayloadUriOption payloadUriOption, IEnumerable<string> dependsOnIds)
        {
            if (!this.isInChangset)
            {
                this.InterceptException(this.IncreaseBatchSize);
            }
            else
            {
                this.InterceptException(this.IncreaseChangeSetSize);
            }

            // Add a potential Content-ID header to the URL resolver so that it will be available
            // to subsequent operations.
            // Note that what we add here is the Content-ID header of the previous operation (if any).
            // This also means that the Content-ID of the last operation in a changeset will never get
            // added to the cache which is fine since we cannot reference it anywhere.
            if (this.currentOperationContentId != null)
            {
                this.payloadUriConverter.AddContentId(this.currentOperationContentId);
            }

            this.InterceptException(() =>
                uri = ODataBatchUtils.CreateOperationRequestUri(uri, this.outputContext.MessageWriterSettings.BaseUri, this.payloadUriConverter));

            this.CurrentOperationRequestMessage = this.CreateOperationRequestMessageImplementation(
                method, uri, contentId, payloadUriOption, dependsOnIds);

            if (this.isInChangset || this.outputContext.MessageWriterSettings.Version > ODataVersion.V4)
            {
                // The content Id can be generated if the value passed in is null, therefore here we get the real value of the content Id.
                this.RememberContentIdHeader(this.CurrentOperationRequestMessage.ContentId);
            }

            return this.CurrentOperationRequestMessage;
        }

        /// <summary>
        /// Perform updates after changeset is started.
        /// </summary>
        private void FinishWriteStartChangeset()
        {
            // Reset the cache of content IDs here. As per spec,
            // For version up to V4 (inclusive): Content-IDs uniqueness scope is whole batch,
            // but existing implementation is change set. We don't want to introduce breaking change, and shouldn't
            // throw error for existing scenario of having same content Id in different changesets.
            // For version above V4: Content-IDs uniqueness scope is whole batch.
            if (this.outputContext.MessageWriterSettings.Version <= ODataVersion.V4)
            {
                this.payloadUriConverter.Reset();
            }

            // reset the size of the current changeset and increase the size of the batch.
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);
            this.isInChangset = true;
        }

        /// <summary>
        /// Perform updates after changeset is ended.
        /// </summary>
        private void FinishWriteEndChangeset()
        {
            // When change set ends, only reset content Id for V4 (and below);
            // We need to carry on the content Id for >V4 to ensure uniqueness (and therefore referable).
            if (this.outputContext.MessageWriterSettings.Version <= ODataVersion.V4)
            {
                this.currentOperationContentId = null;
            }

            this.isInChangset = false;
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
            // The Content-ID header is only supported in request messages and inside of changeSets.
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.payloadUriConverter.ContainsContentId(contentId))
            {
                throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(contentId));
            }

            // Set the current content ID. If no Content-ID header is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.currentOperationContentId = contentId;
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
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(method, "method");

            ExceptionUtils.CheckArgumentNotNull(uri, "uri");

            // For the case within a changeset, verify CreateOperationRequestMessage is valid.
            if (this.isInChangset)
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

            // Common verification.
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.outputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }
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
            Debug.Assert(this.currentOperationContentId == null, "The Content-ID header is only supported in request messages.");

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
        private void ValidateTransition(BatchWriterState newState)
        {
            if (!IsErrorState(this.state) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
                return;
            }

            switch (newState)
            {
                case BatchWriterState.ChangesetStarted:
                    // make sure that we are not starting a changeset when one is already active
                    if (this.isInChangset)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
                    }

                    break;
                case BatchWriterState.ChangesetCompleted:
                    // make sure that we are not completing a changeset without an active changeset
                    if (!this.isInChangset)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
                    }

                    break;
                case BatchWriterState.BatchCompleted:
                    // make sure that we are not completing a batch while a changeset is still active
                    if (this.isInChangset)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
                    }

                    break;
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
