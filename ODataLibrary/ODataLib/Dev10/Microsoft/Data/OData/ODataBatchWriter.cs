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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for writing OData batch messages; also verifies the proper sequence of write calls on the writer.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:ImplementIDisposable", Justification = "IDisposable is implemented on ODataMessageWriter.")]
    public sealed class ODataBatchWriter : IODataBatchOperationListener, IODataWriter
    {
        /// <summary>The encoding to use to write everything except for the operation payloads (e.g., the batch boundaries, headers, etc.)</summary>
        private readonly Encoding encoding;

        /// <summary>A flag indicating whether we are writing a request or a response message.</summary>
        private readonly bool writingResponse;

        /// <summary>The boundary string for the batch structure itself.</summary>
        private readonly string batchBoundary;

        /// <summary>The <see cref="ODataMessageWriterSettings"/> for the batch writer.</summary>
        private readonly ODataMessageWriterSettings settings;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>The batch-specific URL resolver that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        private readonly ODataBatchUrlResolver urlResolver;

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

        /// <summary>
        /// A flags to indicate whether the current changeset start boundary has been written or not.
        /// This is false if a changeset has been started by no changeset boundary was written, and true once the first changeset
        /// boundary for the current changeset has been written.
        /// </summary>
        private bool changesetStartBoundaryWritten;

        /// <summary>The request message for the operation that is currently written if it's a request; or null if no part is written right now or it's a response part.</summary>
        private ODataBatchOperationRequestMessage currentOperationRequestMessage;

        /// <summary>The response message for the operation that is currently written if it's a request; or null if no part is written right now or it's a request part.</summary>
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
        /// <param name="stream">The stream to write to.</param>
        /// <param name="settings">The <see cref="ODataMessageWriterSettings"/> for the batch writer.</param>
        /// <param name="encoding">The encoding to use for writing.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a request or a response message.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        private ODataBatchWriter(
            Stream stream, 
            ODataMessageWriterSettings settings, 
            Encoding encoding, 
            bool writingResponse, 
            string batchBoundary, 
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(settings != null, "settings != null");

            ExceptionUtils.CheckArgumentNotNull(batchBoundary, "batchBoundary");

            this.settings = settings;
            this.encoding = encoding;
            this.outputStream = stream;
            this.writingResponse = writingResponse;
            this.batchBoundary = batchBoundary;
            this.synchronous = synchronous;
            this.urlResolver = new ODataBatchUrlResolver();
            this.asyncBufferedStream = new AsyncBufferedStream(stream);
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

            /// <summary>The writer is in error state; nothing can be written anymore except the error payload.</summary>
            Error
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
            this.VerifyCanWriteStartBatch(true);
            this.WriteStartBatchImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously starts a new batch; can be only called once and as first call.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartBatchAsync()
        {
            this.VerifyCanWriteStartBatch(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteStartBatchImplementation);
        }
#endif

        /// <summary>
        /// Ends a batch; can only be called after WriteStartBatch has been called and if no other active changeset or operation exist.
        /// </summary>
        public void WriteEndBatch()
        {
            this.VerifyCanWriteEndBatch(true);
            this.WriteEndBatchImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously ends a batch; can only be called after WriteStartBatch has been called and if no other active changeset or operation exist.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndBatchAsync()
        {
            this.VerifyCanWriteEndBatch(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndBatchImplementation);
        }
#endif

        /// <summary>
        /// Starts a new changeset; can only be called after WriteStartBatch and if no other active operation or changeset exists.
        /// </summary>
        public void WriteStartChangeset()
        {
            this.VerifyCanWriteStartChangeset(true);
            this.WriteStartChangesetImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously starts a new changeset; can only be called after WriteStartBatch and if no other active operation or changeset exists.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteStartChangesetAsync()
        {
            this.VerifyCanWriteStartChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteStartChangesetImplementation);
        }
#endif

        /// <summary>
        /// Ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.
        /// </summary>
        public void WriteEndChangeset()
        {
            this.VerifyCanWriteEndChangeset(true);
            this.WriteEndChangesetImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously ends an active changeset; this can only be called after WriteStartChangeset and only once for each changeset.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public Task WriteEndChangesetAsync()
        {
            this.VerifyCanWriteEndChangeset(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndChangesetImplementation);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage(HttpMethod method, Uri uri)
        {
            this.VerifyCanCreateOperationRequestMessage(true, method, uri);
            return this.CreateOperationRequestMessageImplementation(method, uri);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <returns>A task that when completed returns the newly created operation request message.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync(HttpMethod method, Uri uri)
        {
            this.VerifyCanCreateOperationRequestMessage(false, method, uri);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                () => this.CreateOperationRequestMessageImplementation(method, uri));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <returns>The message that can be used to write the response operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            this.VerifyCanCreateOperationResponseMessage(true);
            return this.CreateOperationResponseMessageImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response.
        /// </summary>
        /// <returns>A task that when completed returns the newly created operation response message.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
        {
            this.VerifyCanCreateOperationResponseMessage(false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(
                this.CreateOperationResponseMessageImplementation);
        }
#endif

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
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

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public Task FlushAsync()
        {
            this.VerifyCanFlush(false);
            
            // make sure we switch to state FatalExceptionThrown if an exception is thrown during flushing.
            return this.FlushAsynchronously().ContinueWithOnFault(t => this.SetState(BatchWriterState.Error));
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamRequested()
        {
            this.StartBatchOperationContent();
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
        /// Synchronously flushes the write buffer to the underlying stream.
        /// </summary>
        void IODataWriter.FlushWriter()
        {
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        Task IODataWriter.FlushWriterAsync()
        {
            return this.FlushAsync();
        }
#endif

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        void IODataWriter.WriteError(ODataError errorInstance, bool includeDebugInformation)
        {
            this.VerifyNotDisposed();
            this.SetState(BatchWriterState.Error);
            this.batchWriter.Flush();

            // TODO: jli - Need to remove this method after the code refactoring.
            // The OData protocol spec did not defined the behavior when an exception is encountered outside of a batch operation. The batch writer
            // should not allow WriteError in this case. Note that Astoria do serialize the error in XML format when it encounters one outside of a
            // batch operation.
            throw Error.NotSupported();
        }

        /// <summary>
        /// This method will be called by ODataMessageWriter.Dispose() to dispose the object implementing this interface.
        /// </summary>
        void IODataWriter.DisposeWriter()
        {
            this.VerifyNotDisposed();
            this.outputStream = null;

            //// NOTE: we do not check the writer state here (to require it to be either an error state or start or completed)
            ////       because the writer can be disposed due to an exception in user code. In that case we still have to flush any pending data.

            try
            {
                // flush any active text writer to the underlying stream so we guarantee that there is no data buffered in the text writer;
                // the underlying async buffered stream will ignore the call to Dispose() that is triggered by disposing this StreamWriter.
                if (this.batchWriter != null)
                {
                    this.batchWriter.Flush();
                }

                // In synchronous mode we always flush the underlying stream (synchronously).
                // In asynchronous mode we discard any buffered data that has not been flushed before Dispose was called.
                if (this.synchronous)
                {
                    this.asyncBufferedStream.FlushSync();
                }
                else
                {
                    this.asyncBufferedStream.Clear();
                }

                if (this.batchWriter != null)
                {
                    this.batchWriter.Dispose();
                }

                this.asyncBufferedStream.Dispose();
            }
            finally
            {
                this.batchWriter = null;
                this.asyncBufferedStream = null;
            }
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
            ODataMessageWriterSettings settings,
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
        /// <param name="settings">Message writer settings to use.</param>
        /// <param name="writingResponse">A flag indicating whether we are writing a request or a response message.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>The newly created <see cref="ODataBatchWriter"/> instance.</returns>
        private static ODataBatchWriter CreateBatchWriter(
            string batchBoundary, 
            Stream stream, 
            ODataMessageWriterSettings settings, 
            bool writingResponse,
            bool synchronous)
        {
            Debug.Assert(settings.BaseUri == null || settings.BaseUri.IsAbsoluteUri, "We should have validated that BaseUri is absolute.");

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
        /// Starts a new batch - implementation of the actual functionality.
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
        /// Ends a batch - implementation of the actual functionality.
        /// </summary>
        private void WriteEndBatchImplementation()
        {
            // write the start boundary for the batch if not written
            if (!this.batchStartBoundaryWritten)
            {
                Debug.Assert(this.CurrentOperationMessage == null, "If not batch boundary was written we must not have an active message.");
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary, true);
            }

            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            this.SetState(BatchWriterState.BatchCompleted);

            // write the end boundary for the batch
            ODataBatchWriterUtils.WriteEndBoundary(this.batchWriter, this.batchBoundary);

            // TODO: Review this once we know if we will have an internal WCF DS knob - if we would then we should
            // probably do this only if the knob is on.
            // For compatibility with WCF DS we write a newline after the end batch boundary.
            // Technically it's not needed, but it doesn't violate anything either.
            this.batchWriter.WriteLine();
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
        /// Starts a new changeset - implementation of the actual functionality.
        /// </summary>
        private void WriteStartChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            // important to do this first since it will set up the change set boundary.
            this.SetState(BatchWriterState.ChangeSetStarted);
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // reset the size of the current changeset and increase the size of the batch
            this.ResetChangeSetSize();
            this.InterceptException(this.IncreaseBatchSize);

            // write the boundary string
            ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
            this.batchStartBoundaryWritten = true;

            // write the change set headers
            ODataBatchWriterUtils.WriteChangeSetPreamble(this.batchWriter, this.changeSetBoundary);
            this.changesetStartBoundaryWritten = false;
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
        /// Ends an active changeset - implementation of the actual functionality.
        /// </summary>
        private void WriteEndChangesetImplementation()
        {
            // write pending message data (headers, response line) for a previously unclosed message/request
            this.WritePendingMessageData(true);

            string currentChangeSetBoundary = this.changeSetBoundary;

            // change the state first so we validate the change set boundary before attempting to write it.
            this.SetState(BatchWriterState.ChangeSetCompleted);

            // write the start changeset if we haven't written it yet (empty changeset case)
            if (!this.changesetStartBoundaryWritten)
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, currentChangeSetBoundary, true);
            }

            // write the end boundary for the change set
            ODataBatchWriterUtils.WriteEndBoundary(this.batchWriter, currentChangeSetBoundary);

            // Reset the cache of content IDs here. As per spec, content IDs are only unique inside a change set
            this.urlResolver.Reset();
            this.currentOperationContentId = null;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        private void VerifyCanCreateOperationRequestMessage(bool synchronousCall, HttpMethod method, Uri uri)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);

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
            }
            else
            {
                // allow all methods except for GET
                if (method == HttpMethod.Get)
                {
                    this.ThrowODataException(Strings.ODataBatchWriter_InvalidHttpMethodForChangeSetRequest(method.ToText()));
                }
            }

            ExceptionUtils.CheckArgumentNotNull(uri, "uri");
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationRequestMessage"/> for writing an operation of a batch request - implementation of the actual functionality.
        /// </summary>
        /// <param name="method">The Http method to be used for this request operation.</param>
        /// <param name="uri">The Uri to be used for this request operation.</param>
        /// <returns>The message that can be used to write the request operation.</returns>
        private ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(HttpMethod method, Uri uri)
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
            if (this.currentOperationContentId != null)
            {
                this.urlResolver.AddContentId(this.currentOperationContentId);
            }

            this.InterceptException(() => uri = ODataBatchWriterUtils.CreateOperationRequestUri(uri, this.settings.BaseUri, this.urlResolver));

            // create the new request operation
            this.CurrentOperationRequestMessage = new ODataBatchOperationRequestMessage(this.outputStream, method, uri, /*operationListener*/ this, this.urlResolver);
            this.SetState(BatchWriterState.OperationCreated);

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request line
            ODataBatchWriterUtils.WriteRequestPreamble(this.batchWriter, method, uri);

            return this.CurrentOperationRequestMessage;
        }

        /// <summary>
        /// Verifies that calling CreateOperationResponseMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
        {
            this.ValidateWriterReady();
            this.VerifyCallAllowed(synchronousCall);
            
            if (!this.writingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchOperationResponseMessage"/> for writing an operation of a batch response - implementation of the actual functionality.
        /// </summary>
        /// <returns>The message that can be used to write the response operation.</returns>
        private ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
        {
            this.WritePendingMessageData(true);

            this.CurrentOperationResponseMessage = new ODataBatchOperationResponseMessage(this.outputStream, this);
            this.SetState(BatchWriterState.OperationCreated);

            Debug.Assert(this.currentOperationContentId == null, "The Content-ID header is only supported in request messages.");

            // write the operation's start boundary string
            this.WriteStartBoundaryForOperation();

            // write the headers and request separator line
            ODataBatchWriterUtils.WriteResponsePreamble(this.batchWriter);

            return this.CurrentOperationResponseMessage;
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
            this.asyncBufferedStream.FlushSync();
            this.batchWriter.Dispose();
            this.batchWriter = null;

            this.SetState(BatchWriterState.OperationStreamRequested);
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
            Debug.Assert(this.currentOperationRequestMessage != null, "this.currentOperationRequestMessage != null");
            Debug.Assert(this.changeSetBoundary != null, "this.changeSetBoundary != null");

            // Set the current content ID. If no Content-ID header is found in the message,
            // the 'contentId' argument will be null and this will reset the current operation content ID field.
            this.currentOperationContentId = contentId;

            // Check for duplicate content IDs; we have to do this here instead of in the cache itself
            // since the content ID of the last operation never gets added to the cache but we still
            // want to fail on the duplicate.
            if (contentId != null && this.urlResolver.ContainsContentId(contentId))
            {
                throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(contentId));
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
        /// Verifies that a call is allowed to the writer.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchWriter_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if ODATALIB_ASYNC
                if (this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchWriter_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

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

                // NOTE: Content-ID headers are only valid in operations in a request changeset
                bool rememberContentId = this.CurrentOperationRequestMessage != null && this.changeSetBoundary != null;
                string contentId = null;
                IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> headerPair in headers)
                    {
                        string headerName = headerPair.Key;
                        string headerValue = headerPair.Value;
                        this.batchWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", headerName, headerValue));

                        // Remember the Content-ID header (for operations in a request changeset)
                        if (rememberContentId && string.CompareOrdinal(HttpConstants.ContentId, headerName) == 0)
                        {
                            contentId = headerValue;
                        }
                    }
                }

                if (rememberContentId)
                {
                    this.RememberContentIdHeader(contentId);
                }

                // write CRLF after the headers (or request/response line if there are no headers)
                this.batchWriter.WriteLine();

                if (reportMessageCompleted)
                {
                    this.CurrentOperationMessage.WriteMessageDataCompleted();
                    this.CurrentOperationRequestMessage = null;
                    this.CurrentOperationResponseMessage = null;
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
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.batchBoundary, !this.batchStartBoundaryWritten);
                this.batchStartBoundaryWritten = true;
            }
            else
            {
                ODataBatchWriterUtils.WriteStartBoundary(this.batchWriter, this.changeSetBoundary, !this.changesetStartBoundaryWritten);
                this.changesetStartBoundaryWritten = true;
            }
        }

        /// <summary>
        /// Sets the 'FatalExceptionThrown' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.SetState(BatchWriterState.Error);
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
