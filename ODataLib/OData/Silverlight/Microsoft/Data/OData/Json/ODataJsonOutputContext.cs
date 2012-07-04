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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// JSON format output context.
    /// </summary>
    internal sealed class ODataJsonOutputContext : ODataOutputContext
    {
        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;

        /// <summary>The text writer created for the output stream.</summary>
        private TextWriter textWriter;

        /// <summary>The JSON writer to write to.</summary>
        /// <remarks>This field is also used to determine if the output context has been disposed already.</remarks>
        private JsonWriter jsonWriter;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="synchronous">true if the output should be written synchronously; false if it should be written asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        private ODataJsonOutputContext(
            ODataFormat format,
            JsonWriter jsonWriter,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageWriterSettings, writingResponse, synchronous, model, urlResolver)
        {
            this.jsonWriter = jsonWriter;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageStream">The message stream to write the payload to.</param>
        /// <param name="encoding">The encoding to use for the payload.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="synchronous">true if the output should be written synchronously; false if it should be written asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        private ODataJsonOutputContext(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageWriterSettings, writingResponse, synchronous, model, urlResolver)
        {
            try
            {
                this.messageOutputStream = messageStream;

                Stream outputStream;
                if (synchronous)
                {
                    outputStream = messageStream;
                }
                else
                {
                    this.asynchronousOutputStream = new AsyncBufferedStream(messageStream);
                    outputStream = this.asynchronousOutputStream;
                }

                this.textWriter = new StreamWriter(outputStream, encoding);

                this.jsonWriter = new JsonWriter(this.textWriter, messageWriterSettings.Indent);
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && messageStream != null)
                {
                    messageStream.Dispose();
                }

                throw;
            }
        }

        /// <summary>
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal JsonWriter JsonWriter
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.jsonWriter != null, "Trying to get JsonWriter while none is available.");
                return this.jsonWriter;
            }
        }

        /// <summary>
        /// Create JSON output context.
        /// </summary>
        /// <param name="format">The format to create the output context for.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>The newly created output context.</returns>
        internal static ODataOutputContext Create(
            ODataFormat format,
            ODataMessage message, 
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(format == ODataFormat.VerboseJson, "This method only supports the JSON format.");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            Stream messageStream = message.GetStream();
            return new ODataJsonOutputContext(
                format,
                messageStream,
                encoding,
                messageWriterSettings,
                writingResponse,
                true,
                model,
                urlResolver);
        }

        /// <summary>
        /// Create JSON output context.
        /// </summary>
        /// <param name="format">The format to create the output context for.</param>
        /// <param name="jsonWriter">The JSON writer to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>The newly created output context.</returns>
        /// <remarks>This method is for writing the output directly to the JSON writer, when created with this method
        /// the output context doesn't support most of the methods on it. Note that Dispose will do nothing in this case.</remarks>
        internal static ODataJsonOutputContext Create(
            ODataFormat format,
            JsonWriter jsonWriter,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(format == ODataFormat.VerboseJson, "This method only supports the verbose JSON format.");
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            return new ODataJsonOutputContext(
                format,
                jsonWriter,
                messageWriterSettings,
                writingResponse,
                true,
                model,
                urlResolver);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create JSON output context.
        /// </summary>
        /// <param name="format">The format to create the outpu context for.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <returns>Task which when completed returns the newly created output context.</returns>
        internal static Task<ODataOutputContext> CreateAsync(
            ODataFormat format,
            ODataMessage message,
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(format == ODataFormat.VerboseJson, "This method only supports the JSON format.");
            Debug.Assert(message != null, "message != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            return message.GetStreamAsync()
                .FollowOnSuccessWith(
                    (streamTask) => (ODataOutputContext)new ODataJsonOutputContext(
                        format,
                        streamTask.Result,
                        encoding,
                        messageWriterSettings,
                        writingResponse,
                        false,
                        model,
                        urlResolver));
        }
#endif

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        internal void VerifyNotDisposed()
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.messageOutputStream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        internal void Flush()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            // JsonWriter.Flush will call the underlying TextWriter.Flush.
            // The TextWriter.Flush (which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
            // In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
            this.jsonWriter.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flush the writer.
        /// </summary>
        /// <returns>Task which represents the pending flush operation.</returns>
        /// <remarks>The method should not throw directly if the flush operation itself fails, it should instead return a faulted task.</remarks>
        internal Task FlushAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    // In the async case the underlying stream is the async buffered stream, which ignores Flush call.
                    this.jsonWriter.Flush();

                    Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
                    return this.asynchronousOutputStream.FlushAsync();
                })
                .FollowOnSuccessWithTask((asyncBufferedStreamFlushTask) => this.messageOutputStream.FlushAsync());
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataError"/> into the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>
        /// This method is called if the ODataMessageWriter.WriteError is called once some other
        /// write operation has already started.
        /// The method should write the in-stream error representation for the specific format into the current payload.
        /// Before the method is called no flush is performed on the output context or any active writer.
        /// It is the responsibility of this method to flush the output before the method returns.
        /// </remarks>
        internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteInStreamErrorImplementation(error, includeDebugInformation);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Writes an <see cref="ODataError"/> into the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>Task which represents the pending write operation.</returns>
        /// <remarks>
        /// This method is called if the ODataMessageWriter.WriteError is called once some other
        /// write operation has already started.
        /// The method should write the in-stream error representation for the specific format into the current payload.
        /// Before the method is called no flush is performed on the output context or any active writer.
        /// It is the responsibility of this method to make sure that all the data up to this point are written before
        /// the in-stream error is written.
        /// It is the responsibility of this method to flush the output before the task finishes.
        /// </remarks>
        internal override Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteInStreamErrorImplementation(error, includeDebugInformation);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataFeedWriter()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataFeedWriterImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataWriter> CreateODataFeedWriterAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataFeedWriterImplementation());
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataEntryWriter()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataEntryWriterImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataWriter> CreateODataEntryWriterAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataEntryWriterImplementation());
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataCollectionWriter CreateODataCollectionWriter()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataCollectionWriterImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>A running task for the created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataCollectionWriter> CreateODataCollectionWriterAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataCollectionWriterImplementation());
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataParameterWriter CreateODataParameterWriter(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataParameterWriterImplementation(functionImport);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataParameterWriterImplementation(functionImport));
        }
