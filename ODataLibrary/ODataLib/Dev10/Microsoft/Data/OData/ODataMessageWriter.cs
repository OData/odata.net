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
    using System.Xml;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Writer class used to write all OData payloads (entries, feeds, metadata documents, service documents, etc.).
    /// </summary>
    public sealed class ODataMessageWriter : IDisposable
    {
        /// <summary>The message for which the message writer was created.</summary>
        private readonly ODataMessage message;

        /// <summary>A flag indicating whether we are writing a request or a response message.</summary>
        private readonly bool writingResponse;

        /// <summary>The message writer settings to use when writing the message payload.</summary>
        private readonly ODataMessageWriterSettings settings;

        /// <summary>The model. Non-null if we do have metadata available.</summary>
        private readonly IEdmModel model;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs written to the payload.</summary>
        private readonly IODataUrlResolver urlResolver;

        /// <summary>Flag to ensure that only a single write method is called on the message writer.</summary>
        private bool writeMethodCalled;

        /// <summary>True if Dispose() has been called on this message writer, False otherwise.</summary>
        private bool isDispsed;

        /// <summary>The underlying stream from the message.</summary>
        private Stream messageStream;

        /// <summary>Interface to cleanup the created writer.</summary>
        private IODataWriter writer;

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

        /// <summary>Flag to prevent writing more than one error to the payload.</summary>
        private bool writeErrorCalled;

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage)
            : this(requestMessage, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message and message writer settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        /// <param name="settings">The message writer settings to use for writing the message payload.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage, ODataMessageWriterSettings settings)
            : this(requestMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given request message and message writer settings.
        /// </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        /// <param name="settings">The message writer settings to use for writing the message payload.</param>
        /// <param name="model">The model to use.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage, ODataMessageWriterSettings settings, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");

            this.writingResponse = false;
            this.message = new ODataRequestMessage(requestMessage);
            this.urlResolver = requestMessage as IODataUrlResolver;
            if (model == null)
            {
                this.model = EdmCoreModel.Instance;
            }
            else
            {
                this.model = model;
                ValidationUtils.ValidateModel(model);
            }

            // Clone the settings here so we can later modify them without changing the settings passed to us by the user
            this.settings = settings == null ? new ODataMessageWriterSettings() : new ODataMessageWriterSettings(settings);
            WriterValidationUtils.ValidateMessageWriterSettings(this.settings);
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage)
            : this(responseMessage, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message and message writer settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        /// <param name="settings">The message writer settings to use for writing the message payload.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage, ODataMessageWriterSettings settings)
            : this(responseMessage, settings, null)
        {
        }

        /// <summary>
        /// Creates a new ODataMessageWriter for the given response message and message writer settings.
        /// </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        /// <param name="settings">The message writer settings to use for writing the message payload.</param>
        /// <param name="model">The model to use.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage, ODataMessageWriterSettings settings, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");

            this.writingResponse = true;
            this.message = new ODataResponseMessage(responseMessage);
            this.urlResolver = responseMessage as IODataUrlResolver;
            if (model == null)
            {
                this.model = EdmCoreModel.Instance;
            }
            else
            {
                this.model = model;
                ValidationUtils.ValidateModel(model);
            }

            // Clone the settings here so we can later modify them without changing the settings passed to us by the user
            this.settings = settings == null ? new ODataMessageWriterSettings() : new ODataMessageWriterSettings(settings);
            WriterValidationUtils.ValidateMessageWriterSettings(this.settings);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataFeedWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Feed);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message,
                this.settings,
                this.urlResolver,
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.model, 
                true /* writingFeed */,
                true /* synchronous */);
            this.GetMessageStream();
            return this.SetWriter(createFunc(this.messageStream));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataFeedWriterAsync()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Feed);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings,
                this.urlResolver,
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.model, 
                true /* writingFeed */,
                false /* synchronous */);
            return this.GetMessageStreamAsync()
                .ContinueWith(
                    (streamTask) => this.SetWriter(createFunc(streamTask.Result)),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataEntryWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Entry);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings,
                this.urlResolver,
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.model, 
                false /* writingFeed */, 
                true /* synchronous */);
            this.GetMessageStream();
            return this.SetWriter(createFunc(this.messageStream));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataEntryWriterAsync()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Entry);

            Func<Stream, ODataWriter> createFunc = ODataWriter.Create(
                this.message, 
                this.settings,
                this.urlResolver,
                this.format, 
                this.encoding, 
                this.writingResponse, 
                this.model, 
                false /* writingFeed */, 
                false /* synchronous */);
            return this.GetMessageStreamAsync()
                .ContinueWith(
                    (streamTask) => this.SetWriter(createFunc(streamTask.Result)),
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

            this.GetMessageStream();
            return this.SetWriter(createFunc(this.messageStream));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>A running task for the created collection writer.</returns>
        public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync()
        {
            Func<Stream, ODataCollectionWriter> createFunc = this.CreateODataCollectionWriterImplementation(false);

            return this.GetMessageStreamAsync()
                .ContinueWith(
                    (streamTask) => this.SetWriter(createFunc(streamTask.Result)),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        public ODataBatchWriter CreateODataBatchWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Batch);

            Func<Stream, ODataBatchWriter> createFunc = ODataBatchWriter.Create(this.message, this.settings, this.writingResponse, this.batchBoundary, true);
            this.GetMessageStream();
            return this.SetWriter(createFunc(this.messageStream));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        public Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Batch);

            Func<Stream, ODataBatchWriter> createFunc = ODataBatchWriter.Create(this.message, this.settings, this.writingResponse, this.batchBoundary, false);
            return this.GetMessageStreamAsync()
                .ContinueWith(
                    (streamTask) => this.SetWriter(createFunc(streamTask.Result)),
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
        /// <param name="property">The property to write.</param>
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
            Func<Stream, IODataWriter> writerFunc = this.WriteErrorImplementation(error, includeDebugInformation);

            // If messageStream is null when WriteError is called, we are writing a top level error.
            // If messageStream is already set, we are writing an in-stream error.
            if (this.messageStream == null)
            {
                this.GetMessageStream();
            }

            writerFunc(this.messageStream).FlushWriter();
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
            Func<Stream, IODataWriter> writerFunc = this.WriteErrorImplementation(error, includeDebugInformation);

            // we get the stream asynchronously, then write the data to the stream and then
            // use a child task to flush the stream.
            // NOTE it is important to run all child tasks with the 'AttachedToParent' option to make
            //      sure all exceptions are bubbled up to the parent task. In particular this means that
            //      IODataWriter.FlushWriterAsync() and the AsyncBufferedStream.FlushAsync() methods need to do so as
            //      well.
            // If messageStream is null when WriteError is called, we are writing a top level error.
            // If messageStream is already set, we are writing an in-stream error.
            return (this.messageStream == null ? this.GetMessageStreamAsync() : TaskUtils.GetCompletedTask(this.messageStream))
                .ContinueWith((streamTask) => writerFunc(streamTask.Result).FlushWriterAsync(), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        public void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            this.WriteToStream(this.WriteEntityReferenceLinksImplementation(links));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        public Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            return this.WriteToStreamAsync(this.WriteEntityReferenceLinksImplementation(links));
        }
#endif

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        public void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            this.WriteToStream(this.WriteEntityReferenceLinkImplementation(link));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        public Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            return this.WriteToStreamAsync(this.WriteEntityReferenceLinkImplementation(link));
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
            this.WriteToStream(this.WriteMetadataDocumentImplementation());
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
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write (or writer creation) methods on the <see cref="ODataMessageWriter"/>.
        /// If it is sufficient to set the headers when the write (or writer creation) methods on the <see cref="ODataMessageWriter"/>
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="payloadKind">The kind of payload to be written with this message writer.</param>
        /// <param name="mimeTypeForRawValue">
        /// The MIME type to be used for writing the content of the message. 
        /// Note that this is only supported for top-level raw values.
        /// </param>
        internal void SetHeaders(ODataPayloadKind payloadKind, string mimeTypeForRawValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadKind != ODataPayloadKind.Unsupported, "payloadKind != ODataPayloadKind.Unsupported");
            Debug.Assert(mimeTypeForRawValue == null || payloadKind == ODataPayloadKind.Value, "Explicit MIME type can only be set for top-level values.");

            this.writerPayloadKind = payloadKind;

            // Make sure we have a version set on the message writer settings; if none was specified on the settings, try to read
            // it from the message headers. If not specified in the headers either, fall back to a default.
            // NOTE: This method will potentially also set the data service version header.
            this.EnsureODataVersion();
            Debug.Assert(this.settings.Version.HasValue, "ODataVersion must have been set by now.");

            // Make sure we have a content-type and compute the format from the settings and/or the message headers.
            // NOTE: This method will potentially also set the content type header.
            this.EnsureODataFormatAndContentType(mimeTypeForRawValue);
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
                this.SetHeaders(payloadKind, null /*mimeTypeForRawValue*/);
            }
        }

        /// <summary>
        /// Ensures that the version of the OData protocol is set.
        /// </summary>
        /// <remarks>
        /// If a version is specified explicitly on the writer settings, it is used.
        /// Otherwise the method tries to read the version from the message headers.
        /// If there is a version header but the value cannot be parsed, we fail.
        /// If there is no version header, we fall back to the default version.
        /// </remarks>
        private void EnsureODataVersion()
        {
            // if no version was specified in the user settings, try to read it from the message headers
            if (!this.settings.Version.HasValue)
            {
                // Read the version header and parse it; fail if we can't parse it.
                // Fall back to a default if we don't find a version header.
                this.settings.Version = ODataUtilsInternal.GetDataServiceVersion(this.message);
                Debug.Assert(this.settings.Version.HasValue, "The version must have been set by now.");
            }
            else
            {
                // Set the data service version
                ODataUtilsInternal.SetDataServiceVersion(this.message, this.settings);
            }
        }

        /// <summary>
        /// Ensures that the OData format is computed and set; if needed, sets the content type
        /// header of the message.
        /// </summary>
        /// <param name="mimeTypeForRawValue">An explicitly set content type for the message.</param>
        /// <remarks>
        /// This method computes and ensures that a content type exists and computes the 
        /// OData format from it. If a content type is explicitly specified through 
        /// <see cref="Microsoft.Data.OData.ODataUtils.SetHeadersForPayload(Microsoft.Data.OData.ODataMessageWriter, Microsoft.Data.OData.ODataPayloadKind)"/>
        /// or <see cref="Microsoft.Data.OData.ODataMessageWriterSettings.SetContentType(string, string)"/> it will be used. If no
        /// content type is specified in either place, the message headers are checked for
        /// a content type header.
        /// If the content type is computed from settings, the content type header is set on the message.
        /// </remarks>
        private void EnsureODataFormatAndContentType(string mimeTypeForRawValue)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Writer payload kind should have been set by now.");

            string contentType = null;

            // If no explicit MIME type was set through SetHeadersForPayload and neither format nor accept headers 
            // were specified in the writer settings, try to read the content type from the message headers.
            if (mimeTypeForRawValue == null && !this.settings.UseFormat.HasValue)
            {
                contentType = this.message.GetHeader(ODataHttpHeaders.ContentType);
                contentType = contentType == null ? null : contentType.Trim();
            }

            // If we found a content type header, use it. Otherwise use the default behavior.
            if (!string.IsNullOrEmpty(contentType))
            {
                ODataPayloadKind computedPayloadKind;
                MediaType mediaType;
                this.format = MediaTypeUtils.GetFormatFromContentType(contentType, new ODataPayloadKind[] { this.writerPayloadKind }, out mediaType, out this.encoding, out computedPayloadKind, out this.batchBoundary);
                Debug.Assert(this.writerPayloadKind == computedPayloadKind, "The payload kinds must always match.");
            }
            else
            {
                // Determine the content type and format from the settings. Note that if neither format nor accept headers have been specified in the settings
                // we fall back to a default (of null accept headers).
                MediaType mediaType;
                this.format = MediaTypeUtils.GetContentTypeFromSettings(this.settings, this.writerPayloadKind, mimeTypeForRawValue, out mediaType, out this.encoding);

                if (this.writerPayloadKind == ODataPayloadKind.Batch)
                {
                    // Note that this serves as verification only for now, since we only support a single content type and format for $batch payloads.
                    Debug.Assert(this.format == ODataFormat.Default, "$batch should only support default format since it's format independent.");
                    Debug.Assert(mediaType.FullTypeName == MimeConstants.MimeMultipartMixed, "$batch content type is currently only supported to be multipart/mixed.");

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
            }
        }

        /// <summary>
        /// Write a top-level value to the given stream. This method creates an
        /// async buffered stream, writes the value to it and returns an <see cref="IODataWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the value to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The AsyncBufferedStream is only created here and will be disposed after we called FlushAsync.")]
        private IODataWriter WriteValue(Stream stream, object value)
        {
            if (value == null)
            {
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteNullInRawFormat);
            }

            TextWriter textWriter = null;
            IODataWriter asyncWriter;
            AsyncBufferedStream asyncBufferedStream = new AsyncBufferedStream(stream);
            try
            {
                if (this.writerPayloadKind == ODataPayloadKind.BinaryValue)
                {
                    textWriter = WriterUtils.WriteBinaryValue(asyncBufferedStream, (byte[])value);
                }
                else
                {
                    textWriter = WriterUtils.WriteRawValue(asyncBufferedStream, value, this.encoding);
                }
            }
            finally
            {
                // Note AsyncWriter will throw if we were to write an error after WriteValue. Writing in-stream error for raw format is not supported.
                Debug.Assert(this.format == ODataFormat.Default, "The content-type negotiation should return Default as the format for raw values.");
                asyncWriter = this.SetWriter(new AsyncWriter(asyncBufferedStream, textWriter));
            }

            return asyncWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        /// <returns>A func which returns the newly created collection writer given a stream to write to.</returns>
        private Func<Stream, ODataCollectionWriter> CreateODataCollectionWriterImplementation(bool synchronous)
        {
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.Collection);

            return ODataCollectionWriter.Create(this.message, this.settings, this.format, this.encoding, this.model, synchronous, this.writingResponse);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WriteServiceDocumentImplementation(ODataWorkspace defaultWorkspace)
        {
            ExceptionUtils.CheckArgumentNotNull(defaultWorkspace, "defaultWorkspace");

            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_ServiceDocumentInRequest);
            }

            // VerifyWriterNotDisposedAndNotUsed changes the state of the message writer, it should be called after
            // we check the error conditions above.
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.ServiceDocument);

            return (stream) => this.WriteServiceDocument(stream, defaultWorkspace);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WritePropertyImplementation(ODataProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            this.VerifyWriterNotDisposedAndNotUsed();

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
        private Func<Stream, IODataWriter> WriteErrorImplementation(ODataError error, bool includeDebugInformation)
        {
            ExceptionUtils.CheckArgumentNotNull(error, "error");
            this.VerifyNotDisposed();
            if (this.writeErrorCalled)
            {
                throw new ODataException(Strings.ODataMessageWriter_WriteErrorAlreadyCalled);
            }

            this.writeErrorCalled = true;

            // Note that users may call WriteError() for in-stream error, we don't need to verify writer-not-used here.
            this.writeMethodCalled = true;

            // We currently assume that the error is top-level if no create/write method has been called on this message writer.
            // It is possible that the user would create a Json writer, but writes nothing to it before writes the first error.
            // For example it is valid to call CreateEntryWriter() and then WriteError(). In that case the Json payload would
            // contain just an error without the Json wrapper.

            // If the writer is not set, no create/write method has been called yet and we assume this is a top level error.
            bool isTopLevel = this.writer == null;
            if (isTopLevel)
            {
                // TODO: (Dev11 Bug 176574) We only allow writing top-level error to response messages. Do we have any scenario to write in-stream errors
                // to request messages? Should we put the same restriction on in-stream errors as well?
                if (!this.writingResponse)
                {
                    // errors can only be written for response messages
                    throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
                }

                // Set the content type header here since all headers have to be set before getting the stream
                this.SetOrVerifyHeaders(ODataPayloadKind.Error);
            }

            return (stream) =>
            {
                if (isTopLevel)
                {
                    return this.WriteTopLevelError(stream, error, includeDebugInformation);
                }
                else
                {
                    this.writer.WriteError(error, includeDebugInformation);
                    return this.writer;
                }
            };
        }

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            // NOTE: we decided to not stream links for now but only make writing them async.
            ExceptionUtils.CheckArgumentNotNull(links, "links");

            // VerifyWriterNotDisposedAndNotUsed changes the state of the message writer, it should be called after
            // we check the error conditions above.
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.EntityReferenceLinks);
            Debug.Assert(this.settings != null && this.settings.Version.HasValue, "Settings must have been created and the version set.");

            if (links.InlineCount.HasValue)
            {
                // Check that Count is not set for requests
                if (!this.writingResponse)
                {
                    throw new ODataException(Strings.ODataMessageWriter_InlineCountInRequest);
                }

                ODataVersionChecker.CheckInlineCount(this.settings.Version.Value);
            }

            if (links.NextLink != null)
            {
                // Check that NextPageLink is not set for requests
                if (!this.writingResponse)
                {
                    throw new ODataException(Strings.ODataMessageWriter_NextPageLinkInRequest);
                }

                ODataVersionChecker.CheckNextLink(this.settings.Version.Value);
            }

            return (stream) => this.WriteEntityReferenceLinksImplementation(stream, links);
        }

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link)
        {
            ExceptionUtils.CheckArgumentNotNull(link, "link");
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.EntityReferenceLink);

            return (stream) => this.WriteEntityReferenceLinkImplementation(stream, link);
        }

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WriteValueImplementation(object value)
        {
            this.VerifyWriterNotDisposedAndNotUsed();

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
        /// Writes a metadata document as the message body.
        /// </summary>
        /// <returns>A func which performs the actual writing given the stream to write to.</returns>
        private Func<Stream, IODataWriter> WriteMetadataDocumentImplementation()
        {
            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_MetadataDocumentInRequest);
            }

            // check that we have a user model
            if (!this.model.IsUserModel())
            {
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteMetadataWithoutModel);
            }

            // VerifyWriterNotDisposedAndNotUsed changes the state of the message writer, it should be called after
            // we check the error conditions above.
            this.VerifyWriterNotDisposedAndNotUsed();

            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(ODataPayloadKind.MetadataDocument);

            // set the data service and max data service annotations on the model
            Version dataServiceVersion = this.settings.Version.Value.ToDataServiceVersion();
            this.model.SetDataServiceVersion(dataServiceVersion);

            // TODO: add support for MaxDataServiceVersion
            return (stream) => this.WriteMetadataDocument(stream);
        }

        /// <summary>
        /// Gets the underlying stream and invokes the <paramref name="writeFunc"/> to write a payload to the stream.
        /// When done flushes and disposes the stream.
        /// </summary>
        /// <param name="writeFunc">The function that writes a payload to the stream.</param>
        private void WriteToStream(Func<Stream, IODataWriter> writeFunc)
        {
            this.GetMessageStream();
            writeFunc(this.messageStream).FlushWriter();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously gets the underlying stream and invokes the <paramref name="writeFunc"/> to write a payload to the stream.
        /// When done flushes and disposes the async stream.
        /// </summary>
        /// <param name="writeFunc">The function that writes a payload to the stream.</param>
        /// <returns>A task representing the combined asynchronous task of getting the stream, writing the payload, flushing and disposing the stream.</returns>
        private Task WriteToStreamAsync(Func<Stream, IODataWriter> writeFunc)
        {
            // we get the stream asynchronously, then write the data to the stream and then
            // use a child task to flush the stream and dispose it.
            // NOTE it is important to run all child tasks with the 'AttachedToParent' option to make
            //      sure all exceptions are bubbled up to the parent task. In particular this means that
            //      IODataWriter.FlushAsync() and the AsyncBufferedStream.FlushAsync() methods need to do so as
            //      well.
            return this.GetMessageStreamAsync()
                .ContinueWith((streamTask) => writeFunc(streamTask.Result).FlushWriterAsync(), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
        }
#endif

        /// <summary>
        /// Verifies that the ODataMessageWriter has not been disposed and has not been used before. An ODataMessageWriter
        /// can only be used to write a single message payload but can't be reused later except for writing an in-stream error.
        /// </summary>
        private void VerifyWriterNotDisposedAndNotUsed()
        {
            this.VerifyNotDisposed();
            if (this.writeMethodCalled)
            {
                throw new ODataException(Strings.ODataMessageWriter_WriterAlreadyUsed);
            }

            this.writeMethodCalled = true;
        }

        /// <summary>
        /// Check if the object has been disposed. Throws an ObjectDisposedException if the object has already been disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            if (this.isDispsed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Synchronously get the stream backing the message.
        /// </summary>
        /// <returns>The stream for the message.</returns>
        private Stream GetMessageStream()
        {
            Debug.Assert(this.messageStream == null, "VerifyWriterNotUsedAndNotDisposed() should have prevented getting the message stream more than once.");
            return this.messageStream = this.message.GetStream();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing the message.
        /// </summary>
        /// <returns>The stream for the message.</returns>
        private Task<Stream> GetMessageStreamAsync()
        {
            Debug.Assert(this.messageStream == null, "VerifyWriterNotUsedAndNotDisposed() should have prevented getting the message stream more than once.");
            return this.message.GetStreamAsync().ContinueWith((streamTask) =>
            {
                Debug.Assert(this.messageStream == null, "VerifyWriterNotUsedAndNotDisposed() should have prevented getting the message stream more than once.");
                return this.messageStream = streamTask.Result;
            });
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        private void Dispose(bool disposing)
        {
            this.VerifyNotDisposed();
            this.isDispsed = true;
            if (disposing)
            {
                if (this.writer != null)
                {
                    // Note that DisposeWriter() should not dispose the message stream.
                    this.writer.DisposeWriter();
                    this.writer = null;
                }

                if (!this.settings.DisableMessageStreamDisposal && this.messageStream != null)
                {
                    // TODO: jli - If DisableMessageStreamDisposal is set to true for a batch operation message, the batch writer
                    // will fail because the Dispose() method on the batch operation stream needs to be called when disposing the
                    // ODataMessageWriter.  Currently the batch writer will throw an error informing the user that it is in a bad
                    // state.  Should add additional info to the error telling user to always set DisableMessageStreamDisposal to
                    // false for batch operations.
                    Utils.TryDispose(this.messageStream);
                    this.messageStream = null;
                }
            }
        }

        /// <summary>
        /// Sets the given writer.
        /// </summary>
        /// <typeparam name="T">A writer type that implements IODataWriter.</typeparam>
        /// <param name="newWriter">Writer instance.</param>
        /// <returns>Returns the exact instance as <paramref name="newWriter"/>.</returns>
        private T SetWriter<T>(T newWriter)
            where T : IODataWriter
        {
            Debug.Assert(this.writer == null, "this.writer == null");
            this.writer = newWriter;
            return newWriter;
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
        /// async buffered stream, writes the property to it and returns an <see cref="IODataWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the property to.</param>
        /// <param name="property">The property to write.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteProperty(Stream stream, ODataProperty property)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");
            Debug.Assert(property != null, "property != null");

            if (property.Value is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty(property.Name));
            }

            // TODO: how would we get the EDM property for validation purposes here?
            return this.WriteTopLevelContent(
                stream, 
                (xmlWriter) => 
                    ODataAtomWriterUtils.WriteProperty(
                        xmlWriter,
                        this.model,
                        property,
                        null  /* owningType */,
                        this.settings.Version.Value,
                        true  /* isTopLevel */,
                        false /* isWritingCollection */,
                        this.writingResponse,
                        this.settings.WriterBehavior,
                        null  /* beforePropertyAction */,
                        null  /* epmValueCache */,
                        null  /* epmParentSourcePathSegment */,
                        new DuplicatePropertyNamesChecker(this.settings.WriterBehavior.AllowDuplicatePropertyNames, this.writingResponse),
                        null  /* projectedProperties */),
                (jsonWriter) => 
                    ODataJsonWriterUtils.WriteTopLevelProperty(
                        jsonWriter, 
                        this.urlResolver,
                        this.model, 
                        property,
                        null, 
                        this.settings.Version.Value, 
                        this.writingResponse,
                        this.settings.WriterBehavior),
                null /*writeRawAction*/,
                InternalErrorCodes.ODataMessageWriter_WriteProperty);
        }

        /// <summary>
        /// Write a service document with the specified <paramref name="defaultWorkspace"/> to the given stream. 
        /// This method creates an async buffered stream, writes the service document to it and returns 
        /// an <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteServiceDocument(Stream stream, ODataWorkspace defaultWorkspace)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteServiceDocument(xmlWriter, defaultWorkspace, this.settings.BaseUri, this.urlResolver, this.settings.WriterBehavior),
                (jsonWriter) => ODataJsonWriterUtils.WriteServiceDocument(jsonWriter, defaultWorkspace),
                null /*writeRawAction*/,
                InternalErrorCodes.ODataMessageWriter_WriteServiceDocument);
        }

        /// <summary>
        /// Write a metadata document to the given stream. 
        /// This method creates an async buffered stream, writes the metadata document to it and returns 
        /// an <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteMetadataDocument(Stream stream)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");
            Debug.Assert(this.model != null, "this.model != null");

            // Save the in-memory OData annotations to the model (i.e., turn them into serializable ones)
            this.model.SaveODataAnnotations();

            return this.WriteTopLevelContent(
                stream,
                null /*writeAtomAction*/,
                null /*writeJsonAction*/,
                (xmlWriter) => EdmxWriter.WriteEdmx(this.model, xmlWriter, EdmxTarget.OData),
                InternalErrorCodes.ODataMessageWriter_WriteMetadataDocument);
        }

        /// <summary>
        /// Write a top-level link to the given stream. This method creates an
        /// async buffered stream, writes the link to it and returns an <see cref="IODataWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the property to.</param>
        /// <param name="link">The entity reference link to write.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteEntityReferenceLinkImplementation(Stream stream, ODataEntityReferenceLink link)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteEntityReferenceLink(xmlWriter, this.settings.BaseUri, this.urlResolver, this.settings.WriterBehavior, link, true),
                (jsonWriter) => ODataJsonWriterUtils.WriteEntityReferenceLink(jsonWriter, this.settings.BaseUri, this.urlResolver, link, this.writingResponse),
                null /*writeRawAction*/,
                InternalErrorCodes.ODataMessageWriter_WriteEntityReferenceLink);
        }

        /// <summary>
        /// Write a top-level error to the given stream. This method creates an
        /// async buffered stream, writes the error to it and returns an <see cref="IODataWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the error to.</param>
        /// <param name="error">The error to write as message payload.</param>
        /// <param name="includeDebugInformation">A flag indicating whether error details should be written (in debug mode only) or not.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteTopLevelError(Stream stream, ODataError error, bool includeDebugInformation)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteError(xmlWriter, error, includeDebugInformation),
                (jsonWriter) => ODataJsonWriterUtils.WriteTopLevelError(jsonWriter, error, includeDebugInformation, this.writingResponse),
                null /*writeRawAction*/,
                InternalErrorCodes.ODataMessageWriter_WriteError);
        }

        /// <summary>
        /// Write a set of top-level links to the given stream. This method creates an
        /// async buffered stream, writes the links to it and returns an <see cref="IODataWriter"/>
        /// that can be used to flush and close/dispose the stream.
        /// </summary>
        /// <param name="stream">The stream to write the links to.</param>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        private IODataWriter WriteEntityReferenceLinksImplementation(Stream stream, ODataEntityReferenceLinks links)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            return this.WriteTopLevelContent(
                stream,
                (xmlWriter) => ODataAtomWriterUtils.WriteEntityReferenceLinks(xmlWriter, this.settings.BaseUri, this.urlResolver, this.settings.WriterBehavior, links),
                (jsonWriter) => ODataJsonWriterUtils.WriteEntityReferenceLinks(jsonWriter, this.settings.BaseUri, this.urlResolver, links, this.settings.Version.Value, this.writingResponse),
                null /*writeRawAction*/,
                InternalErrorCodes.ODataMessageWriter_WriteEntityReferenceLinks);
        }

        /// <summary>
        /// Method to write plain Xml (or JSON) content for top-level properties, top-level links and a single top-level link.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="writeAtomAction">An action that writes the payload in ATOM format.</param>
        /// <param name="writeJsonAction">An action that writes the payload in JSON format.</param>
        /// <param name="writeRawAction">An action that writes the payload in default/raw format.</param>
        /// <param name="internalErrorCode">An internal error code identifying the caller used in case of an internal error.</param>
        /// <returns>An <see cref="IODataWriter"/> that can be used to flush and close/dispose the stream.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The AsyncBufferedStream is only created here and will be disposed after we called FlushAsync.")]
        private IODataWriter WriteTopLevelContent(
            Stream stream, 
            Action<XmlWriter> writeAtomAction, 
            Action<JsonWriter> writeJsonAction,
            Action<XmlWriter> writeRawAction,
            InternalErrorCodes internalErrorCode)
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Expected payload kind, format and encoding to be set by now.");

            AsyncBufferedStream asyncBufferedStream = new AsyncBufferedStream(stream);
            IODataWriter asyncWriter;
            bool exceptionInTopLevelWrite = false;
            switch (this.format)
            {
                case ODataFormat.Json:
                    Debug.Assert(writeJsonAction != null, "Content negotiation supports JSON but no writer action is available.");

                    StreamWriter textWriter = new StreamWriter(asyncBufferedStream, this.encoding);
                    JsonWriter jsonWriter = new JsonWriter(textWriter, this.settings.Indent);
                    try
                    {
                        writeJsonAction(jsonWriter);
                    }
                    catch
                    {
                        exceptionInTopLevelWrite = true;
                        throw;
                    }
                    finally
                    {
                        asyncWriter = this.SetWriter(new AsyncWriter(asyncBufferedStream, textWriter, jsonWriter, exceptionInTopLevelWrite));
                    }

                    return asyncWriter;

                case ODataFormat.Atom:
                    Debug.Assert(writeAtomAction != null, "Content negotiation supports ATOM but no writer action is available.");

                    XmlWriter xmlWriter = ODataAtomWriterUtils.CreateXmlWriter(asyncBufferedStream, this.settings, this.encoding);
                    try
                    {
                        writeAtomAction(xmlWriter);
                    }
                    catch
                    {
                        exceptionInTopLevelWrite = true;
                        throw;
                    }
                    finally
                    {
                        asyncWriter = this.SetWriter(new AsyncWriter(asyncBufferedStream, xmlWriter, exceptionInTopLevelWrite));
                    }

                    return asyncWriter;

                case ODataFormat.Default:
                    Debug.Assert(writeRawAction != null, "Content negotiation supports RAW but no writer action is available.");
                    Debug.Assert(this.writerPayloadKind == ODataPayloadKind.MetadataDocument, "Only metadata is supported as top-level raw payload.");

                    // NOTE: we assume all top-level, non-atomic payloads use an Xml format. This is an assumption for
                    //       now based on the fact that we only support metadata today. Once we refactor the code to use 
                    //       the output context, this can be made more flexible.
                    XmlWriter rawWriter = ODataAtomWriterUtils.CreateXmlWriter(asyncBufferedStream, this.settings, this.encoding);
                    try
                    {
                        writeRawAction(rawWriter);
                    }
                    catch
                    {
                        exceptionInTopLevelWrite = true;
                        throw;
                    }
                    finally
                    {
                        asyncWriter = this.SetWriter(new AsyncWriter(asyncBufferedStream, rawWriter, exceptionInTopLevelWrite));
                    }

                    return asyncWriter;

                default:
                    throw new ODataException(Strings.General_InternalError(internalErrorCode));
            }
        }

        /// <summary>
        /// Helper class representing an async writer that can be either an ATOM writer, a JSON writer or a TEXT writer.
        /// </summary>
        private sealed class AsyncWriter : IODataWriter
        {
            /// <summary>true if an exception was thrown when ODataMessageWriter was writing a top level element, false otherwise.</summary>
            private readonly bool exceptionInTopLevelWrite;

            /// <summary>The async buffered stream for the writer.</summary>
            private AsyncBufferedStream asyncBufferedStream;

            /// <summary>Can either be the TEXT writer or the text writer backing the <see cref="jsonWriter"/>; null in the ATOM case.</summary>
            private TextWriter textWriter;

            /// <summary>The JSON writer; null in the ATOM case.</summary>
            private JsonWriter jsonWriter;

            /// <summary>The ATOM writer; null in the JSON case.</summary>
            private XmlWriter atomWriter;

            /// <summary>Flag to prevent writing more than one error to the payload.</summary>
            private bool writeErrorCalled;

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on an Xml ATOM writer.
            /// </summary>
            /// <param name="asyncBufferedStream">The async buffered stream to use.</param>
            /// <param name="atomWriter">The Xml writer to write to.</param>
            /// <param name="exceptionInTopLevelWrite">true if an exception was thrown when ODataMessageWriter was writing a top level element, false otherwise.</param>
            internal AsyncWriter(AsyncBufferedStream asyncBufferedStream, XmlWriter atomWriter, bool exceptionInTopLevelWrite)
            {
                Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
                Debug.Assert(atomWriter != null, "atomWriter != null");

                this.asyncBufferedStream = asyncBufferedStream;
                this.atomWriter = atomWriter;
                this.exceptionInTopLevelWrite = exceptionInTopLevelWrite;
            }

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on a JSON writer.
            /// </summary>
            /// <param name="asyncBufferedStream">The async buffered stream to use.</param>
            /// <param name="textWriter">The text writer backing the <see cref="jsonWriter"/>.</param>
            /// <param name="jsonWriter">The JSON writer to write to.</param>
            /// <param name="exceptionInTopLevelWrite">true if an exception was thrown when ODataMessageWriter was writing a top level element, false otherwise.</param>
            internal AsyncWriter(AsyncBufferedStream asyncBufferedStream, TextWriter textWriter, JsonWriter jsonWriter, bool exceptionInTopLevelWrite)
            {
                Debug.Assert(asyncBufferedStream != null, "asyncBufferedStream != null");
                Debug.Assert(textWriter != null, "textWriter != null");
                Debug.Assert(jsonWriter != null, "jsonWriter != null");

                this.asyncBufferedStream = asyncBufferedStream;
                this.textWriter = textWriter;
                this.jsonWriter = jsonWriter;
                this.exceptionInTopLevelWrite = exceptionInTopLevelWrite;
            }

            /// <summary>
            /// Creates a new <see cref="AsyncWriter"/> based on a TEXT writer.
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
            /// Synchronously flushes the write buffer to the underlying stream.
            /// </summary>
            void IODataWriter.FlushWriter()
            {
                this.FlushInnerWriters();
                this.asyncBufferedStream.FlushSync();
            }

#if ODATALIB_ASYNC
            /// <summary>
            /// Asynchronously flushes the write buffer to the underlying stream.
            /// </summary>
            /// <returns>A task instance that represents the asynchronous operation.</returns>
            Task IODataWriter.FlushWriterAsync()
            {
                this.FlushInnerWriters();

                // We guarantee that the combined task never is in Faulted state; even if AsyncBufferedStream.FlushAsync()
                // throws an exception we eat it in the continuation and return 'this'. The exception will still
                // be reported on the calling thread since AsyncBufferedStream.FlushAsync() uses the 'AttachedToParent' option.
                return this.asyncBufferedStream.FlushAsync()
                    .ContinueWith((task) => { return this; }, TaskContinuationOptions.ExecuteSynchronously);
            }
#endif

            /// <summary>
            /// Write an OData error.
            /// </summary>
            /// <param name='errorInstance'>The error information to write.</param>
            /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
            void IODataWriter.WriteError(ODataError errorInstance, bool includeDebugInformation)
            {
                if (this.writeErrorCalled)
                {
                    throw new ODataException(Strings.ODataMessageWriter_WriteErrorAlreadyCalled);
                }

                this.writeErrorCalled = true;
                ExceptionUtils.CheckArgumentNotNull(errorInstance, "errorInstance");

                if (this.atomWriter != null)
                {
                    // The ATOM writer case.
                    Debug.Assert(this.jsonWriter == null, "this.jsonWriter == null");
                    Debug.Assert(this.textWriter == null, "this.textWriter == null");
                    ODataAtomWriterUtils.WriteError(this.atomWriter, errorInstance, includeDebugInformation);
                }
                else if (this.jsonWriter != null)
                {
                    // The JSON writer case.
                    Debug.Assert(this.atomWriter == null, "this.atomWriter == null");
                    ODataJsonWriterUtils.WriteError(this.jsonWriter, errorInstance, includeDebugInformation);
                }
                else
                {
                    throw new ODataException(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues);
                }
            }

            /// <summary>
            /// Dispose the writer and the underlying stream.
            /// </summary>
            void IODataWriter.DisposeWriter()
            {
                try
                {
                    // TODO: jli -
                    // We always flush the writer on a successful write so the AsyncBufferedStream will always dispose
                    // successfully. However if an exception is thrown from any of the top-level write methods, there is un-flushed
                    // data in the AsyncBufferedStream and ODataMessageWriter.Dispose() will throw because of that. And since there
                    // is no Flush method on ODataMessageWriter, an error message telling user to flush is not helpful.
                    //
                    // We should consider these options:
                    // 1) The existing behavior is we clear the buffered data so AsyncBufferedStream.Dispose() will not throw.
                    // 2) Should we always flush any previously written data to the underlying stream in dispose?
                    // 3) Should we throw on dispose to tell user that they should write the error payload? Note that successfully
                    //    writing an error payload will also flush any privously written data.
                    if (this.exceptionInTopLevelWrite)
                    {
                        this.FlushInnerWriters();
                        this.asyncBufferedStream.Clear();
                    }

                    // Disposing the writers will also dispose the this.asyncBufferedStream since they own that stream.
                    // Note AsyncBufferedStream.Dispose will not dispose the real output stream underneath it.
                    Utils.TryDispose(this.jsonWriter);
                    Utils.TryDispose(this.textWriter);

                    // XmlWriter.Close() guarantees that well-formed xml is produced by injecting close elements
                    // if any is missing.  In the case of an exception, we want the stream to end at the close
                    // element </m:error>.  So we skip writer.Dispose here if we are in error state.
                    if (!this.writeErrorCalled && !this.exceptionInTopLevelWrite)
                    {
                        Utils.TryDispose(this.atomWriter);
                    }

                    Utils.TryDispose(this.asyncBufferedStream);
                }
                finally
                {
                    this.asyncBufferedStream = null;
                    this.jsonWriter = null;
                    this.atomWriter = null;
                    this.textWriter = null;
                }
            }

            /// <summary>
            /// Flushes all writers to the underlying stream.
            /// </summary>
            private void FlushInnerWriters()
            {
                if (this.jsonWriter != null)
                {
                    // JSON writer case, flushing the JSON writer will also flush the underlying text writer.
                    Debug.Assert(this.textWriter != null, "this.textWriter != null");
                    Debug.Assert(this.atomWriter == null, "this.atomWriter == null");
                    this.jsonWriter.Flush();
                }
                else if (this.textWriter != null)
                {
                    // TEXT writer case.
                    Debug.Assert(this.atomWriter == null, "this.atomWriter == null");
                    Debug.Assert(this.jsonWriter == null, "this.jsonWriter == null");
                    this.textWriter.Flush();
                }
                else if (this.atomWriter != null)
                {
                    // ATOM writer case.
                    Debug.Assert(this.jsonWriter == null, "this.jsonWriter == null");
                    Debug.Assert(this.textWriter == null, "this.textWriter == null");
                    this.atomWriter.Flush();
                }
            }
        }
    }
}
