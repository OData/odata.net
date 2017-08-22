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
    using System.Globalization;

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
        #region JsonPropertyNames
        /// <summary>
        /// Camel-case property name for request Id in Json batch.
        /// </summary>
        internal const string PropertyId = "id";

        /// <summary>
        /// Property name for request atomic group association in Json batch.
        /// </summary>
        internal const string PropertyAtomicityGroup = "atomicityGroup";

        /// <summary>
        /// Property name for request HTTP headers in Json batch.
        /// </summary>
        internal const string PropertyHeaders = "headers";

        /// <summary>
        /// Property name for request body in Json batch.
        /// </summary>
        internal const string PropertyBody = "body";
        #endregion JsonPropertyNames

        #region RequestJsonPropertyNames
        /// <summary>
        /// Property name for top-level requests array in Json batch request.
        /// </summary>
        internal const string PropertyRequests = "requests";

        /// <summary>
        /// Property name for request HTTP method in Json batch request.
        /// </summary>
        internal const string PropertyMethod = "method";

        /// <summary>
        /// Property name for request URL in Json batch request.
        /// </summary>
        internal const string PropertyUrl = "url";
        #endregion RequestJsonPropertyNames

        #region ResponseJsonPropertyNames
        /// <summary>
        /// Property name for top-level responses array in Json batch response.
        /// </summary>
        internal const string PropertyResponses = "responses";

        /// <summary>
        /// Property name for response status in Json batch response.
        /// </summary>
        internal const string PropertyStatus = "status";
        #endregion ResponseJsonPropertyNames

        /// <summary>
        /// The underlying JSON writer.
        /// </summary>
        private readonly IJsonWriter jsonWriter;

        /// <summary>
        /// Indicates whether the json batch top-level envelope has been written
        /// </summary>
        private bool isBatchEnvelopeWritten;

        /// <summary>
        /// The auto-generated Guid for AtomicityGroup of the Json item. Should be null for Json item
        /// that doesn't belong to atomic group.
        /// </summary>
        private string atomicityGroupId = null;

        /// <summary>
        /// Indicates whether we have pending message Json object open that needs to be closed.
        /// Default value set to false since no messages available.
        /// </summary>
        private bool isPendingMessageObjectOpened = false;

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

        /// <summary>
        /// Gets the writer's output context as the real runtime type.
        /// </summary>
        private ODataJsonLightOutputContext JsonLightOutputContext
        {
            get { return this.OutputContext as ODataJsonLightOutputContext; }
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
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        public override void BatchOperationContentStreamRequested()
        {
            // Write any pending data and flush the batch writer to the async buffered stream
            this.StartBatchOperationContent();

            // Flush the async buffered stream to the underlying message stream (if there's any)
            this.JsonLightOutputContext.FlushBuffers();

            // Set the corresponding state but we need to keep the Json batch writer around.
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
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndBatchImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            EnsurePreceedingMessageIsClosed();

            Debug.Assert(!this.isPendingMessageObjectOpened, "!this.isPendingMessageObjectOpened");

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
            Debug.Assert(this.atomicityGroupId == null, "this.atomicityGroupId == null");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangesetStarted);
            this.atomicityGroupId = Guid.NewGuid().ToString();

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);
        }

        /// <summary>
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        protected override void WriteEndChangesetImplementation()
        {
            Debug.Assert(this.atomicityGroupId != null, "this.atomicityGroupId != null");

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangesetCompleted);
            this.atomicityGroupId = null;

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set.
            this.UrlResolver.Reset();
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
                this.UrlResolver.AddContentId(this.CurrentOperationContentId);
            }

            this.InterceptException(() => uri = ODataBatchUtils.CreateOperationRequestUri(
                uri, this.JsonLightOutputContext.MessageWriterSettings.PayloadBaseUri, this.UrlResolver));

            // create the new request operation
            this.CurrentOperationRequestMessage = ODataBatchOperationRequestMessage.CreateWriteMessage(
                this.JsonLightOutputContext.GetOutputStream(),
                method,
                uri,
                /*operationListener*/ this,
                this.UrlResolver);

            // For json batch request, content Id is required for single request or request within atomicityGroup.
            if (contentId == null)
            {
                contentId = Guid.NewGuid().ToString();
            }

            if (this.atomicityGroupId != null)
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

            this.jsonWriter.WriteName(PropertyId);
            this.jsonWriter.WriteValue(contentId);

            if (this.atomicityGroupId != null)
            {
                this.jsonWriter.WriteName(PropertyAtomicityGroup);
                this.jsonWriter.WriteValue(this.atomicityGroupId);
            }

            this.jsonWriter.WriteName(PropertyMethod);
            this.jsonWriter.WriteValue(method);

            this.jsonWriter.WriteName(PropertyUrl);
            this.jsonWriter.WriteValue(UriUtils.UriToString(uri));

            return this.CurrentOperationRequestMessage;
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
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch
        /// response - implementation of the actual functionality.
        /// </summary>
        /// <param name="contentId">The Content-ID value to write in ChangeSet header.</param>
        /// <returns>The message that can be used to rite the response operation.</returns>
        protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(string contentId)
        {
            this.WritePendingMessageData(true);

            // Url resolver: In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            //
            // ContentId: could be null from public API common for both formats, so we don't enforce non-null value for Json format
            // for sake of backward compatiblity. For Json Batch response message, normally caller should use a non-null value
            // available from the request; and we generate a new one when a null value is passed in.
            this.CurrentOperationResponseMessage = ODataBatchOperationResponseMessage.CreateWriteMessage(
                this.JsonLightOutputContext.GetOutputStream(),
                /*operationListener*/ this,
                this.UrlResolver.BatchMessageUrlResolver,
                contentId ?? Guid.NewGuid().ToString());
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
        /// Write any pending data for the current operation message (if any).
        /// </summary>
        /// <param name="reportMessageCompleted">
        /// A flag to control whether after writing the pending data we report writing the message to be completed or not.
        /// </param>
        protected override void WritePendingMessageData(bool reportMessageCompleted)
        {
            if (this.CurrentOperationMessage == null)
            {
                if (reportMessageCompleted)
                {
                    // Check if we need to close preceeding message Json object.
                    EnsurePreceedingMessageIsClosed();
                }

                return;
            }

            Debug.Assert(this.JsonLightOutputContext.JsonWriter != null, "Must have a batch writer if pending data exists.");

            if (this.CurrentOperationRequestMessage != null)
            {
                WritePendingRequestMessageData();
            }
            else
            {
                WritePendingResponseMessageData();
            }

            if (reportMessageCompleted)
            {
                this.CurrentOperationMessage.PartHeaderProcessingCompleted();
                this.CurrentOperationRequestMessage = null;
                this.CurrentOperationResponseMessage = null;

                EnsurePreceedingMessageIsClosed();
            }
        }

        /// <summary>
        /// Writes the start boundary for an operation. This is Json start object.
        /// </summary>
        protected override void WriteStartBoundaryForOperation()
        {
            // Start the individual response object
            this.jsonWriter.StartObjectScope();
            this.isPendingMessageObjectOpened = true;
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

            this.jsonWriter.WriteRawValue(string.Format(CultureInfo.InvariantCulture,
                "{0} \"{1}\" {2}",
                JsonConstants.ArrayElementSeparator,
                ODataJsonLightBatchWriter.PropertyBody,
                JsonConstants.NameValueSeparator));

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.JsonLightOutputContext.JsonWriter.Flush();
        }

        /// <summary>
        /// Set the 'OperationStreamRequested' batch writer state;
        /// called after the flush operation(s) have completed.
        /// </summary>
        /// <remarks>Json batch writer is not disposed since it contains useful Json scopes.</remarks>
        protected override void DisposeBatchWriterAndSetContentStreamRequestedState()
        {
            this.SetState(BatchWriterState.OperationStreamRequested);
        }

        /// <summary>
        /// Close preceeding message Json object if any.
        /// </summary>
        private void EnsurePreceedingMessageIsClosed()
        {
            // There shouldn't be any pending message object.
            Debug.Assert(this.CurrentOperationMessage == null, "this.CurrentOperationMessage == null");

            // Check if we need to close preceeding message Json object.
            if (this.isPendingMessageObjectOpened)
            {
                this.jsonWriter.EndObjectScope();
                this.isPendingMessageObjectOpened = false;
            }
        }

        /// <summary>
        /// Remember a non-null id for request operation of atomicityGroup.
        /// If a non-null id value is specified for a request operation of atomicityGroup, record it in the URL resolver.
        /// </summary>
        /// <param name="contentId">The id proprety value read from the message.</param>
        /// <remarks>
        /// Note that the id of this operation will only become visible once this operation has been written
        /// and OperationCompleted has been called on the URL resolver.
        /// </remarks>
        private void RememberContentIdHeader(string contentId)
        {
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");
            Debug.Assert(this.atomicityGroupId != null, "this.atomicityGroupId != null");

            // Set the current content ID. If no "id" property value is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.CurrentOperationContentId = contentId;

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.UrlResolver.ContainsContentId(contentId))
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
            this.jsonWriter.WriteName(isRequest ? PropertyRequests : PropertyResponses);
            this.jsonWriter.StartArrayScope();

            this.isBatchEnvelopeWritten = true;
        }

        /// <summary>
        /// Writing pending data for the current request message.
        /// </summary>
        private void WritePendingRequestMessageData()
        {
            Debug.Assert(this.CurrentOperationRequestMessage != null, "this.CurrentOperationRequestMessage != null");

            // headers property.
            this.jsonWriter.WriteName(PropertyHeaders);
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
        }

        /// <summary>
        /// Writing pending data for the current response message.
        /// </summary>
        private void WritePendingResponseMessageData()
        {
            Debug.Assert(this.JsonLightOutputContext.WritingResponse, "If the response message is available we must be writing response.");
            Debug.Assert(this.CurrentOperationResponseMessage != null, "this.CurrentOperationResponseMessage != null");

            // id property.
            this.jsonWriter.WriteName(PropertyId);
            this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.ContentId);

            // atomicityGroup property.
            if (this.atomicityGroupId != null)
            {
                this.jsonWriter.WriteName(PropertyAtomicityGroup);
                this.jsonWriter.WriteValue(this.atomicityGroupId);
            }

            // response status property.
            this.jsonWriter.WriteName(PropertyStatus);
            this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.StatusCode);

            // headers property.
            this.jsonWriter.WriteName(PropertyHeaders);
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
        }
    }
}