#endif

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteServiceDocumentImplementation(defaultWorkspace);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteServiceDocumentAsync(ODataWorkspace defaultWorkspace)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteServiceDocumentImplementation(defaultWorkspace);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteProperty(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WritePropertyImplementation(property);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A task representing the asynchronous operation of writing the property.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WritePropertyAsync(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WritePropertyImplementation(property);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteError(ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteErrorImplementation(error, includeDebugInformation);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>A task representing the asynchronous operation of writing the error.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteErrorImplementation(error, includeDebugInformation);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteEntityReferenceLinksImplementation(links);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinksImplementation(links);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteEntityReferenceLinkImplementation(link);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinkImplementation(link);
                    return this.FlushAsync();
                });
        }
#endif

        //// Raw value is not supported by JSON.

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "textWriter", Justification = "We don't dispose the jsonWriter or textWriter, instead we dispose the underlying stream directly.")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                if (this.messageOutputStream != null)
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    this.jsonWriter.Flush();

                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitely.
                    if (this.asynchronousOutputStream != null)
                    {
                        this.asynchronousOutputStream.FlushSync();
                        this.asynchronousOutputStream.Dispose();
                    }

                    // Dipose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.textWriter = null;
                this.jsonWriter = null;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataFeedWriterImplementation()
        {
            ODataJsonWriter odataJsonWriter = new ODataJsonWriter(this, true);
            this.outputInStreamErrorListener = odataJsonWriter;
            return odataJsonWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataEntryWriterImplementation()
        {
            ODataJsonWriter odataJsonWriter = new ODataJsonWriter(this, false);
            this.outputInStreamErrorListener = odataJsonWriter;
            return odataJsonWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection writer.</returns>
        private ODataCollectionWriter CreateODataCollectionWriterImplementation()
        {
            ODataJsonCollectionWriter jsonCollectionWriter = new ODataJsonCollectionWriter(this, /*expectedItemType*/ null, /*listener*/ null);
            this.outputInStreamErrorListener = jsonCollectionWriter;
            return jsonCollectionWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        private ODataParameterWriter CreateODataParameterWriterImplementation(IEdmFunctionImport functionImport)
        {
            ODataJsonParameterWriter jsonParameterWriter = new ODataJsonParameterWriter(this, functionImport);
            this.outputInStreamErrorListener = jsonParameterWriter;
            return jsonParameterWriter;
        }

        /// <summary>
        /// Writes an in-stream error.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        private void WriteInStreamErrorImplementation(ODataError error, bool includeDebugInformation)
        {
            if (this.outputInStreamErrorListener != null)
            {
                this.outputInStreamErrorListener.OnInStreamError();
            }

            ODataJsonWriterUtils.WriteError(this.jsonWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataJsonPropertyAndValueSerializer jsonPropertyAndValueSerializer = new ODataJsonPropertyAndValueSerializer(this);
            jsonPropertyAndValueSerializer.WriteTopLevelProperty(property);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        private void WriteServiceDocumentImplementation(ODataWorkspace defaultWorkspace)
        {
            ODataJsonServiceDocumentSerializer jsonServiceDocumentSerializer = new ODataJsonServiceDocumentSerializer(this);
            jsonServiceDocumentSerializer.WriteServiceDocument(defaultWorkspace);
        }

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        private void WriteErrorImplementation(ODataError error, bool includeDebugInformation)
        {
            ODataJsonSerializer jsonSerializer = new ODataJsonSerializer(this);
            jsonSerializer.WriteTopLevelError(error, includeDebugInformation);
        }

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            jsonEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links);
        }

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            jsonEntityReferenceLinkSerializer.WriteEntityReferenceLink(link);
        }
    }
}
