//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.MultipartMixed
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;

    #endregion Namespaces

    internal sealed class ODataMultipartMixedBatchWriter : ODataBatchWriter
    {
        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

        /// <summary>
        /// The dependsOnIds tracker for writer processing.
        /// </summary>
        private readonly DependsOnIdsTracker dependsOnIdsTracker;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawOutputContext">The output context to write to.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        internal ODataMultipartMixedBatchWriter(ODataMultipartMixedBatchOutputContext rawOutputContext, string batchBoundary)
            : base(rawOutputContext)
        {
            Debug.Assert(rawOutputContext != null, "rawOutputContext != null");
            ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary is null");
            this.batchBoundary = batchBoundary;
            this.RawOutputContext.InitializeRawValueWriter();
            this.dependsOnIdsTracker = new DependsOnIdsTracker();
        }

        /// <summary>
        /// Gets the writer's output context as the real runtime type.
        /// </summary>
        private ODataMultipartMixedBatchOutputContext RawOutputContext
        {
            get { return this.OutputContext as ODataMultipartMixedBatchOutputContext; }
        }

        /// <summary>
        /// The message for the operation that is currently written; or null if no operation is written right now.
        /// </summary>
        private ODataBatchOperationMessage CurrentOperationMessage
        {
            get
            {
                Debug.Assert(this.CurrentOperationRequestMessage == null || this.CurrentOperationResponseMessage == null,
                    "Only request or response message can be set, not both.");
                if (this.CurrentOperationRequestMessage != null)
                {
                    Debug.Assert(!this.RawOutputContext.WritingResponse, "Request message can only be set when writing request.");
                    return this.CurrentOperationRequestMessage.OperationMessage;
                }
                else if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.RawOutputContext.WritingResponse, "Response message can only be set when writing response.");
                    return this.CurrentOperationResponseMessage.OperationMessage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public override void StreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.RawOutputContext.FlushBuffers();

            // Dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            this.DisposeBatchWriterAndSetContentStreamRequestedState();
        }

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the operation;
        /// null if no such action exists.
        /// </returns>
        public override async Task StreamRequestedAsync()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            await this.StartBatchOperationContentAsync()
                .ConfigureAwait(false);

            // Asynchronously flush the async buffered stream to the underlying message stream (if there's any);
            // then dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            await this.RawOutputContext.FlushBuffersAsync()
                .ConfigureAwait(false);
            this.DisposeBatchWriterAndSetContentStreamRequestedState();
        }

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        public override void StreamDisposed()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");

            this.SetState(BatchWriterState.OperationStreamDisposed);
            this.CurrentOperationRequestMessage = null;
            this.CurrentOperationResponseMessage = null;
            this.RawOutputContext.InitializeRawValueWriter();
        }

        public override Task StreamDisposedAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(
                () => this.StreamDisposed());
        }

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        public override void OnInStreamError()
        {
            this.RawOutputContext.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            this.RawOutputContext.TextWriter.Flush();

            // The OData protocol spec does not define the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }

        public override async Task OnInStreamErrorAsync()
        {
            this.RawOutputContext.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            await this.RawOutputContext.TextWriter.FlushAsync()
                .ConfigureAwait(false);

            // The OData protocol spec does not define the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected override void FlushSynchronously()
        {
            this.RawOutputContext.Flush();
        }

        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected override Task FlushAsynchronously()
        {
            return this.RawOutputContext.FlushAsync();
        }

        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        /// <param name="changeSetId">The value for changeset boundary for multipart batch.</param>
        protected override void WriteStartChangesetImplementation(string changeSetId)
        {
            Debug.Assert(changeSetId != null, "changeSetId != null");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.ChangesetStarted);
            Debug.Assert(this.changeSetBoundary == null, "this.changeSetBoundary == null");
            this.changeSetBoundary = ODataMultipartMixedBatchWriterUtils.CreateChangeSetBoundary(this.RawOutputContext.WritingResponse, changeSetId);

            // write the boundary string
            ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
            this.batchStartBoundaryWritten = true;

            // write the change set headers
            ODataMultipartMixedBatchWriterUtils.WriteChangeSetPreamble(this.RawOutputContext.TextWriter, this.changeSetBoundary);
            this.changesetStartBoundaryWritten = false;

            // Set state to track dependsOn Ids.
            this.dependsOnIdsTracker.ChangeSetStarted();
        }

        /// <summary>
        /// Given an enumerable of dependsOnIds, return an enumeration of equivalent request ids.
        /// </summary>
        /// <param name="dependsOnIds">The dependsOn ids specifying current request's prerequisites.</param>
        /// <returns>If <code>dependsOnIds</code> is null, this is the implicit case therefore returns
        /// an enumerable consists of request id from the <code>dependsOnIdsTracker</code>;
        /// otherwise, this is explicit case therefore returns value passed in directly.</returns>
        protected override IEnumerable<string> GetDependsOnRequestIds(IEnumerable<string> dependsOnIds)
        {
            return dependsOnIds ?? this.dependsOnIdsTracker.GetDependsOnIds();
        }

        /// <summary>
        /// Validate if the dependsOnIds are in the ContentIdCache.
        /// </summary>
        /// <param name="contentId">The content Id.</param>
        /// <param name="dependsOnIds">The dependsOn ids specifying current request's prerequisites.</param>
        protected override void ValidateDependsOnIds(string contentId, IEnumerable<string> dependsOnIds)
        {
            // Validate explicit dependsOnIds cases.
            foreach (string id in dependsOnIds)
            {
                if (!this.payloadUriConverter.ContainsContentId(id))
                {
                    throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound(id, contentId));
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request
        /// - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request. By default its value should be null for Multipart/Mixed
        /// format and the dependsOnIds implicitly derived per the protocol will be used; Otherwise, non-null will be used as override after
        /// validation.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(
            string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption,
            IEnumerable<string> dependsOnIds)
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // create the new request operation
            // For Multipart batch format, validate dependsOnIds if it is user explicit input, otherwise skip validation
            // when it is implicitly derived per protocol.
            ODataBatchOperationRequestMessage operationRequestMessage = BuildOperationRequestMessage(
                this.RawOutputContext.OutputStream,
                method, uri, contentId,
                this.changeSetBoundary,
                dependsOnIds);

            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            if (contentId != null)
            {
                this.dependsOnIdsTracker.AddDependsOnId(contentId);
            }

            // write the headers and request line
            ODataMultipartMixedBatchWriterUtils.WriteRequestPreamble(this.RawOutputContext.TextWriter, method, uri,
                this.RawOutputContext.MessageWriterSettings.BaseUri, changeSetBoundary != null, contentId,
                payloadUriOption);

            return operationRequestMessage;
        }

        /// <summary>
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndBatchImplementation()
        {
            Debug.Assert(
                this.batchStartBoundaryWritten || this.CurrentOperationMessage == null,
                "If not batch boundary was written we must not have an active message.");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // write the end boundary for the batch
            ODataMultipartMixedBatchWriterUtils.WriteEndBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);

            // For compatibility with WCF DS we write a newline after the end batch boundary.
            // Technically it's not needed, but it doesn't violate anything either.
            this.RawOutputContext.TextWriter.WriteLine();
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            string currentChangeSetBoundary = this.changeSetBoundary;

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangesetCompleted);

            this.dependsOnIdsTracker.ChangeSetEnded();

            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");
            this.changeSetBoundary = null;

            // In the case of an empty changeset the start changeset boundary has not been written yet
            // we will leave it like that, since we want the empty changeset to be represented only as
            // the end changeset boundary.
            // Due to WCF DS V2 compatibility we must not write the start boundary in this case
            // otherwise WCF DS V2 won't be able to read it (it fails on the start-end boundary empty changeset).

            // write the end boundary for the change set
            ODataMultipartMixedBatchWriterUtils.WriteEndBoundary(this.RawOutputContext.TextWriter, currentChangeSetBoundary, !this.changesetStartBoundaryWritten);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            this.CurrentOperationResponseMessage = BuildOperationResponseMessage(
                this.RawOutputContext.OutputStream, contentId,
                this.changeSetBoundary);

            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request separator line
            ODataMultipartMixedBatchWriterUtils.WriteResponsePreamble(this.RawOutputContext.TextWriter, changeSetBoundary != null, contentId);

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Verifies that the writer is not disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.RawOutputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Starts a new batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteStartBatchImplementation()
        {
            this.SetState(BatchWriterState.BatchStarted);
        }

        /// <summary>
        /// Asynchronously ends a batch - implementation of the actual functionality.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WriteEndBatchImplementationAsync()
        {
            Debug.Assert(
                this.batchStartBoundaryWritten || this.CurrentOperationMessage == null,
                "If not batch boundary was written we must not have an active message.");

            // Write pending message data (headers, response line) for a previously unclosed message/request
            await this.WritePendingMessageDataAsync(true)
                .ConfigureAwait(false);

            this.SetState(BatchWriterState.BatchCompleted);

            // Write the end boundary for the batch
            await ODataMultipartMixedBatchWriterUtils.WriteEndBoundaryAsync(
                this.RawOutputContext.TextWriter,
                this.batchBoundary,
                !this.batchStartBoundaryWritten).ConfigureAwait(false);

            // For compatibility with WCF DS we write a newline after the end batch boundary.
            // Technically it's not needed, but it doesn't violate anything either.
            await this.RawOutputContext.TextWriter.WriteLineAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously starts a new changeset - implementation of the actual functionality.
        /// </summary>
        /// <param name="changesetId">The value for changeset boundary for multipart batch.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WriteStartChangesetImplementationAsync(string changesetId)
        {
            Debug.Assert(changesetId != null, "changesetId != null");

            // Write pending message data (headers, response line) for a previously unclosed message/request
            await this.WritePendingMessageDataAsync(true)
                .ConfigureAwait(false);

            this.SetState(BatchWriterState.ChangesetStarted);
            Debug.Assert(this.changeSetBoundary == null, "this.changeSetBoundary == null");
            this.changeSetBoundary = ODataMultipartMixedBatchWriterUtils.CreateChangeSetBoundary(this.RawOutputContext.WritingResponse, changesetId);

            // Write the boundary string
            await ODataMultipartMixedBatchWriterUtils.WriteStartBoundaryAsync(
                this.RawOutputContext.TextWriter,
                this.batchBoundary,
                !this.batchStartBoundaryWritten).ConfigureAwait(false);
            this.batchStartBoundaryWritten = true;

            // Write the changeset headers
            await ODataMultipartMixedBatchWriterUtils.WriteChangesetPreambleAsync(
                this.RawOutputContext.TextWriter,
                this.changeSetBoundary).ConfigureAwait(false);
            this.changesetStartBoundaryWritten = false;

            // Set state to track dependsOnIds.
            this.dependsOnIdsTracker.ChangeSetStarted();
        }

        /// <summary>
        /// Asynchronously ends an active changeset - implementation of the actual functionality.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected override async Task WriteEndChangesetImplementationAsync()
        {
            // Write pending message data (headers, response line) for a previously unclosed message/request
            await this.WritePendingMessageDataAsync(true)
                .ConfigureAwait(false);

            string currentChangesetBoundary = this.changeSetBoundary;

            // Change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangesetCompleted);

            this.dependsOnIdsTracker.ChangeSetEnded();

            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");
            this.changeSetBoundary = null;

            // In the case of an empty changeset the start changeset boundary has not been written yet
            // we will leave it like that, since we want the empty changeset to be represented only as
            // the end changeset boundary.
            // Due to WCF DS V2 compatibility we must not write the start boundary in this case
            // otherwise WCF DS V2 won't be able to read it (it fails on the start-end boundary empty changeset).

            // Write the end boundary for the change set
            await ODataMultipartMixedBatchWriterUtils.WriteEndBoundaryAsync(
                this.RawOutputContext.TextWriter,
                currentChangesetBoundary,
                !this.changesetStartBoundaryWritten).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request
        /// - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">
        /// The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <param name="dependsOnIds">The prerequisite request ids of this request. By default its value should be null for Multipart/Mixed
        /// format and the dependsOnIds implicitly derived per the protocol will be used; Otherwise, non-null will be used as override after
        /// validation.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an <see cref="ODataBatchOperationRequestMessage"/>
        /// that can be used to write the request operation.</returns>
        protected override async Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageImplementationAsync(
            string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption,
            IEnumerable<string> dependsOnIds)
        {
            // Write pending message data (headers, response line) for a previously unclosed message/request
            await this.WritePendingMessageDataAsync(true)
                .ConfigureAwait(false);

            // Create the new request operation
            // For Multipart batch format, validate dependsOnIds if it is user explicit input, otherwise skip validation
            // when it is implicitly derived per protocol.
            ODataBatchOperationRequestMessage operationRequestMessage = BuildOperationRequestMessage(
                this.RawOutputContext.OutputStream,
                method, uri, contentId,
                this.changeSetBoundary,
                dependsOnIds);

            this.SetState(BatchWriterState.OperationCreated);

            // Write the operation's start boundary string
            await this.WriteStartBoundaryForOperationAsync()
                .ConfigureAwait(false);

            if (contentId != null)
            {
                this.dependsOnIdsTracker.AddDependsOnId(contentId);
            }

            // Write the headers and request line
            await ODataMultipartMixedBatchWriterUtils.WriteRequestPreambleAsync(
                this.RawOutputContext.TextWriter,
                method,
                uri,
                this.RawOutputContext.MessageWriterSettings.BaseUri,
                changeSetBoundary != null,
                contentId,
                payloadUriOption).ConfigureAwait(false);

            return operationRequestMessage;
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an <see cref="ODataBatchOperationResponseMessage"/>
        /// that can be used to write the response operation.</returns>
        protected override async Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageImplementationAsync(string contentId)
        {
            await this.WritePendingMessageDataAsync(true)
                .ConfigureAwait(false);

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            this.CurrentOperationResponseMessage = BuildOperationResponseMessage(
                this.RawOutputContext.OutputStream,
                contentId,
                this.changeSetBoundary);

            this.SetState(BatchWriterState.OperationCreated);

            // Write the operation's start boundary string
            await this.WriteStartBoundaryForOperationAsync()
                .ConfigureAwait(false);

            // Write the headers and request separator line
            await ODataMultipartMixedBatchWriterUtils.WriteResponsePreambleAsync(
                this.RawOutputContext.TextWriter,
                changeSetBoundary != null,
                contentId).ConfigureAwait(false);

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        private void StartBatchOperationContent()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.RawOutputContext.TextWriter != null, "Must have a batch writer!");

            // write the pending headers (if any)
            this.WritePendingMessageData(false);

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.RawOutputContext.TextWriter.Flush();
        }

        /// <summary>
        /// Disposes the batch writer and set the 'OperationStreamRequested' batch writer state;
        /// called after the flush operation(s) have completed.
        /// </summary>
        private void DisposeBatchWriterAndSetContentStreamRequestedState()
        {
            this.RawOutputContext.CloseWriter();

            this.SetState(BatchWriterState.OperationStreamRequested);
        }

        /// <summary>
        /// Writes the start boundary for an operation. This is either the batch or the changeset boundary.
        /// </summary>
        private void WriteStartBoundaryForOperation()
        {
            if (this.changeSetBoundary == null)
            {
                ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.changeSetBoundary, !this.changesetStartBoundaryWritten);
                this.changesetStartBoundaryWritten = true;
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
                Debug.Assert(this.RawOutputContext.TextWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.RawOutputContext.WritingResponse, "If the response message is available we must be writing response.");
                    int statusCode = this.CurrentOperationResponseMessage.StatusCode;
                    string statusMessage = HttpUtils.GetStatusMessage(statusCode);
                    this.RawOutputContext.TextWriter.WriteLine("{0} {1} {2}", ODataConstants.HttpVersionInBatching, statusCode, statusMessage);
                }

                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        string headerName = headerPair.Key;
                        string headerValue = headerPair.Value;
                        this.RawOutputContext.TextWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", headerName, headerValue));
                    }
                }

                // write CRLF after the headers (or request/response line if there are no headers)
                this.RawOutputContext.TextWriter.WriteLine();

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;
                }
            }
        }

        /// <summary>
        /// Asynchronously writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task StartBatchOperationContentAsync()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.RawOutputContext.TextWriter != null, "Must have a batch writer!");

            // Write the pending headers (if any)
            await this.WritePendingMessageDataAsync(false)
                .ConfigureAwait(false);

            // Flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            await this.RawOutputContext.TextWriter.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the start boundary for an operation. This is either the batch or the changeset boundary.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteStartBoundaryForOperationAsync()
        {
            if (this.changeSetBoundary == null)
            {
                await ODataMultipartMixedBatchWriterUtils.WriteStartBoundaryAsync(
                    this.RawOutputContext.TextWriter,
                    this.batchBoundary,
                    !this.batchStartBoundaryWritten).ConfigureAwait(false);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                await ODataMultipartMixedBatchWriterUtils.WriteStartBoundaryAsync(
                    this.RawOutputContext.TextWriter,
                    this.changeSetBoundary,
                    !this.changesetStartBoundaryWritten).ConfigureAwait(false);
                this.changesetStartBoundaryWritten = true;
            }
        }

        /// <summary>
        /// Asynchronously write any pending headers for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WritePendingMessageDataAsync(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage != null)
            {
                Debug.Assert(this.RawOutputContext.TextWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.RawOutputContext.WritingResponse, "If the response message is available we must be writing response.");
                    int statusCode = this.CurrentOperationResponseMessage.StatusCode;
                    string statusMessage = HttpUtils.GetStatusMessage(statusCode);

                    await this.RawOutputContext.TextWriter.WriteLineAsync(
                        string.Concat(ODataConstants.HttpVersionInBatching, " ", statusCode, " ", statusMessage)).ConfigureAwait(false);
                }

                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        await this.RawOutputContext.TextWriter.WriteLineAsync(
                            string.Concat(headerPair.Key, ": ", headerPair.Value))
                            .ConfigureAwait(false);
                    }
                }

                // Write CRLF after the headers (or request/response line if there are no headers)
                await this.RawOutputContext.TextWriter.WriteLineAsync()
                    .ConfigureAwait(false);

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;
                }
            }
        }
    }
}
