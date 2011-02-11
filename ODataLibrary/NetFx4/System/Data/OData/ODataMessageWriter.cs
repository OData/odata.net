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
    using System.Data.OData.Atom;
    using System.Data.OData.Json;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    #endregion Namespaces.

    /// <summary>
    /// Writer class used to write all OData payloads (entries, feeds, metadata documents, service documents, etc.).
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ODataMessageWriter
#else
    public sealed class ODataMessageWriter
#endif
    {
        /// <summary>The message for which the message writer was created.</summary>
        private readonly ODataMessage message;

        /// <summary>A flag indicating whether we are writing a request or a response message.</summary>
        private readonly bool writingResponse;

        /// <summary>The writer settings to use when writing the message payload.</summary>
        private readonly ODataWriterSettings settings;

        /// <summary>The metadata provider. Non-null if we do have metadata available.</summary>
        private readonly DataServiceMetadataProviderWrapper metadataProvider;

        /// <summary>Flag to ensure that only a single write method is called on the message writer.</summary>
        private bool writeMethodCalled;

        /// <summary>The payload kind of the payload to be written with this writer.</summary>
        /// <remarks>This is either set via the SetHeadersForPayload method or implicitly when one of the write (or writer creation) methods is called.</remarks>
        private ODataPayloadKind writerPayloadKind = ODataPayloadKind.Unsupported;

        /// <summary>The <see cref="ODataFormat"/> of the payload to be written with this writer.</summary>
        /// <remarks>This is either set via the SetHeadersForPayload method or implicitly when one of the write (or writer creation) methods is called.</remarks>
        private ODataFormat format;

        /// <summary>The <see cref="Encoding"/> of the payload to be written with this writer.</summary>
        /// <remarks>This is either set via the SetHeadersForPayload method or implicitly when one of the write (or writer creation) methods is called.</remarks>
        private Encoding encoding;

        /// <summary>The batch boundary string if the payload to be written is a batch request or response.</summary>
        /// <remarks>This is either set via the SetHeadersForPayload method or implicitly when the CreateBatchWriter method is called.</remarks>
        private string batchBoundary;

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage)
            : this(requestMessage, new ODataWriterSettings())
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message and writer settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        /// <param name="settings">The writer settings to use for writing the message payload.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage, ODataWriterSettings settings)
            : this(requestMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message and writer settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        /// <param name="settings">The writer settings to use for writing the message payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage, ODataWriterSettings settings, IDataServiceMetadataProvider metadataProvider)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            ODataVersionChecker.CheckVersionSupported(settings.Version);

            this.writingResponse = false;
            this.message = new ODataRequestMessage(requestMessage);
            this.settings = settings;
            if (metadataProvider != null)
            {
                this.metadataProvider = new DataServiceMetadataProviderWrapper(metadataProvider);
            }
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage)
            : this(responseMessage, new ODataWriterSettings())
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message and writer settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        /// <param name="settings">The writer settings to use for writing the message payload.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage, ODataWriterSettings settings)
            : this(responseMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message and writer settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        /// <param name="settings">The writer settings to use for writing the message payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage, ODataWriterSettings settings, IDataServiceMetadataProvider metadataProvider)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            ODataVersionChecker.CheckVersionSupported(settings.Version);

            this.writingResponse = true;
            this.message = new ODataResponseMessage(responseMessage);
            this.settings = settings;
            if (metadataProvider != null)
            {
                this.metadataProvider = new DataServiceMetadataProviderWrapper(metadataProvider);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataFeedWriter()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Feed);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings, 
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.metadataProvider, 
                true /* writingFeed */,
                true /* synchronous */);
            Stream messageStream = this.message.GetStream();
            return createFunc(messageStream);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataFeedWriterAsync()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Feed);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings, 
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.metadataProvider, 
                true /* writingFeed */,
                false /* synchronous */);
            return this.message.GetStreamAsync()
                .ContinueWith(
                    (streamTask) => createFunc(streamTask.Result),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataEntryWriter()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Entry);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings, 
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.metadataProvider, 
                false /* writingFeed */, 
                true /* synchronous */);
            Stream messageStream = this.message.GetStream();
            return createFunc(messageStream);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataEntryWriterAsync()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Entry);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings, 
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.metadataProvider, 
                false /* writingFeed */, 
                false /* synchronous */);
            return this.message.GetStreamAsync()
                .ContinueWith(
                    (streamTask) => createFunc(streamTask.Result),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection writer.</returns>
        public ODataCollectionWriter CreateODataCollectionWriter()
        {
            Func<Stream, ODataCollectionWriter> createFunc = this.CreateODataCollectionWriterImplementation(true);

            Stream messageStream = this.message.GetStream();
            return createFunc(messageStream);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>A running task for the created collection writer.</returns>
        public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync()
        {
            Func<Stream, ODataCollectionWriter> createFunc = this.CreateODataCollectionWriterImplementation(false);

            return this.message.GetStreamAsync()
                .ContinueWith(
                    (streamTask) => createFunc(streamTask.Result),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        public ODataBatchWriter CreateODataBatchWriter()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Batch);

            Func<Stream, ODataBatchWriter> createFunc = ODataBatchWriter.Create(this.message, this.settings, this.writingResponse, this.batchBoundary, true);
            Stream messageStream = this.message.GetStream();
            return createFunc(messageStream);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        public Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Batch);

            Func<Stream, ODataBatchWriter> createFunc = ODataBatchWriter.Create(this.message, this.settings, this.writingResponse, this.batchBoundary, false);
            return this.message.GetStreamAsync()
                .ContinueWith(
                    (streamTask) => createFunc(streamTask.Result),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        public void WriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            this.WriteToStream(this.WriteServiceDocumentImplementation(defaultWorkspace));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        public Task WriteServiceDocumentAsync(ODataWorkspace defaultWorkspace)
        {
            return this.WriteToStreamAsync(this.WriteServiceDocumentImplementation(defaultWorkspace));
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        public void WriteProperty(ODataProperty property)
        {
            this.WriteToStream(this.WritePropertyImplementation(property));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A task representing the asynchronous operation of writing the property.</returns>
        public Task WritePropertyAsync(ODataProperty property)
        {
            return this.WriteToStreamAsync(this.WritePropertyImplementation(property));
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
        public void WriteError(ODataError error, bool includeDebugInformation)
        {
            this.WriteToStream(this.WriteErrorImplementation(error, includeDebugInformation));
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
        public Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
        {
            return this.WriteToStreamAsync(this.WriteErrorImplementation(error, includeDebugInformation));
        }
#endif

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The associated entity links to write as message payload.</param>
        public void WriteAssociatedEntityLinks(ODataAssociatedEntityLinks links)
        {
            this.WriteToStream(this.WriteAssociatedEntityLinksImplementation(links));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The associated entity links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the associated entity links.</returns>
        public Task WriteAssociatedEntityLinksAsync(ODataAssociatedEntityLinks links)
        {
            return this.WriteToStreamAsync(this.WriteAssociatedEntityLinksImplementation(links));
        }
#endif

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The associated entity link to write as message payload.</param>
        public void WriteAssociatedEntityLink(ODataAssociatedEntityLink link)
        {
            this.WriteToStream(this.WriteAssociatedEntityLinkImplementation(link));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        public Task WriteAssociatedEntityLinkAsync(ODataAssociatedEntityLink link)
        {
            return this.WriteToStreamAsync(this.WriteAssociatedEntityLinkImplementation(link));
        }
#endif

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue(object value)
        {
            this.WriteToStream(this.WriteValueImplementation(value));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>A running task representing the writing of the value.</returns>
        public Task WriteValueAsync(object value)
        {
            return this.WriteToStreamAsync(this.WriteValueImplementation(value));
        }
#endif

        /// <summary>
        /// Writes the metadata document as the message body.
        /// </summary>
        public void WriteMetadataDocument()
        {
            this.VerifyWriterNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.MetadataDocument);

            // TODO, ckerer: this method will take an IDSMP as input
            // TODO, ckerer: this method will translate IDSMP into the EdmLib OM and then call the EdmLib serializer
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write (or writer creation) methods on the <see cref="ODataMessageWriter"/>.
        /// If it is sufficient to set the headers when the write (or writer creation) methods on the <see cref="ODataMessageWriter"/>
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="payloadKind">The kind of payload to be written with this message writer.</param>
        internal void SetHeaders(ODataPayloadKind payloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadKind != ODataPayloadKind.Unsupported, "payloadKind != ODataPayloadKind.Unsupported");

            this.writerPayloadKind = payloadKind;

            // Determine the content type and format from the settings.
            MediaType mediaType;
            this.format = MediaTypeUtils.GetContentTypeFromSettings(this.settings, payloadKind, out mediaType, out this.encoding);

            string contentType;

            if (payloadKind == ODataPayloadKind.Batch)
            {
                // Note that this serves as verification only for now, since we only support a single content type and format for $batch payloads.
                Debug.Assert(this.format == ODataFormat.Default, "$batch should only support default format since it's format independent.");
                Debug.Assert(mediaType.TypeName == MimeConstants.MimeMultipartMixed, "$batch content type is currently only supported to be multipart/mixed.");

                //// TODO: Bug 135611: What about the encoding - should we verify that it's 7bit US-ASCII only?

                this.batchBoundary = ODataBatchWriterUtils.CreateBatchBoundary(this.writingResponse);

                // Set the content type header here since all headers have to be set before getting the stream
                // Note that the mediaType may have additional parameters, which we ignore here (intentional as per MIME spec).
                // Note that we always generate a new boundary string here, even if the accept header contained one.
                // We need the boundary to be as unique as possible to avoid possible collision with content of the batch operation payload.
                // Our boundary string are generated to fulfill this requirement, client specified ones might not which might lead to wrong responses
                // and at least in theory security issues.
                contentType = ODataBatchWriterUtils.CreateMultipartMixedContentType(this.batchBoundary);
            }
            else
            {
                this.batchBoundary = null;

                // compute the content type (incl. charset) and set the Content-Type header on the response message
                contentType = HttpUtils.BuildContentType(mediaType, this.encoding);
            }

            this.message.SetHeader(ODataHttpHeaders.ContentType, contentType);

            // Set the data service version
            ODataUtilsInternal.SetDataServiceVersion(this.message, this.settings);
        }

        /// <summary>
        /// If no headers have been set, sets the content-type and data service version headers on the message used by the message writer.
        /// If headers have been set explicitly (via ODataUtils.SetHeaderForPayload) this method verifies that the payload kind used to 
        /// create the headers is the same as the one being passed in <paramref name="payloadKind"/>.
        /// </summary>
        /// <param name="payloadKind">The kind of payload to be written with this message writer.</param>
        private void SetOrVerifyHeaders(ODataPayloadKind payloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadKind != ODataPayloadKind.Unsupported, "payloadKind != ODataPayloadKind.Unsupported");

            // verify that no payload kind has been set or that the payload kind set previously and the 
            // payload that is attempted to being written are the same
            this.VerifyPayloadKind(payloadKind);

            if (this.writerPayloadKind == ODataPayloadKind.Unsupported)
            {
                // no payload kind or headers have been set; set them now
                this.SetHeaders(payloadKind);
            }
        }

        /// <summary>
        /// Write a top-level value to the given stream. This method creates an
        /// async buffered stream, writes the link to it and returns an <see cref="AsyncWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the value to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The AsyncBufferedStream is only created here and will be disposed after we called FlushAsync.")]
        private AsyncWriter WriteValue(Stream stream, object value)
        {
            if (value == null)
            {
                // TODO: Bug 84434: OIPI doc seems to indicate in Section 2.2.6.4.1 that 'null' is permissible but the product does not support it.
                // We also throw in this case.
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteNullInRawFormat);
            }

            TextWriter textWriter;
            AsyncBufferedStream asyncBufferedStream = new AsyncBufferedStream(stream);
            if (this.writerPayloadKind == ODataPayloadKind.BinaryValue)
            {
                textWriter = WriterUtils.WriteBinaryValue(asyncBufferedStream, (byte[])value);
            }
            else
            {
                textWriter = WriterUtils.WriteRawValue(asyncBufferedStream, value, this.encoding);
            }

            return new AsyncWriter(asyncBufferedStream, textWriter);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A func which returns the newly created collection writer given a stream to write to.</returns>
        private Func<Stream, ODataCollectionWriter> CreateODataCollectionWriterImplementation(bool synchronous)
        {
            this.VerifyWriterNotUsed();

            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_CollectionInRequest);
            }

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Collection);

            return ODataCollectionWriter.Create(this.message, this.settings, this.format, this.encoding, this.metadataProvider, synchronous);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WriteServiceDocumentImplementation(ODataWorkspace defaultWorkspace)
        {
            this.VerifyWriterNotUsed();
            ExceptionUtils.CheckArgumentNotNull(defaultWorkspace, "defaultWorkspace");

            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_ServiceDocumentInRequest);
            }

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.ServiceDocument);

            return (stream) => this.WriteServiceDocument(stream, defaultWorkspace);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WritePropertyImplementation(ODataProperty property)
        {
            this.VerifyWriterNotUsed();
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Property);

            return (stream) => this.WriteProperty(stream, property);
        }

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WriteErrorImplementation(ODataError error, bool includeDebugInformation)
        {
            this.VerifyWriterNotUsed();
            ExceptionUtils.CheckArgumentNotNull(error, "error");

            if (!this.writingResponse)
            {
                // top-level errors can only be written for response messages
                throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
            }

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Error);

            return (stream) => this.WriteError(stream, error, includeDebugInformation);
        }

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The associated entity links to write as message payload.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WriteAssociatedEntityLinksImplementation(ODataAssociatedEntityLinks links)
        {
            // NOTE: we decided to not stream links for now but only make writing them async.
            this.VerifyWriterNotUsed();
            ExceptionUtils.CheckArgumentNotNull(links, "links");

            if (links.InlineCount.HasValue)
            {
                // Check that Count is not set for requests
                if (!this.writingResponse)
                {
                    throw new ODataException(Strings.ODataMessageWriter_InlineCountInRequest);
                }

                ODataVersionChecker.CheckInlineCount(this.settings.Version);
            }

            if (links.NextLink != null)
            {
                // Check that NextPageLink is not set for requests
                if (!this.writingResponse)
                {
                    throw new ODataException(Strings.ODataMessageWriter_NextPageLinkInRequest);
                }

                ODataVersionChecker.CheckServerPaging(this.settings.Version);
            }

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.AssociatedEntityLink);

            return (stream) => this.WriteAssociatedEntityLinksImplementation(stream, links);
        }

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WriteAssociatedEntityLinkImplementation(ODataAssociatedEntityLink link)
        {
            this.VerifyWriterNotUsed();
            ExceptionUtils.CheckArgumentNotNull(link, "link");

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.AssociatedEntityLink);

            return (stream) => this.WriteAssociatedEntityLinkImplementation(stream, link);
        }

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, AsyncWriter> WriteValueImplementation(object value)
        {
            this.VerifyWriterNotUsed();

            // We cannot use AtomValueUtils.TryConvertPrimitiveToString for all cases since binary values are
            // converted into unencoded byte streams in the raw format 
            // (as opposed to base64 encoded byte streams in the AtomValueUtils); see OIPI 2.2.6.4.1.
            ODataPayloadKind payloadKind = value is byte[] ? ODataPayloadKind.BinaryValue : ODataPayloadKind.Value;

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(payloadKind);

            Debug.Assert(this.format == ODataFormat.Default, "The content-type negotiation should return Default as the format for raw values.");
            if (this.format != ODataFormat.Default)
            {
                throw new ODataException(Strings.ODataMessageWriter_InvalidAcceptHeaderForWritingRawValue(this.settings.AcceptableMediaTypes));
            }

            return (stream) => this.WriteValue(stream, value);
        }

        /// <summary>
        /// Gets the underlying stream and invokes the <paramref name="writeFunc"/> to write a payload to the stream.
        /// When done flushes and disposes the stream.
        /// </summary>
        /// <param name="writeFunc">The function that writes a payload to the stream.</param>
        private void WriteToStream(Func<Stream, AsyncWriter> writeFunc)
        {
            Stream messageStream = this.message.GetStream();
            AsyncWriter asyncWriter = writeFunc(messageStream);

            // NOTE we ensure that Dispose() is called because we guarantee that AsyncWriter.FlushSync() cannot
            //      fail (even if an exception is reported there). See AsyncWriter.FlushAsync() for more information.
            try
            {
                asyncWriter.FlushSync();
            }
            finally
            {
                asyncWriter.Dispose();
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously gets the underlying stream and invokes the <paramref name="writeFunc"/> to write a payload to the stream.
        /// When done flushes and disposes the async stream.
        /// </summary>
        /// <param name="writeFunc">The function that writes a payload to the stream.</param>
        /// <returns>A task representing the combined asynchronous task of getting the stream, writing the payload, flushing and diposing the stream.</returns>
        private Task WriteToStreamAsync(Func<Stream, AsyncWriter> writeFunc)
        {
            // we get the stream asynchronously, then write the data to the stream and then
            // use a child task to flush the stream and dispose it.
            // NOTE it is important to run all child tasks with the 'AttachedToParent' option to make
            //      sure all exceptions are bubbled up to the parent task. In particular this means that
            //      AsyncWriter.FlushAsync() and the AsyncBufferedStream.FlushAsync() methods need to do so as
            //      well.
            // NOTE we ensure that Dispose() is called because we guarantee that AsyncWriter.FlushAsync() cannot
            //      fail (even if an exception is reported there). See AsyncWriter.FlushAsync() for more information.
            return this.message.GetStreamAsync()
                .ContinueWith((streamTask) => writeFunc(streamTask.Result), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent)
                .ContinueWith(
                    (asyncWriterTask) =>
                        (asyncWriterTask.Result.FlushAsync()
                            .ContinueWith(
                                (asyncWriterTask2) => asyncWriterTask2.Result.Dispose(),
                                TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously)),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Verifies that the ODataMessageWriter has not been used before; an ODataMessageWriter can only be used to
        /// write a single message payload but can't be reused later.
        /// </summary>
        private void VerifyWriterNotUsed()
        {
            if (this.writeMethodCalled)
            {
                throw new ODataException(Strings.ODataMessageWriter_WriterAlreadyUsed);
            }

            this.writeMethodCalled = true;
        }

        /// <summary>
        /// Verifies that, if a payload kind has been set via SetHeaders, the payload kind that
        /// is being written is the same.
        /// </summary>
        /// <param name="payloadKindToWrite">The payload kind that is attempted to write.</param>
        private void VerifyPayloadKind(ODataPayloadKind payloadKindToWrite)
        {
            Debug.Assert(payloadKindToWrite != ODataPayloadKind.Unsupported, "payloadKindToWrite != ODataPayloadKind.Unsupported");

            if (this.writerPayloadKind != ODataPayloadKind.Unsupported && this.writerPayloadKind != payloadKindToWrite)
            {
                // if a payload kind has been set via SetHeaders that is not the same as the payload kind
                // that is attempted to write, we fail.
                throw new ODataException(Strings.ODataMessageWriter_IncompatiblePayloadKinds(this.writerPayloadKind, payloadKindToWrite));
            }
        }

        /// <summary>
        /// Write an <see cref="ODataProperty" /> to the given stream. This method creates an
        /// async buffered stream, writes the property to it and returns an <see cref="AsyncWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the property to.</param>
        /// <param name="property">The property to write.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private AsyncWriter WriteProperty(Stream stream, ODataProperty property)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            // TODO: how would we get the resource property for validation purposes here?
            return this.WriteTopLevelContent(
                stream, 
                (xmlWriter) => ODataAtomWriterUtils.WriteProperty(xmlWriter, this.metadataProvider, property, null, this.settings.Version, true, false, null, null),
                (jsonWriter) => ODataJsonWriterUtils.WriteTopLevelProperty(jsonWriter, this.metadataProvider, property, null, this.settings.Version, this.writingResponse),
                Strings.ODataMessageWriter_InvalidContentTypeForWritingProperty,
                InternalErrorCodes.ODataMessageWriter_WriteProperty);
        }

        /// <summary>
        /// Write a service document with the specified <paramref name="defaultWorkspace"/>to the given stream. 
        /// This method creates an async buffered stream, writes the service document to it and returns 
        /// an <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private AsyncWriter WriteServiceDocument(Stream stream, ODataWorkspace defaultWorkspace)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteServiceDocument(xmlWriter, this.metadataProvider, defaultWorkspace, this.settings.BaseUri),
                (jsonWriter) => ODataJsonWriterUtils.WriteServiceDocument(jsonWriter, this.metadataProvider, defaultWorkspace),
                Strings.ODataMessageWriter_InvalidContentTypeForWritingServiceDocument,
                InternalErrorCodes.ODataMessageWriter_WriteServiceDocument);
        }

        /// <summary>
        /// Write a top-level link to the given stream. This method creates an
        /// async buffered stream, writes the link to it and returns an <see cref="AsyncWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the property to.</param>
        /// <param name="link">The associated entity link to write.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private AsyncWriter WriteAssociatedEntityLinkImplementation(Stream stream, ODataAssociatedEntityLink link)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteAssociatedEntityLink(xmlWriter, link, true),
                (jsonWriter) => ODataJsonWriterUtils.WriteAssociatedEntityLink(jsonWriter, this.settings.BaseUri, link, this.writingResponse),
                Strings.ODataMessageWriter_InvalidContentTypeForWritingLink,
                InternalErrorCodes.ODataMessageWriter_WriteAssociatedEntityLink);
        }

        /// <summary>
        /// Write a top-level error to the given stream. This method creates an
        /// async buffered stream, writes the error to it and returns an <see cref="AsyncWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the links to.</param>
        /// <param name="error">The error to write as message payload.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private AsyncWriter WriteError(Stream stream, ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteError(xmlWriter, error, includeDebugInformation),
                (jsonWriter) => ODataJsonWriterUtils.WriteTopLevelError(jsonWriter, error, includeDebugInformation, this.writingResponse),
                Strings.ODataMessageWriter_InvalidContentTypeForWritingError,
                InternalErrorCodes.ODataMessageWriter_WriteError);
        }

        /// <summary>
        /// Write a set of top-level links to the given stream. This method creates an
        /// async buffered stream, writes the links to it and returns an <see cref="AsyncWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the links to.</param>
        /// <param name="links">The associated entity links to write as message payload.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private AsyncWriter WriteAssociatedEntityLinksImplementation(Stream stream, ODataAssociatedEntityLinks links)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteAssociatedEntityLinks(xmlWriter, links),
                (jsonWriter) => ODataJsonWriterUtils.WriteAssociatedEntityLinks(jsonWriter, this.settings.BaseUri, links, this.settings.Version, this.writingResponse),
                Strings.ODataMessageWriter_InvalidContentTypeForWritingLinks,
                InternalErrorCodes.ODataMessageWriter_WriteAssociatedEntityLinks);
        }

        /// <summary>
        /// Method to write plain Xml (or JSON) content for top-level properties, top-level links and a single top-level link.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="writeAtomAction">An action that writes the payload in ATOM format.</param>
        /// <param name="writeJsonAction">An action that writes the payload in JSON format.</param>
        /// <param name="invalidContentTypeErrorMessageFunction">A function that returns an error message if the content type is invalid.</param>
        /// <param name="internalErrorCode">An internal error code identifying the caller used in case of an internal error.</param>
        /// <returns>An <see cref="AsyncWriter"/> that can be used to flush and close/dispose the stream.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The AsyncBufferedStream is only created here and will be disposed after we called FlushAsync.")]
        private AsyncWriter WriteTopLevelContent(
            Stream stream, 
            Action<XmlWriter> writeAtomAction, 
            Action<JsonWriter> writeJsonAction,
            Func<object, string> invalidContentTypeErrorMessageFunction, 
            InternalErrorCodes internalErrorCode)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            AsyncBufferedStream asyncBufferedStream = new AsyncBufferedStream(stream);
            switch (this.format)
            {
                case ODataFormat.Json:
                    StreamWriter textWriter = new StreamWriter(asyncBufferedStream, this.encoding);
                    JsonWriter jsonWriter = new JsonWriter(textWriter, this.settings.Indent);
                    writeJsonAction(jsonWriter);
                    return new AsyncWriter(asyncBufferedStream, textWriter, jsonWriter);

                case ODataFormat.Atom:
                    XmlWriter xmlWriter = ODataAtomWriterUtils.CreateXmlWriter(asyncBufferedStream, this.settings, this.encoding);
                    writeAtomAction(xmlWriter);
                    return new AsyncWriter(asyncBufferedStream, xmlWriter);

                case ODataFormat.Default:
                    Debug.Assert(false, "Should never get here as content-type negotiation should not return Default format for top level content.");
                    string contentType = this.message.GetHeader(ODataHttpHeaders.ContentType);
                    throw new ODataException(invalidContentTypeErrorMessageFunction(contentType));

                default:
                    throw new ODataException(Strings.General_InternalError(internalErrorCode));
            }
        }

        /// <summary>
        /// Helper class representing an async writer that can be either an ATOM writer or a JSON writer.
        /// </summary>
        private sealed class AsyncWriter : IDisposable
        {
            /// <summary>The async buffered stream for the writer.</summary>
            private AsyncBufferedStream asyncBufferedStream;

            /// <summary>The text writer backing the <see cref="jsonWriter"/>; null in the ATOM case.</summary>
            private TextWriter textWriter;

            /// <summary>The JSON writer; null in the ATOM case.</summary>
            private JsonWriter jsonWriter;

            /// <summary>The ATOM writer; null in the JSON case.</summary>
            private XmlWriter atomWriter;

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on an Xml ATOM writer.
            /// </summary>
            /// <param name="asyncBufferedStream">The async buffered stream to use.</param>
            /// <param name="atomWriter">The Xml writer to write to.</param>
            internal AsyncWriter(AsyncBufferedStream asyncBufferedStream, XmlWriter atomWriter)
            {
                Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
                Debug.Assert(atomWriter != null, "atomWriter != null");

                this.asyncBufferedStream = asyncBufferedStream;
                this.atomWriter = atomWriter;
            }

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on a JSON writer.
            /// </summary>
            /// <param name="asyncBufferedStream">The async buffered stream to use.</param>
            /// <param name="textWriter">The text writer backing the <see cref="jsonWriter"/>.</param>
            /// <param name="jsonWriter">The JSON writer to write to.</param>
            internal AsyncWriter(AsyncBufferedStream asyncBufferedStream, TextWriter textWriter, JsonWriter jsonWriter)
            {
                Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
                Debug.Assert(textWriter != null, "textWriter != null");
                Debug.Assert(jsonWriter != null, "jsonWriter != null");

                this.asyncBufferedStream = asyncBufferedStream;
                this.textWriter = textWriter;
                this.jsonWriter = jsonWriter;
            }

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on a text writer.
            /// </summary>
            /// <param name="asyncBufferedStream">The async buffered stream to use.</param>
            /// <param name="textWriter">The text writer for writing raw values.</param>
            internal AsyncWriter(AsyncBufferedStream asyncBufferedStream, TextWriter textWriter)
            {
                Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");

                this.asyncBufferedStream = asyncBufferedStream;
                this.textWriter = textWriter;
            }

            /// <summary>
            /// Dispose the writer and the underlying stream.
            /// </summary>
            public void Dispose()
            {
                try
                {
                    if (this.atomWriter == null)
                    {
                        if (this.textWriter == null)
                        {
                            this.asyncBufferedStream.Dispose();
                        }
                        else
                        {
                            // The text writer will also dispose the this.asyncBufferedStream since it owns that stream
                            // which in turn will dispose the real output stream underneath it.
                            this.textWriter.Dispose();
                        }
                    }
                    else
                    {
                        Utils.TryDispose(this.atomWriter);
                        this.asyncBufferedStream.Dispose();
                    }
                }
                finally
                {
                    this.asyncBufferedStream = null;
                    this.jsonWriter = null;
                    this.atomWriter = null;
                }
            }

            /// <summary>
            /// Flushes the write buffer to the underlying stream.
            /// </summary>
            internal void FlushSync()
            {
                this.FlushWriters();
                this.asyncBufferedStream.FlushSync();
            }

#if ODATALIB_ASYNC
            /// <summary>
            /// Asynchronously flushes the write buffer to the underlying stream.
            /// </summary>
            /// <returns>A task instance that represents the asynchronous operation.</returns>
            internal Task<AsyncWriter> FlushAsync()
            {
                this.FlushWriters();

                // We guarantee that the combined task never is in Faulted state; even if AsyncBufferedStream.FlushAsync()
                // throws an exception we eat it in the continuation and return 'this'. The exception will still
                // be reported on the calling thread since AsyncBufferedStream.FlushAsync() uses the 'AttachedToParent' option.
                return this.asyncBufferedStream.FlushAsync()
                    .ContinueWith((task) => { return this; }, TaskContinuationOptions.ExecuteSynchronously);
            }
#endif

            /// <summary>
            /// Flushes all writers to the underlying stream.
            /// </summary>
            private void FlushWriters()
            {
                if (this.atomWriter != null)
                {
                    this.atomWriter.Flush();
                }
                else if (this.jsonWriter != null)
                {
                    this.jsonWriter.Flush();
                }
                else if (this.textWriter != null)
                {
                    this.textWriter.Flush();
                }
            }
        }
    }
}
