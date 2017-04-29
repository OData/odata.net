//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core.Json;

    #endregion Namespaces

    /// <summary>
    /// Class for writing OData batch messages of MIME application/json type.
    /// </summary>
    internal sealed class ODataJsonLightBatchWriter : ODataBatchWriter
    {
        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// Indicates whether the json batch top-level envelope has been written
        /// </summary>
        private bool isBatchEnvelopeWritten;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        internal ODataJsonLightBatchWriter(ODataJsonLightOutputContext jsonLightOutputContext)
            : base(jsonLightOutputContext)
        {
            this.jsonWriter = this.JsonLightOutputContext.JsonWriter;
            this.isBatchEnvelopeWritten = false;
        }

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public override void Flush()
        {
            this.VerifyCanFlush(true);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.JsonLightOutputContext.Flush();
            }
            catch
            {
                this.SetState(BatchWriterState.Error);
                throw;
            }
        }

#if ODATALIB_ASYNC
        /// <summary>Flushes the write buffer to the underlying stream asynchronously.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            this.VerifyCanFlush(false);

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            return this.JsonLightOutputContext.FlushAsync().FollowOnFaultWith(t => this.SetState(BatchWriterState.Error));
        }
#endif

        /// <summary>
        /// Gets the writer's output context as the real runtime type.
        /// </summary>
        private ODataJsonLightOutputContext JsonLightOutputContext
        {
            get { return this.OutputContext as ODataJsonLightOutputContext;}
        }

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        protected override void VerifyCanFlush(bool synchronousCall)
        {
            this.JsonLightOutputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
            if (this.State == BatchWriterState.OperationStreamRequested)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// Validates that the batch writer is ready to process a new write request.
        /// </summary>
        protected override void ValidateWriterReady()
        {
            this.JsonLightOutputContext.VerifyNotDisposed();

            // If the operation stream was requested but not yet disposed, the writer can't be used to do anything.
            if (this.State == BatchWriterState.OperationStreamRequested)
            {
                throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested);
            }
        }

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public override void BatchOperationContentStreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.JsonLightOutputContext.FlushBuffers();

            // Dispose the batch writer (since we are now writing the operation content) and set the corresponding state.
            this.DisposeBatchWriterAndSetContentStreamRequestedState();
        }

