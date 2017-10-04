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
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    internal sealed class ODataMultipartMixedBatchWriter : ODataBatchWriter
    {
        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

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
        internal ODataMultipartMixedBatchWriter(ODataRawOutputContext rawOutputContext, string batchBoundary)
            : base(rawOutputContext)
        {
            Debug.Assert(this.RawOutputContext != null, "this.RawOutputContext != null");

            ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary is null");

            this.batchBoundary = batchBoundary;
            this.RawOutputContext.InitializeRawValueWriter();
        }

        /// <summary>
        /// Gets the writer's output context as the real runtime type.
        /// </summary>
        internal ODataRawOutputContext RawOutputContext
        {
            get { return this.OutputContext as ODataRawOutputContext; }
        }

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public override void Flush()
        {
            this.VerifyCanFlush(true);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.RawOutputContext.Flush();
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
        public override Task FlushAsync()
        {
            this.VerifyCanFlush(false);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            return this.RawOutputContext.FlushAsync().FollowOnFaultWith(t => this.SetState(BatchWriterState.Error));
        }
#endif
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public override void BatchOperationContentStreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.RawOutputContext.FlushBuffers();

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
        public override Task BatchOperationContentStreamRequestedAsync()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Asynchronously flush the async buffered stream to the underlying message stream (if there's any);
            // then dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            return this.RawOutputContext.FlushBuffersAsync()
                .FollowOnSuccessWith(task => this.DisposeBatchWriterAndSetContentStreamRequestedState());
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        public override void BatchOperationContentStreamDisposed()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");

            this.SetState(BatchWriterState.OperationStreamDisposed);
            this.CurrentOperationRequestMessage = null;
            this.CurrentOperationResponseMessage = null;
            this.RawOutputContext.InitializeRawValueWriter();
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

            // The OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }


        /// <summary>
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteStartChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangesetStarted);
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);

            // write the boundary string
            ODataMultipartMixedBatchWriterUtils.WriteStartBoundary(this.RawOutputContext.TextWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
            this.batchStartBoundaryWritten = true;

            // write the change set headers
            ODataMultipartMixedBatchWriterUtils.WriteChangeSetPreamble(this.RawOutputContext.TextWriter, this.changeSetBoundary);
            this.changesetStartBoundaryWritten = false;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <param name="payloadUriOption">The format of operation Request-URI, which could be AbsoluteUri, AbsoluteResourcePathAndHost, or RelativeResourcePath.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method, Uri uri, string contentId, BatchPayloadUriOption payloadUriOption)
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
            if (this.CurrentOperationContentId != null)
            {
                this.PayloadUriConverter.AddContentId(this.CurrentOperationContentId);
            }

            this.InterceptException(() => uri = ODataBatchUtils.CreateOperationRequestUri(uri, this.RawOutputContext.MessageWriterSettings.BaseUri, this.PayloadUriConverter));

            // create the new request operation
            this.CurrentOperationRequestMessage = ODataBatchOperationRequestMessage.CreateWriteMessage(
                this.RawOutputContext.OutputStream,
                method,
                uri,
                /*operationListener*/ this,
                this.PayloadUriConverter,
                this.Container);

            if (this.changeSetBoundary != null)
            {
                this.RememberContentIdHeader(contentId);
            }

            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request line
            ODataMultipartMixedBatchWriterUtils.WriteRequestPreamble(this.RawOutputContext.TextWriter, method, uri,
                this.RawOutputContext.MessageWriterSettings.BaseUri, changeSetBoundary != null, contentId,
                payloadUriOption);

            return this.CurrentOperationRequestMessage;
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

            // In the case of an empty changeset the start changeset boundary has not been written yet
            // we will leave it like that, since we want the empty changeset to be represented only as
            // the end changeset boundary.
            // Due to WCF DS V2 compatibility we must not write the start boundary in this case
            // otherwise WCF DS V2 won't be able to read it (it fails on the start-end boundary empty changeset).

            // write the end boundary for the change set
            ODataMultipartMixedBatchWriterUtils.WriteEndBoundary(this.RawOutputContext.TextWriter, currentChangeSetBoundary, !this.changesetStartBoundaryWritten);

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.PayloadUriConverter.Reset();
            this.CurrentOperationContentId = null;
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
            this.CurrentOperationResponseMessage = ODataBatchOperationResponseMessage.CreateWriteMessage(
                this.RawOutputContext.OutputStream,
                /*operationListener*/ this,
                this.PayloadUriConverter.BatchMessagePayloadUriConverter,
                this.Container);
            this.SetState(BatchWriterState.OperationCreated);

            Debug.Assert(this.CurrentOperationContentId == null, "The Content-ID header is only supported in request messages.");

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request separator line
            ODataMultipartMixedBatchWriterUtils.WriteResponsePreamble(this.RawOutputContext.TextWriter, changeSetBoundary != null, contentId);

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        protected override void StartBatchOperationContent()
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
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        protected override void VerifyCanCreateOperationRequestMessage(bool synchronousCall, string method, Uri uri, string contentId)
        {
            this.CanCreateOperationRequestMessageVerifierCommon(synchronousCall, method, uri, contentId);
            VerifyCanCreateOperationRequestMessageAgainstChangeSetBoundary(method, contentId);
        }

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        protected override void VerifyCanFlush(bool synchronousCall)
        {
            this.RawOutputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
            if (this.State == BatchWriterState.OperationStreamRequested)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected override void SetState(BatchWriterState newState)
        {
            this.InterceptException(() => this.ValidateTransition(
                newState,
                () => ValidateTransitionAgainstChangesetBoundary(newState, this.changeSetBoundary)));

            switch (newState)
            {
                case BatchWriterState.BatchStarted:
                    Debug.Assert(!this.batchStartBoundaryWritten, "The batch boundary must not be written before calling WriteStartBatch.");
                    break;
                case BatchWriterState.ChangesetStarted:
                    Debug.Assert(this.changeSetBoundary == null, "this.changeSetBoundary == null");
                    this.changeSetBoundary = ODataMultipartMixedBatchWriterUtils.CreateChangeSetBoundary(this.RawOutputContext.WritingResponse);
                    break;
                case BatchWriterState.ChangesetCompleted:
                    Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");
                    this.changeSetBoundary = null;
                    break;
            }

            this.State = newState;
        }

        /// <summary>
        /// Verifies that the writer is not disposed.
        /// </summary>
        protected override void VerifyNotDisposed()
        {
            this.RawOutputContext.VerifyNotDisposed();
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
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // Set the current content ID. If no Content-ID header is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.CurrentOperationContentId = contentId;

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.PayloadUriConverter.ContainsContentId(contentId))
            {
                throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(contentId));
            }
        }

        /// <summary>
        /// Verifies that, for the case within a changeset, CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="method">The HTTP method to be validated.</param>
        /// <param name="contentId">The content Id string to be validated.</param>
        private void VerifyCanCreateOperationRequestMessageAgainstChangeSetBoundary(string method, string contentId)
        {
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
        }

        /// <summary>
        /// Validates state transition is allowed if we are within a changeset.
        /// </summary>
        /// <param name="newState">Teh new writer state to transition into.</param>
        /// <param name="changeSetBoundary">The changeset boundary string.</param>
        private static void ValidateTransitionAgainstChangesetBoundary(BatchWriterState newState, string changeSetBoundary)
        {
            // make sure that we are not starting a changeset when one is already active
            if (newState == BatchWriterState.ChangesetStarted)
            {
                if (changeSetBoundary != null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
                }
            }

            // make sure that we are not completing a changeset without an active changeset
            if (newState == BatchWriterState.ChangesetCompleted)
            {
                if (changeSetBoundary == null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
                }
            }

            // make sure that we are not completing a batch while a changeset is still active
            if (newState == BatchWriterState.BatchCompleted)
            {
                if (changeSetBoundary != null)
                {
                    throw new ODataException(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
                }
            }
        }
    }
}
