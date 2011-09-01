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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Base class for all input contexts, defines the interface 
    /// to be implemented by the specific formats.
    /// </summary>
    internal abstract class ODataInputContext : IDisposable
    {
        /// <summary>The message reader settings to be used for reading.</summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>The protocol version to use when reading the payload.</summary>
        private readonly ODataVersion version;

        /// <summary>Set to true if this reader is reading a response payload.</summary>
        private readonly bool readingResponse;

        /// <summary>true if the input should be read synchronously; false if it should be read asynchronously.</summary>
        private readonly bool synchronous;

        /// <summary>The model to use.</summary>
        private readonly IEdmModel model;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs read from the payload.</summary>
        private readonly IODataUrlResolver urlResolver;

        /// <summary>Set to true if the input was disposed.</summary>
        private bool disposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        protected ODataInputContext(
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            this.messageReaderSettings = messageReaderSettings;
            this.version = version;
            this.readingResponse = readingResponse;
            this.synchronous = synchronous;
            this.model = model;
            this.urlResolver = urlResolver;
        }

        /// <summary>
        /// The message reader settings to be used for reading.
        /// </summary>
        internal ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.messageReaderSettings;
            }
        }

        /// <summary>
        /// The version of the OData protocol to use.
        /// </summary>
        internal ODataVersion Version
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.version;
            }
        }

        /// <summary>
        /// Set to true if a response is being read.
        /// </summary>
        internal bool ReadingResponse
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.readingResponse;
            }
        }

        /// <summary>
        /// true if the input should be read synchronously; false if it should be read asynchronously.
        /// </summary>
        internal bool Synchronous
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.synchronous;
            }
        }

        /// <summary>
        /// The model to use or null if no metadata is available.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.model;
            }
        }

        /// <summary>
        /// The optional URL resolver to perform custom URL resolution for URLs read from the payload.
        /// </summary>
        internal IODataUrlResolver UrlResolver
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.urlResolver;
            }
        }

        /// <summary>
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the writer.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates an instance of the input context for the specified format.
        /// </summary>
        /// <param name="format">The format to create the context for.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <returns>The newly created input context.</returns>
        internal static ODataInputContext CreateInputContext(
            ODataFormat format,
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (format)
            {
                case ODataFormat.Atom:
                    return ODataAtomInputContext.Create(
                        message,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        model,
                        urlResolver);

                case ODataFormat.Json:
                    return ODataJsonInputContext.Create(
                        message,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        model,
                        urlResolver);

                case ODataFormat.Default:
                    if (readerPayloadKind == ODataPayloadKind.MetadataDocument)
                    {
                        return ODataMetadataInputContext.Create(
                            message,
                            encoding,
                            messageReaderSettings,
                            version,
                            readingResponse,
                            model,
                            urlResolver);
                    }
                    else
                    {
                        return ODataRawInputContext.Create(
                            message,
                            encoding,
                            messageReaderSettings,
                            version,
                            readingResponse,
                            model,
                            urlResolver,
                            readerPayloadKind);
                    }

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataInputContext_CreateInputContextForStream_UnrecognizedFormat));
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an instance of the input context for the specified format.
        /// </summary>
        /// <param name="format">The format to create the context for.</param>
        /// <param name="readerPayloadKind">The <see cref="ODataPayloadKind"/> to read.</param>
        /// <param name="message">The message to use.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <returns>Task which when completed returned the newly created input context.</returns>
        internal static Task<ODataInputContext> CreateInputContextAsync(
            ODataFormat format,
            ODataPayloadKind readerPayloadKind,
            ODataMessage message,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            IEdmModel model,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (format)
            {
                case ODataFormat.Atom:
                    return ODataAtomInputContext.CreateAsync(
                        message,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        model,
                        urlResolver);

                case ODataFormat.Json:
                    return ODataJsonInputContext.CreateAsync(
                        message,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        model,
                        urlResolver);

                case ODataFormat.Default:
                    Debug.Assert(readerPayloadKind != ODataPayloadKind.MetadataDocument, "Async reading of metadata documents is not supported.");
                    return ODataRawInputContext.CreateAsync(
                        message,
                        encoding,
                        messageReaderSettings,
                        version,
                        readingResponse,
                        model,
                        urlResolver,
                        readerPayloadKind);

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataInputContext_CreateInputContextForStream_UnrecognizedFormat));
            }
        }