#if ODATALIB_ASYNC
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
            return this.JsonLightOutputContext.FlushBuffersAsync()
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

            // Close the response object. Do not dispose the json writer.
            this.jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Starts a new batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteStartBatchImplementation()
        {
            this.SetState(BatchWriterState.BatchStarted);
        }

        /// <summary>
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndBatchImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // Close the responses array
            jsonWriter.EndArrayScope();

            // Close the top level scope
            jsonWriter.EndObjectScope();
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

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangesetCompleted);

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.urlResolver.Reset();
            this.CurrentOperationContentId = null;
        }

        protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(string method,
            Uri uri, string contentId)
        {
            if (this.State == BatchWriterState.ChangesetStarted)
            {
                this.InterceptException(this.IncreaseChangeSetSize);
            }
            else
            {
                this.InterceptException(this.IncreaseBatchSize);
            }

            // write pending message data (headers, request line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // Add a potential Content-ID header to the URL resolver so that it will be available
            // to subsequent operations.
            // Note that what we add here is the Content-ID header of the previous operation (if any).
            // This also means that the Content-ID of the last operation in a changeset will never get
            // added to the cache which is fine since we cannot reference it anywhere.
            if (this.CurrentOperationContentId != null)
            {
                this.urlResolver.AddContentId(this.CurrentOperationContentId);
            }

            this.InterceptException(() => uri = ODataBatchUtils.CreateOperationRequestUri(uri, this.JsonLightOutputContext.MessageWriterSettings.PayloadBaseUri, this.urlResolver));

            // create the new request operation
            this.CurrentOperationRequestMessage = ODataBatchOperationRequestMessage.CreateWriteMessage(
                this.JsonLightOutputContext.GetOutputStream(),
                method,
                uri,
                /*operationListener*/ this,
                this.urlResolver);

            // Generate a GUID if the content Id is not provided.
            if (contentId == null)
            {
                contentId = Guid.NewGuid().ToString();
            }

            if (this.State == BatchWriterState.ChangesetStarted)
            {
                this.RememberContentIdHeader(contentId);
            }

            this.SetState(BatchWriterState.OperationCreated);

            if (!this.isBatchEnvelopeWritten)
            {
                WriteBatchEnvelope(true);
            }

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            this.jsonWriter.WriteName("id");
            this.jsonWriter.WriteValue( contentId);

            this.jsonWriter.WriteName("method");
            this.jsonWriter.WriteValue(method);

            this.jsonWriter.WriteName("url");
            this.jsonWriter.WriteValue(UriUtils.UriToString(uri));

            return this.CurrentOperationRequestMessage;
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
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");
            Debug.Assert(this.State != BatchWriterState.ChangesetStarted, "this.State != BatchWriterState.ChangesetStarted");

            // Set the current content ID. If no Content-ID header is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.CurrentOperationContentId = contentId;

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.urlResolver.ContainsContentId(contentId))
            {
                throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(contentId));
            }
        }

        /// <summary>
        /// Writes the json format batch envelope.
        /// Always sets the isBatchEvelopeWritten flag to true before return.
        /// </summary>
        /// <param name="isRequest">Whether this is for request envelope.</param>
        private void WriteBatchEnvelope(bool isRequest)
        {
            Debug.Assert(!this.isBatchEnvelopeWritten, "!this.isBatchEnvelopeWritten");

            // Start the top level scope
            this.jsonWriter.StartObjectScope();

            // Start the requests / responses property
            this.jsonWriter.WriteName(isRequest ? "requests" : "responses");
            this.jsonWriter.StartArrayScope();

            this.isBatchEnvelopeWritten = true;
        }

        /// <summary>
        /// Sets a new writer state; verifies that the transition from the current state into new state is valid.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        protected override void SetState(BatchWriterState newState)
        {
            this.InterceptException(() => this.ValidateTransition(newState, /*customizedValidationAction*/null));

            this.State = newState;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        protected override void VerifyCanCreateOperationRequestMessage(bool synchronousCall, string method, Uri uri,
            string contentId)
        {
            this.CanCreateOperationRequestMessageVerifierCommon(synchronousCall, method, uri, contentId);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet header.</param>
        /// <returns>The message that can be used to write the response operation.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            this.CurrentOperationResponseMessage = ODataBatchOperationResponseMessage.CreateWriteMessage(
                this.JsonLightOutputContext.GetOutputStream(),
                /*operationListener*/ this,
                this.urlResolver.BatchMessageUrlResolver,
                contentId);
            this.SetState(BatchWriterState.OperationCreated);

            Debug.Assert(this.CurrentOperationContentId == null, "The Content-ID header is only supported in request messages.");

            if (!this.isBatchEnvelopeWritten)
            {
                WriteBatchEnvelope(false);
            }

            // Start the Json object for the response
            this.WriteStartBoundaryForOperation();

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Write any pending headers for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        protected override void WritePendingMessageData(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage != null)
            {
                Debug.Assert(this.JsonLightOutputContext.JsonWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.JsonLightOutputContext.WritingResponse, "If the response message is available we must be writing response.");

                    this.jsonWriter.WriteName("id");
                    this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.ContentId);

                    this.jsonWriter.WriteName("status");
                    this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.StatusCode);
                }

                // headers attribute
                this.jsonWriter.WriteName("headers");
                this.jsonWriter.StartObjectScope();
                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        this.jsonWriter.WriteName(headerPair.Key);
                        this.jsonWriter.WriteValue(headerPair.Value);
                    }
                }
                this.jsonWriter.EndObjectScope();

                // body attribute
                if (this.CurrentOperationResponseMessage != null)
                {
                    if (!ODataBatchWriterUtils.HasResponseBody(this.CurrentOperationResponseMessage))
                    {
                        // Close the individual response object now since there are no content.
                        // If there is content, the response object will be closed when the content stream is disposed.
                        // See <cref="ODataJsonLightBatchWriter.BatchOperationContentStreamDisposed"/>.
                        this.jsonWriter.EndObjectScope();
                    }
                    else
                    {
                        this.jsonWriter.WriteName("body");
                    }
                }
                else
                {
                    if (!ODataBatchWriterUtils.HasRequestBody(this.CurrentOperationRequestMessage.Method))
                    {
                        this.jsonWriter.EndObjectScope();
                    }
                    else
                    {
                        this.jsonWriter.WriteName("body");
                    }
                }

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
        protected override void WriteStartBoundaryForOperation()
        {
            // Start the individual response object
            this.jsonWriter.StartObjectScope();
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
            this.JsonLightOutputContext.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            this.JsonLightOutputContext.JsonWriter.Flush();

            // The OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that WCF DS Server does serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        protected override void StartBatchOperationContent()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.JsonLightOutputContext.JsonWriter != null, "Must have a Json writer!");

            // write the pending headers (if any)
            this.WritePendingMessageData(false);

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.JsonLightOutputContext.JsonWriter.Flush();
        }

        /// <summary>
        /// Disposes the batch writer and set the 'OperationStreamRequested' batch writer state;
        /// called after the flush operation(s) have completed.
        /// </summary>
        protected override void DisposeBatchWriterAndSetContentStreamRequestedState()
        {
            this.SetState(BatchWriterState.OperationStreamRequested);
        }
    }
}
