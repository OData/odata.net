//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces.

    /// <summary>
    /// Class for writing OData batch messages; also verifies the proper sequence of write calls on the writer.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ODataBatchWriter : IODataBatchOperationListener, IDisposable
#else
    public sealed class ODataBatchWriter : IODataBatchOperationListener, IDisposable
#endif
    {
        /// <summary>The encoding to use to write everything except for the operation payloads (e.g., the batch boundaries, headers, etc.)</summary>
        private readonly Encoding encoding;

        /// <summary>A flag indicating whether we are writing a request or a response message.</summary>
        private readonly bool writingResponse;

        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

        /// <summary>The <see cref="ODataWriterSettings"/> for the batch writer.</summary>
        private readonly ODataWriterSettings settings;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>The output stream that was passed to the writer.</summary>
        private Stream outputStream;

        /// <summary>
        /// A helper buffering stream to overcome the limitation of text writer of supporting only synchronous APIs.
        /// </summary>
        private AsyncBufferedStream asyncBufferedStream;

        /// <summary>The state the writer currently is in.</summary>
        private BatchWriterState state;

        /// <summary>
        /// The boundary string for the current changeset (only set when writing a changeset, 
        /// e.g., after WriteStartChangeSet has been called and before WriteEndChangeSet is called).
        /// </summary>
        /// <remarks>When not writing a changeset this field is null.</remarks>
        private string changeSetBoundary;

        /// <summary>The <see cref="TextWriter"/> used to write the batch structure (i.e., boundaries, headers, etc.).</summary>
        /// <remarks>
        /// This <see cref="StreamWriter"/> is exclusively used to write the batch structure such as boundaries or headers. 
        /// This writer exists except when the content stream for a batch operation is returned to clients to directly write the
        /// payload to it. While this is the case this writer is null.
        /// </remarks>
        private StreamWriter batchWriter;

        /// <summary>
        /// A flag to indicate whether the batch start boundary has been written or not; important to support writing of empty batches.
        /// </summary>
        private bool batchStartBoundaryWritten;

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no part is written right now or it's a response part.</summary>
        private ODataBatchOperationRequestMessage currentOperationRequestMessage;

        /// <summary>The response message for the operation that is currently written if it's a request; or null if no part is written right now or it's a request part.</summary>
        private ODataBatchOperationResponseMessage currentOperationResponseMessage;

        /// <summary>The current size of the batch message, i.e., how many query operations and changesets have been written.</summary>
        private uint currentBatchSize;

        /// <summary>The current size of the active changeset, i.e., how many request have been written for the changeset.</summary>
        private uint currentChangeSetSize;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="settings">The <see cref="ODataWriterSettings"/> for the batch writer.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a request or a response message.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        private ODataBatchWriter(
            Stream stream, 
            ODataWriterSettings settings, 
            Encoding encoding, 
            bool writingResponse, 
            string batchBoundary, 
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");

            if (batchBoundary == null)
            {
                ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary");
            }

            this.settings = settings;
            this.encoding = encoding;
            this.outputStream = stream;
            this.writingResponse = writingResponse;
            this.batchBoundary = batchBoundary;
            this.synchronous = synchronous;
            this.asyncBufferedStream = new AsyncBufferedStream(stream, true);
            this.batchWriter = new StreamWriter(this.asyncBufferedStream, this.encoding);
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

            /// <summary>A fatal exception has been thrown during writing; the writer is possibly corrupted and cannot be used anymore.</summary>
            FatalExceptionThrown,
        }

        /// <summary>
        /// Helper property to compute whether the batch writer has been disposed.
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                return this.outputStream == null;
            }
        }

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no operation is written right now or it's a response operation.</summary>
        private ODataBatchOperationRequestMessage CurrentOperationRequestMessage
        {
            get
            {
                Debug.Assert(this.currentOperationRequestMessage == null || !this.writingResponse, "Request message can only be filled when writing request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationRequestMessage;
            }

            set
            {
                Debug.Assert(value == null || !this.writingResponse, "Can only set the request message if we're writing a request.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                this.currentOperationRequestMessage = value;
            }
        }

        /// <summary>The response message for the operation that is currently written if it's a request; or null if no operation is written right now or it's a request operation.</summary>
        private ODataBatchOperationResponseMessage CurrentOperationResponseMessage
        {
            get
            {
                Debug.Assert(this.currentOperationResponseMessage == null || this.writingResponse, "Response message can only be filled when writing response.");
                Debug.Assert(this.currentOperationRequestMessage == null || this.currentOperationResponseMessage == null, "Only request or reponse message can be set, not both.");
                return this.currentOperationResponseMessage;
            }

            set
            {
                Debug.Assert(value == null || this.writingResponse, "Can only set the response message if we're writing a response.");
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
                    Debug.Assert(!this.writingResponse, "Request message can only be set when writing request.");
                    return this.currentOperationRequestMessage.OperationMessage;
                }
                else if (this.currentOperationResponseMessage != null)
                {
                    Debug.Assert(this.writingResponse, "Response message can only be set when writing response.");
                    return this.currentOperationResponseMessage.OperationMessage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Starts a new batch; can be only called once and as first call.
        /// </summary>
        public void WriteStartBatch()
        {
            this.ValidateWriterReady();
            this.SetState(BatchWriterState.BatchStarted);
        }

        /// <summary>
        /// Ends a batch; can only be called after WriteStartBatch has been called and if no other active changeset or operation exist.
        /// </summary>
        public void WriteEndBatch()
        {
            this.ValidateWriterReady();

            // write the start boundary for the batch if not written
            if (!this.batchStartBoundaryWritten)
            {
                Debug.Assert(this.CurrentOperationMessage == null, "If not batch boundary was written we must not have an active message.");
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary);
            }

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // write the end boundary for the batch
            ODataBatchWriterUtils.WriteEndBoundary(this.batchWriter, this.batchBoundary);
        }

        /// <summary>
        /// Starts a new changeset; can only be called after WriteStartBatch and if no other active operation or changeset exists.
        /// </summary>
        public void WriteStartChangeset()
        {
            this.ValidateWriterReady();

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangeSetStarted);
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);

            // write the boundary string
            ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary);
            this.batchStartBoundaryWritten = true;

            // write the change set headers
            ODataBatchWriterUtils.WriteChangeSetPreamble(this.batchWriter, this.changeSetBoundary);
        }

        /// <summary>
        /// Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.
        /// </summary>
        public void WriteEndChangeset()
        {
            this.ValidateWriterReady();

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // change the state first so we validate the change set boundary before attempting to write it.
            string currentChangeSetBoundary = this.changeSetBoundary;
            this.SetState(BatchWriterState.ChangeSetCompleted);

            // write the end boundary for the change set
            ODataBatchWriterUtils.WriteEndBoundary(this.batchWriter, currentChangeSetBoundary);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <param name="contentId">The (optional) content ID to be included in this request operation.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(HttpMethod method, Uri uri, string contentId)
        {
            this.ValidateWriterReady();

            if (this.writingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse);
            }

            if (this.changeSetBoundary == null)
            {
                // only allow GET requests for query operations
                if (method != HttpMethod.Get)
                {
                    this.ThrowODataException(Strings.ODataBatchWriter_InvalidHttpMethodForQueryOperation(method.ToText()));
                }

                // do not allow content-id for query operations
                if (contentId != null)
                {
                    this.ThrowODataException(Strings.ODataBatchWriter_ContentIdNotSupportedForQueryOperations(contentId));
                }

                this.InterceptException(this.IncreaseBatchSize);
            }
            else
            {
                // allow all methods except for GET
                if (method == HttpMethod.Get)
                {
                    this.ThrowODataException(Strings.ODataBatchWriter_InvalidHttpMethodForChangeSetRequest(method.ToText()));
                }

                this.InterceptException(this.IncreaseChangeSetSize);
            }

            ExceptionUtils.CheckArgumentNotNull(uri, "uri");

            this.InterceptException(() => uri = ODataBatchWriterUtils.BuildAbsoluteUri(uri, this.settings.BaseUri));

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // create the new request operation
            this.CurrentOperationRequestMessage = new ODataBatchOperationRequestMessage(this.outputStream, method, uri, this);
            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers, request line and (optional) Content-ID
            ODataBatchWriterUtils.WriteRequestPreamble(this.batchWriter, method, uri, contentId);

            return this.CurrentOperationRequestMessage;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <returns>The message that can be used to write the response operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            this.ValidateWriterReady();

            if (!this.writingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }

            this.WritePendingMessageData(true);

            this.CurrentOperationResponseMessage = new ODataBatchOperationResponseMessage(this.outputStream, this);
            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers, request line and (optional) Content-ID
            ODataBatchWriterUtils.WriteResponsePreamble(this.batchWriter);

            return this.CurrentOperationResponseMessage;
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public void Flush()
        {
            this.VerifySynchronousCallAllowed();
            this.VerifyFlushAllowed();

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.FlushSynchronously();
            }
            catch
            {
                this.SetState(BatchWriterState.FatalExceptionThrown);
                throw;
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public Task FlushAsync()
        {
            this.VerifyAsynchronousCallAllowed();
            this.VerifyFlushAllowed();

            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            return this.FlushAsynchronously()
                .ContinueWith(
                    t =>
                    {
                        // TODO, ckerer: if we use TaskContinuationOptions.OnlyOnFaulted instead of this check,
                        //               we always get a TaskCanceledException and it is unclear where it is thrown; review.
                        if (t.IsFaulted)
                        {
                            this.SetState(BatchWriterState.FatalExceptionThrown);

                            // to avoid nested aggregate exceptions only because we changed the internal state
                            // we re-throw the inner exception if there is only one. Otherwise we have to live
                            // with the nesting.
                            throw t.Exception.InnerExceptions.Count == 1 ? t.Exception.InnerException : t.Exception;
                        }
                    });
        }
#endif

        /// <summary>
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the writer.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamRequested()
        {
            this.StartBatchOperationContent();
            this.asyncBufferedStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the operation; 
        /// null if no such action exists.
        /// </returns>
        Task IODataBatchOperationListener.BatchOperationContentStreamRequestedAsync()
        {
            this.StartBatchOperationContent();
            return this.asyncBufferedStream.FlushAsync();
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamDisposed()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.batchWriter == null, "this.batchWriter == null");

            this.SetState(BatchWriterState.OperationStreamDisposed);
            this.CurrentOperationRequestMessage = null;
            this.CurrentOperationResponseMessage = null;
            this.batchWriter = new StreamWriter(this.asyncBufferedStream, this.encoding);
        }

        /// <summary>
        /// Create a func which creates an <see cref="ODataBatchWriter"/> for a given message and stream.
        /// </summary>
        /// <param name="message">The message to create the writer for.</param>
        /// <param name="settings">Configuration settings for the writer to create.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a request or a response message.</param>
        /// <param name="batchBoundary">The batch boundary string used when writing the batch payload.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A func which creates the batch writer given a stream to write to.</returns>
        internal static Func<Stream, ODataBatchWriter> Create(
            ODataMessage message,
            ODataWriterSettings settings,
            bool writingResponse,
            string batchBoundary,
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(message, "message");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");
            ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary");

            return (stream) => CreateBatchWriter(batchBoundary, stream, settings, writingResponse, synchronous);
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter"/> for the specified message and its stream.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary string for this writer.</param>
        /// <param name="stream">The response stream to write to.</param>
        /// <param name="settings">Writer settings to use.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a request or a response message.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>The newly created <see cref="ODataBatchWriter"/> instance.</returns>
        private static ODataBatchWriter CreateBatchWriter(
            string batchBoundary, 
            Stream stream, 
            ODataWriterSettings settings, 
            bool writingResponse,
            bool synchronous)
        {
            if (settings.BaseUri != null && !settings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.ODataWriter_BaseUriMustBeNullOrAbsolute(UriUtils.UriToString(settings.BaseUri)));
            }

            // TODO: Bug 92529: how should we determine the encoding? Use the fixed UTF-8 encoding (this is what the product does)?
            Encoding encoding = MediaTypeUtils.EncodingUtf8NoPreamble;
            return new ODataBatchWriter(stream, settings, encoding, writingResponse, batchBoundary, synchronous);
        }

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        private static bool IsErrorState(BatchWriterState state)
        {
            return state == BatchWriterState.FatalExceptionThrown;
        }

        /// <summary>
        /// Writes all the pending headers and prepares the writer to write a content of the operation.
        /// </summary>
        private void StartBatchOperationContent()
        {
            Debug.Assert(this.CurrentOperationMessage != null, "Expected non-null operation message!");
            Debug.Assert(this.batchWriter != null, "Must have a batch writer!");

            // write the pending headers (if any)
            this.WritePendingMessageData(false);

            // flush the text writer to make sure all buffers of the text writer
            // are flushed to the underlying async stream
            this.batchWriter.Flush();
            this.batchWriter.Dispose();
            this.batchWriter = null;

            this.SetState(BatchWriterState.OperationStreamRequested);
        }

        /// <summary>
        /// Verifies that the writer is in correct state for the Flush operation.
        /// </summary>
        private void VerifyFlushAllowed()
        {
            this.VerifyNotDisposed();

            if (this.state == BatchWriterState.FatalExceptionThrown)
            {
                throw new ODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInFatalErrorState);
            }

            if (this.state == BatchWriterState.OperationStreamRequested)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_FlushOrFlushAsyncCalledInStreamRequestedState);
            }
        }

        /// <summary>
        /// Verifies that a synchronous operation is allowed on this writer.
        /// </summary>
        private void VerifySynchronousCallAllowed()
        {
            if (!this.synchronous)
            {
                throw new ODataException(Strings.ODataBatchWriter_SyncCallOnAsyncWriter);
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Verifies that an asynchronous operation is allowed on this writer.
        /// </summary>
        private void VerifyAsynchronousCallAllowed()
        {
            if (this.synchronous)
            {
                throw new ODataException(Strings.ODataBatchWriter_AsyncCallOnSyncWriter);
            }
        }
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        private void FlushSynchronously()
        {
            // if we are holding an active text writer flush it first to the async buffered stream here
            if (this.batchWriter != null)
            {
                this.batchWriter.Flush();
            }

            this.asyncBufferedStream.FlushSync();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        private Task FlushAsynchronously()
        {
            // if we are holding an active text writer flush it first to the async buffered stream here
            if (this.batchWriter != null)
            {
                this.batchWriter.Flush();
            }

            return this.asyncBufferedStream.FlushAsync();
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "asyncBufferedStream", 
            Justification = "The AsyncBufferedStream supports an extended Dispose protocol where Dispose calls are sometimes ignored. We use DisposeExplicitly to force a dispose (as requested by this rule).")]
        private void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    if (!IsErrorState(this.state) && this.state != BatchWriterState.Start && this.state != BatchWriterState.BatchCompleted)
                    {
                        this.ThrowODataException(Strings.ODataBatchWriter_WriterDisposedWithoutProperBatchEnd);
                    }

                    // if the writer is disposed after a fatal exception has been thrown discard all buffered data
                    // of the underlying output stream so we can safely dispose it (below).
                    bool discardBufferedData = this.state == BatchWriterState.FatalExceptionThrown || this.state == BatchWriterState.Start;

                    try
                    {
                        // flush any active text writer to the underlying stream so we guarantee that there is no data buffered in the text writer;
                        // the underlying async buffered stream will ignore the call to Dispose() that is triggered by disposing this StreamWriter.
                        if (this.batchWriter != null)
                        {
                            this.batchWriter.Flush();
                            this.batchWriter.Dispose();
                        }

                        if (discardBufferedData)
                        {
                            this.asyncBufferedStream.Clear();
                        }

                        this.asyncBufferedStream.DisposeExplicitly();
                    }
                    finally
                    {
                        this.batchWriter = null;
                        this.asyncBufferedStream = null;
                        this.outputStream = null;
                    }
                }
            }

            this.outputStream = null;
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
            catch (Exception e)
            {
                // NOTE: we do not treat ODataExceptions special here since we do not have a concept of
                //       in-stream errors for the batch writer.
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                // any exception is considered fatal; we transition into FatalExceptionThrown state and
                // do not allow any further writes.
                this.SetState(BatchWriterState.FatalExceptionThrown);
                throw;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
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
                    this.changeSetBoundary = ODataBatchWriterUtils.CreateChangeSetBoundary(this.writingResponse);
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
                case BatchWriterState.FatalExceptionThrown:
                    if (newState != BatchWriterState.FatalExceptionThrown)
                    {
                        throw new ODataException(Strings.ODataBatchWriter_InvalidTransitionFromFatalExceptionThrown);
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
                Debug.Assert(this.batchWriter != null, "Must have a batch writer if pending data exists.");

                if (this.CurrentOperationResponseMessage != null)
                {
                    Debug.Assert(this.writingResponse, "If the response message is available we must be writing response.");
                    int statusCode = this.CurrentOperationResponseMessage.StatusCode;
                    string statusMessage = HttpUtils.GetStatusMessage(statusCode);
                    this.batchWriter.WriteLine("{0} {1} {2}", HttpConstants.HttpVersionInBatching, statusCode, statusMessage);
                }

                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        this.batchWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", headerPair.Key, headerPair.Value));
                    }
                }

                // write CRLF after the headers (or request/response line if there are no headers)
                this.batchWriter.WriteLine();

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.WriteMessageDataCompleted();
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
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.changeSetBoundary);
            }
        }

        /// <summary>
        /// Sets the 'FatalExceptionThrown' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.SetState(BatchWriterState.FatalExceptionThrown);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseBatchSize()
        {
            this.currentBatchSize++;

            if (this.currentBatchSize > this.settings.MaxBatchSize)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxBatchSizeExceeded(this.settings.MaxBatchSize));
            }
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseChangeSetSize()
        {
            this.currentChangeSetSize++;

            if (this.currentChangeSetSize > this.settings.MaxChangesetSize)
            {
                throw new ODataException(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(this.settings.MaxChangesetSize));
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