#endif

        /// <summary>
        /// Synchronously get the stream backing the message.
        /// </summary>
        /// <param name="message">The message to get the stream for.</param>
        /// <param name="disableMessageStreamDisposal">true if the stream returned should ignore dispose calls.</param>
        /// <returns>The message stream to use - this stream should always be disposed, regardless of any setting.</returns>
        internal static Stream GetMessageStream(ODataMessage message, bool disableMessageStreamDisposal)
        {
            DebugUtils.CheckNoExternalCallers();

            Stream stream = message.GetStream();
            if (disableMessageStreamDisposal)
            {
                stream = new NonDisposingStream(stream);
            }

            return stream;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing the message.
        /// </summary>
        /// <param name="message">The message to get the stream for.</param>
        /// <param name="disableMessageStreamDisposal">true if the stream returned should ignore dispose calls.</param>
        /// <returns>A task which returns the message stream to use - this stream should always be disposed, regardless of any setting.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by the caller.")]
        internal static Task<Stream> GetMessageStreamAsync(ODataMessage message, bool disableMessageStreamDisposal)
        {
            DebugUtils.CheckNoExternalCallers();

            // NOTE it is important to run all child tasks with the 'AttachedToParent' option to make
            //      sure all exceptions are bubbled up to the parent task (in particular this is also true to the child tasks created in BufferStream).
            return message.GetStreamAsync().ContinueWith(
                streamTask =>
                {
                    Stream stream = streamTask.Result;
                    if (disableMessageStreamDisposal)
                    {
                        stream = new NonDisposingStream(stream);
                    }

                    return BufferedReadStream.BufferStreamAsync(stream);
                },
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent)

            // TODO: review how exceptions are handled by Unwrap
            .Unwrap()
            .ContinueWith(
                readStreamTask =>
                {
                    BufferedReadStream bufferedReadStream = readStreamTask.Result;
                    return (Stream)bufferedReadStream;
                },
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal abstract ODataReader CreateFeedReader(IEdmEntityType expectedBaseEntityType);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal abstract Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedBaseEntityType);
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal abstract ODataReader CreateEntryReader(IEdmEntityType expectedEntityType);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal abstract Task<ODataReader> CreateEntryReaderAsync(IEdmEntityType expectedEntityType);
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        internal abstract ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        internal abstract Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference);
#endif

        //// TODO: Do we really want to allow more than one batch format? If so we will need to make ODataBatchReader abstract
        //// and implement our own implementation on top of it (just like ODataReader).
        //// If not, this method should be kept internal, or maybe even not made abstract in the first place.

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>The newly created <see cref="ODataBatchReader"/>.</returns>
        internal abstract ODataBatchReader CreateBatchReader(string batchBoundary);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <param name="batchBoundary">The batch boundary to use.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataBatchReader"/>.</returns>
        internal abstract Task<ODataBatchReader> CreateBatchReaderAsync(string batchBoundary);
#endif

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal abstract ODataWorkspace ReadServiceDocument();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal abstract Task<ODataWorkspace> ReadServiceDocumentAsync();
#endif

        /// <summary>
        /// Read a metadata document. 
        /// This method reads the metadata document from the input and returns 
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal abstract IEdmModel ReadMetadataDocument();

        /// <summary>
        /// Read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal abstract ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal abstract Task<ODataProperty> ReadPropertyAsync(IEdmTypeReference expectedPropertyTypeReference);
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal abstract ODataError ReadError();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal abstract Task<ODataError> ReadErrorAsync();
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal abstract ODataEntityReferenceLinks ReadEntityReferenceLinks();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal abstract Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync();
#endif

        /// <summary>
        /// Read a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal abstract ODataEntityReferenceLink ReadEntityReferenceLink();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal abstract Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync();
#endif

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal abstract object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference);

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>Task which when completed returns an <see cref="object"/> representing the read value.</returns>
        internal abstract Task<object> ReadValueAsync(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference);
#endif

        /// <summary>
        /// Check if the object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the object has already been disposed.</exception>
        internal void VerifyNotDisposed()
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Asserts that the input context was created for synchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertSynchronous()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            Debug.Assert(this.synchronous, "The method should only be called on a synchronous input context.");
#endif
        }

        /// <summary>
        /// Asserts that the input context was created for asynchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertAsynchronous()
        {
            DebugUtils.CheckNoExternalCallers();

#if DEBUG
            Debug.Assert(!this.synchronous, "The method should only be called on an asynchronous input context.");
#endif
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker()
        {
            DebugUtils.CheckNoExternalCallers();

            return new DuplicatePropertyNamesChecker(this.MessageReaderSettings.ReaderBehavior.AllowDuplicatePropertyNames, this.ReadingResponse);
        }

        /// <summary>
        /// Disposes the input context.
        /// </summary>
        protected abstract void DisposeImplementation();

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        private void Dispose(bool disposing)
        {
            this.disposed = true;
            if (disposing)
            {
                this.DisposeImplementation();
            }
        }

        /// <summary>
        /// Private stream wrapper for the message stream to ignore the Stream.Dispose method so that readers on top of
        /// it can be disposed without affecting it.
        /// </summary>
        private sealed class NonDisposingStream : Stream
        {
            /// <summary>
            /// Stream that is being wrapped.
            /// </summary>
            private readonly Stream innerStream;

            /// <summary>
            /// Constructs an instance of the stream wrapper class.
            /// </summary>
            /// <param name="innerStream">Stream that is being wrapped.</param>
            public NonDisposingStream(Stream innerStream)
            {
                Debug.Assert(innerStream != null, "innerStream != null");
                this.innerStream = innerStream;
            }

            /// <summary>
            /// Determines if the stream can read.
            /// </summary>
            public override bool CanRead
            {
                get { return this.innerStream.CanRead; }
            }

            /// <summary>
            /// Determines if the stream can seek.
            /// </summary>
            public override bool CanSeek
            {
                get { return this.innerStream.CanSeek; }
            }

            /// <summary>
            /// Determines if the stream can write.
            /// </summary>
            public override bool CanWrite
            {
                get { return this.innerStream.CanWrite; }
            }

            /// <summary>
            /// Returns the length of the stream.
            /// </summary>
            public override long Length
            {
                get { return this.innerStream.Length; }
            }

            /// <summary>
            /// Gets or sets the position in the stream.
            /// </summary>
            public override long Position
            {
                get { return this.innerStream.Position; }
                set { this.innerStream.Position = value; }
            }

            /// <summary>
            /// Flush the stream to the underlying storage.
            /// </summary>
            public override void Flush()
            {
                this.innerStream.Flush();
            }

            /// <summary>
            /// Reads data from the stream.
            /// </summary>
            /// <param name="buffer">The buffer to read the data to.</param>
            /// <param name="offset">The offset in the buffer to write to.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <returns>The number of bytes actually read.</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.innerStream.Read(buffer, offset, count);
            }

            /// <summary>
            /// Begins a read operation from the stream.
            /// </summary>
            /// <param name="buffer">The buffer to read the data to.</param>
            /// <param name="offset">The offset in the buffer to write to.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="callback">The async callback.</param>
            /// <param name="state">The async state.</param>
            /// <returns>Async result representing the asynchornous operation.</returns>
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.innerStream.BeginRead(buffer, offset, count, callback, state);
            }

            /// <summary>
            /// Ends a read operation from the stream.
            /// </summary>
            /// <param name="asyncResult">The async result representing the read operation.</param>
            /// <returns>The number of bytes actually read.</returns>
            public override int EndRead(IAsyncResult asyncResult)
            {
                return this.innerStream.EndRead(asyncResult);
            }

            /// <summary>
            /// Seeks the stream.
            /// </summary>
            /// <param name="offset">The offset to seek to.</param>
            /// <param name="origin">The origin of the seek operation.</param>
            /// <returns>The new position in the stream.</returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.innerStream.Seek(offset, origin);
            }

            /// <summary>
            /// Sets the length of the stream.
            /// </summary>
            /// <param name="value">The length in bytes to set.</param>
            public override void SetLength(long value)
            {
                this.innerStream.SetLength(value);
            }

            /// <summary>
            /// Writes to the stream.
            /// </summary>
            /// <param name="buffer">The buffer to get data from.</param>
            /// <param name="offset">The offset in the buffer to start from.</param>
            /// <param name="count">The number of bytes to write.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.innerStream.Write(buffer, offset, count);
            }

            /// <summary>
            /// Begins an asynchronous write operation to the stream.
            /// </summary>
            /// <param name="buffer">The buffer to get data from.</param>
            /// <param name="offset">The offset in the buffer to start from.</param>
            /// <param name="count">The number of bytes to write.</param>
            /// <param name="callback">The async callback.</param>
            /// <param name="state">The async state.</param>
            /// <returns>Async result representing the write operation.</returns>
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
            }

            /// <summary>
            /// Ends the asynchronous write operation.
            /// </summary>
            /// <param name="asyncResult">Async result representing the write operation.</param>
            public override void EndWrite(IAsyncResult asyncResult)
            {
                this.innerStream.EndWrite(asyncResult);
            }

            /// <summary>
            /// Release unmanaged resources.
            /// </summary>
            /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
            /// <remarks>This method doesn't dispose the inner stream.</remarks>
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
            }
        }
    }
}
