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
    using System.Globalization;
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
        private bool isDisposed;

        /// <summary>The output context we're using to write the payload.</summary>
        /// <remarks>This is null until the first write operation is called.</remarks>
        private ODataOutputContext outputContext;

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

        /// <summary>The media type resolver to use when interpreting the content type.</summary>
        private MediaTypeResolver mediaTypeResolver;

        /// <summary>The <see cref="MediaType"/> of the payload to be written with this writer.</summary>
        /// <remarks>This is either set via the SetHeadersForPayload method or implicitly when one of the write (or writer creation) methods is called.</remarks>
        private MediaType mediaType;

        /// <summary> Creates a new <see cref="T:Microsoft.Data.OData.ODataMessageWriter" /> for the given request message. </summary>
        /// <param name="requestMessage">The request message for which to create the writer.</param>
        public ODataMessageWriter(IODataRequestMessage requestMessage)
            : this(requestMessage, null)
        {
        }

        /// <summary> Creates a new <see cref="T:Microsoft.Data.OData.ODataMessageWriter" /> for the given request message and message writer settings. </summary>
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

            // Clone the settings here so we can later modify them without changing the settings passed to us by the user
            this.settings = settings == null ? new ODataMessageWriterSettings() : new ODataMessageWriterSettings(settings);
            this.writingResponse = false; 
            this.urlResolver = requestMessage as IODataUrlResolver;
            this.model = model ?? EdmCoreModel.Instance;
            WriterValidationUtils.ValidateMessageWriterSettings(this.settings, this.writingResponse);
            this.message = new ODataRequestMessage(requestMessage, /*writing*/ true, this.settings.DisableMessageStreamDisposal, /*maxMessageSize*/ -1);
        }

        /// <summary> Creates a new <see cref="T:Microsoft.Data.OData.ODataMessageWriter" /> for the given response message. </summary>
        /// <param name="responseMessage">The response message for which to create the writer.</param>
        public ODataMessageWriter(IODataResponseMessage responseMessage)
            : this(responseMessage, null)
        {
        }

        /// <summary> Creates a new <see cref="T:Microsoft.Data.OData.ODataMessageWriter" /> for the given response message and message writer settings. </summary>
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

            // Clone the settings here so we can later modify them without changing the settings passed to us by the user
            this.settings = settings == null ? new ODataMessageWriterSettings() : new ODataMessageWriterSettings(settings);
            this.writingResponse = true;
            this.urlResolver = responseMessage as IODataUrlResolver;
            this.model = model ?? EdmCoreModel.Instance;
            WriterValidationUtils.ValidateMessageWriterSettings(this.settings, this.writingResponse);
            this.message = new ODataResponseMessage(responseMessage, /*writing*/ true, this.settings.DisableMessageStreamDisposal, /*maxMessageSize*/ -1);

            // If the Preference-Applied header on the response message contains an annotation filter, we set the filter
            // to the writer settings so that we would only write annotations that satisfy the filter.
            Debug.Assert(this.settings.ShouldIncludeAnnotation == null, "this.settings.AnnotationFilter == null");
            string annotationFilter = responseMessage.PreferenceAppliedHeader().AnnotationFilter;
            if (!string.IsNullOrEmpty(annotationFilter))
            {
                this.settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter(annotationFilter);
            }
        }

        /// <summary>
        /// The message writer settings to use when writing the message payload.
        /// </summary>
        internal ODataMessageWriterSettings Settings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.settings;
            }
        }

        /// <summary>
        /// The media type resolver to use when interpreting the content type.
        /// </summary>
        private MediaTypeResolver MediaTypeResolver
        {
            get
            {
                if (this.mediaTypeResolver == null)
                {
                    Debug.Assert(this.settings.Version.HasValue, "Version should have been set by now.");
                    this.mediaTypeResolver = MediaTypeResolver.GetWriterMediaTypeResolver(this.settings.Version.Value);
                }

                return this.mediaTypeResolver;
            }
        }

        /// <summary> Creates an <see cref="T:Microsoft.Data.OData.ODataWriter" /> to write a feed. </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataFeedWriter()
        {
            return CreateODataFeedWriter(/*entitySet*/null, /*entityType*/null);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        public ODataWriter CreateODataFeedWriter(IEdmEntitySet entitySet)
        {
            return CreateODataFeedWriter(entitySet, /*entityType*/null);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        public ODataWriter CreateODataFeedWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            this.VerifyCanCreateODataFeedWriter();
            return this.WriteToOutput(
                ODataPayloadKind.Feed,
                null /* verifyHeaders */,
                (context) => context.CreateODataFeedWriter(entitySet, entityType));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously creates an <see cref="T:Microsoft.Data.OData.ODataWriter" /> to write a feed. </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataFeedWriterAsync()
        {
            return CreateODataFeedWriterAsync(/*entitySet*/null, /*entityType*/null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataFeedWriterAsync(IEdmEntitySet entitySet)
        {
            return CreateODataFeedWriterAsync(entitySet, /*entityType*/null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataFeedWriterAsync(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            this.VerifyCanCreateODataFeedWriter();
            return this.WriteToOutputAsync(
                ODataPayloadKind.Feed,
                null /* verifyHeaders */,
                (context) => context.CreateODataFeedWriterAsync(entitySet, entityType));
        }
#endif

        /// <summary> Creates an <see cref="T:Microsoft.Data.OData.ODataWriter" /> to write an entry. </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataEntryWriter()
        {
            return CreateODataEntryWriter(/*entitySet*/null, /*entityType*/ null);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataEntryWriter(IEdmEntitySet entitySet)
        {
            return CreateODataEntryWriter(entitySet, /*entityType*/ null);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataEntryWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            this.VerifyCanCreateODataEntryWriter();
            return this.WriteToOutput(
                ODataPayloadKind.Entry,
                null /* verifyHeaders */,
                (context) => context.CreateODataEntryWriter(entitySet, entityType));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously creates an <see cref="T:Microsoft.Data.OData.ODataWriter" /> to write an entry. </summary>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataEntryWriterAsync()
        {
            return CreateODataEntryWriterAsync(/*entitySet*/null, /*entityType*/null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataEntryWriterAsync(IEdmEntitySet entitySet)
        {
            return CreateODataEntryWriterAsync(entitySet, /*entityType*/null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        public Task<ODataWriter> CreateODataEntryWriterAsync(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            this.VerifyCanCreateODataEntryWriter();
            return this.WriteToOutputAsync(
                ODataPayloadKind.Entry,
                null /* verifyHeaders */,
                (context) => context.CreateODataEntryWriterAsync(entitySet, entityType));
        }
#endif

        /// <summary> Creates an <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation). </summary>
        /// <returns>The created collection writer.</returns>
        public ODataCollectionWriter CreateODataCollectionWriter()
        {
            return this.CreateODataCollectionWriter(/*collectionType*/ null);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        public ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            this.VerifyCanCreateODataCollectionWriter(itemTypeReference);
            return this.WriteToOutput(
                ODataPayloadKind.Collection,
                null /* verifyHeaders */,
                (context) => context.CreateODataCollectionWriter(itemTypeReference));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously creates an <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation). </summary>
        /// <returns>A running task for the created collection writer.</returns>
        public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync()
        {
            return this.CreateODataCollectionWriterAsync(null);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>A running task for the created collection writer.</returns>
        public Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(IEdmTypeReference itemTypeReference)
        {
            this.VerifyCanCreateODataCollectionWriter(itemTypeReference);
            return this.WriteToOutputAsync(
                ODataPayloadKind.Collection,
                null /* verifyHeaders */,
                (context) => context.CreateODataCollectionWriterAsync(itemTypeReference));
        }
#endif

        /// <summary> Creates an <see cref="T:Microsoft.Data.OData.ODataBatchWriter" /> to write a batch of requests or responses. </summary>
        /// <returns>The created batch writer.</returns>
        public ODataBatchWriter CreateODataBatchWriter()
        {
            this.VerifyCanCreateODataBatchWriter();
            return this.WriteToOutput(
                ODataPayloadKind.Batch,
                null /* verifyHeaders */,
                (context) => context.CreateODataBatchWriter(this.batchBoundary));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously creates an <see cref="T:Microsoft.Data.OData.ODataBatchWriter" /> to write a batch of requests or responses. </summary>
        /// <returns>A running task for the created batch writer.</returns>
        public Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.VerifyCanCreateODataBatchWriter();
            return this.WriteToOutputAsync(
                ODataPayloadKind.Batch,
                null /* verifyHeaders */,
                (context) => context.CreateODataBatchWriterAsync(this.batchBoundary));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        public ODataParameterWriter CreateODataParameterWriter(IEdmFunctionImport functionImport)
        {
            this.VerifyCanCreateODataParameterWriter(functionImport);
            return this.WriteToOutput(
                ODataPayloadKind.Parameter,
                this.VerifyODataParameterWriterHeaders,
                (context) => context.CreateODataParameterWriter(functionImport));
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        public Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmFunctionImport functionImport)
        {
            this.VerifyCanCreateODataParameterWriter(functionImport);
            return this.WriteToOutputAsync(
                ODataPayloadKind.Parameter,
                this.VerifyODataParameterWriterHeaders,
                (context) => context.CreateODataParameterWriterAsync(functionImport));
        }
#endif

        /// <summary> Writes a service document with the specified <paramref name="defaultWorkspace" /> as the message payload. </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        public void WriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            this.VerifyCanWriteServiceDocument(defaultWorkspace);
            this.WriteToOutput(
                ODataPayloadKind.ServiceDocument,
                null /* verifyHeaders */,
                (context) => context.WriteServiceDocument(defaultWorkspace));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes a service document with the specified <paramref name="defaultWorkspace" /> as the message payload. </summary>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        public Task WriteServiceDocumentAsync(ODataWorkspace defaultWorkspace)
        {
            this.VerifyCanWriteServiceDocument(defaultWorkspace);
            return this.WriteToOutputAsync(
                ODataPayloadKind.ServiceDocument,
                null /* verifyHeaders */,
                (context) => context.WriteServiceDocumentAsync(defaultWorkspace));
        }
#endif

        /// <summary> Writes an <see cref="T:Microsoft.Data.OData.ODataProperty" /> as the message payload. </summary>
        /// <param name="property">The property to write.</param>
        public void WriteProperty(ODataProperty property)
        {
            this.VerifyCanWriteProperty(property);
            this.WriteToOutput(
                ODataPayloadKind.Property,
                null /* verifyHeaders */,
                (context) => context.WriteProperty(property));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes an <see cref="T:Microsoft.Data.OData.ODataProperty" /> as the message payload. </summary>
        /// <returns>A task representing the asynchronous operation of writing the property.</returns>
        /// <param name="property">The property to write</param>
        public Task WritePropertyAsync(ODataProperty property)
        {
            this.VerifyCanWriteProperty(property);
            return this.WriteToOutputAsync(
                ODataPayloadKind.Property,
                null /* verifyHeaders */,
                (context) => context.WritePropertyAsync(property));
        }
#endif

        /// <summary> Writes an <see cref="T:Microsoft.Data.OData.ODataError" /> as the message payload. </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation"> A flag indicating whether debug information (for example, the inner error from the <paramref name="error" />) should be included in the payload. This should only be used in debug scenarios. </param>
        public void WriteError(ODataError error, bool includeDebugInformation)
        {
            // We currently assume that the error is top-level if no create/write method has been called on this message writer.
            // It is possible that the user would create a Json writer, but writes nothing to it before writes the first error.
            // For example it is valid to call CreateEntryWriter() and then WriteError(). In that case the Json payload would
            // contain just an error without the Json wrapper.
            if (this.outputContext == null)
            {
                // Top-level error
                this.VerifyCanWriteTopLevelError(error);
                this.WriteToOutput(
                    ODataPayloadKind.Error,
                    null /* verifyHeaders */,
                    (context) => context.WriteError(error, includeDebugInformation));
                return;
            }

            // In-stream error
            this.VerifyCanWriteInStreamError(error);
            this.outputContext.WriteInStreamError(error, includeDebugInformation);
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes an <see cref="T:Microsoft.Data.OData.ODataError" /> as the message payload. </summary>
        /// <returns>A task representing the asynchronous operation of writing the error.</returns>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation"> A flag indicating whether debug information (for example, the inner error from the <paramref name="error" />) should be included in the payload. This should only be used in debug scenarios. </param>
        public Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
        {
            // We currently assume that the error is top-level if no create/write method has been called on this message writer.
            // It is possible that the user would create a Json writer, but writes nothing to it before writes the first error.
            // For example it is valid to call CreateEntryWriter() and then WriteError(). In that case the Json payload would
            // contain just an error without the Json wrapper.
            if (this.outputContext == null)
            {
                // Top-level error
                this.VerifyCanWriteTopLevelError(error);
                return this.WriteToOutputAsync(
                    ODataPayloadKind.Error,
                    null /* verifyHeaders */,
                    (context) => context.WriteErrorAsync(error, includeDebugInformation));
            }

            // In-stream error
            this.VerifyCanWriteInStreamError(error);
            return this.outputContext.WriteInStreamErrorAsync(error, includeDebugInformation);
        }
#endif

        /// <summary> Writes the result of a $links query as the message payload. </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        public void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            WriteEntityReferenceLinks(links, /*entitySet*/ null, /*navigationProperty*/ null);
        }

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        public void WriteEntityReferenceLinks(ODataEntityReferenceLinks links, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            this.VerifyCanWriteEntityReferenceLinks(links, navigationProperty);
            this.WriteToOutput(
                ODataPayloadKind.EntityReferenceLinks,
                () => this.VerifyEntityReferenceLinksHeaders(links),
                (context) => context.WriteEntityReferenceLinks(links, entitySet, navigationProperty));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes the result of a $links query as the message payload. </summary>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <param name="links">The entity reference links to write as message payload.</param>
        public Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            return WriteEntityReferenceLinksAsync(links, /*entitySet*/ null, /*navigationProperty*/ null);
        }

        /// <summary>
        /// Asynchronously writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        public Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            this.VerifyCanWriteEntityReferenceLinks(links, navigationProperty);
            return this.WriteToOutputAsync(
                ODataPayloadKind.EntityReferenceLinks,
                () => this.VerifyEntityReferenceLinksHeaders(links),
                (context) => context.WriteEntityReferenceLinksAsync(links, entitySet, navigationProperty));
        }
#endif

        /// <summary> Writes a singleton result of a $links query as the message payload. </summary>
        /// <param name="link">The entity reference link to write as the message payload.</param>
        public void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            WriteEntityReferenceLink(link, /*entitySet*/ null, /*navigationProperty*/ null);
        }

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference link is being written, or null if none is available.</param>
        public void WriteEntityReferenceLink(ODataEntityReferenceLink link, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            this.VerifyCanWriteEntityReferenceLink(link);
            this.WriteToOutput(
                ODataPayloadKind.EntityReferenceLink,
                null /* verifyHeaders */,
                (context) => context.WriteEntityReferenceLink(link, entitySet, navigationProperty));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes a singleton result of a $links query as the message payload. </summary>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <param name="link">The link result to write as the message payload.</param>
        public Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            return WriteEntityReferenceLinkAsync(link, /*entitySet*/null, /*navigationProperty*/ null);
        }

        /// <summary>
        /// Asynchronously writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property information for the link being written, or null if none is available.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        public Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            this.VerifyCanWriteEntityReferenceLink(link);
            return this.WriteToOutputAsync(
                ODataPayloadKind.EntityReferenceLink,
                null /* verifyHeaders */,
                (context) => context.WriteEntityReferenceLinkAsync(link, entitySet, navigationProperty));
        }
#endif

        /// <summary> Writes a single value as the message body. </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue(object value)
        {
            ODataPayloadKind payloadKind = this.VerifyCanWriteValue(value);
            this.WriteToOutput(
                payloadKind,
                null /* verifyHeaders */,
                (context) => context.WriteValue(value));
        }

#if ODATALIB_ASYNC
        /// <summary> Asynchronously writes a single value as the message body. </summary>
        /// <returns>A running task representing the writing of the value.</returns>
        /// <param name="value">The value to write.</param>
        public Task WriteValueAsync(object value)
        {
            ODataPayloadKind payloadKind = this.VerifyCanWriteValue(value);
            return this.WriteToOutputAsync(
                payloadKind,
                null /* verifyHeaders */,
                (context) => context.WriteValueAsync(value));
        }
#endif

        /// <summary> Writes the metadata document as the message body. </summary>
        public void WriteMetadataDocument()
        {
            this.VerifyCanWriteMetadataDocument();
            this.WriteToOutput(
                ODataPayloadKind.MetadataDocument,
                this.VerifyMetadataDocumentHeaders,
                (context) => context.WriteMetadataDocument());
        }

        /// <summary><see cref="M:System.IDisposable.Dispose()" /> implementation to cleanup unmanaged resources of the writer. </summary>
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
        /// <returns>The <see cref="ODataFormat"/> used for the specified <paramref name="payloadKind"/>.</returns>
        internal ODataFormat SetHeaders(ODataPayloadKind payloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(payloadKind != ODataPayloadKind.Unsupported, "payloadKind != ODataPayloadKind.Unsupported");

            this.writerPayloadKind = payloadKind;

            // Make sure we have a version set on the message writer settings; if none was specified on the settings, try to read
            // it from the message headers. If not specified in the headers either, fall back to a default.
            // NOTE: This method will potentially also set the data service version header.
            this.EnsureODataVersion();
            Debug.Assert(this.settings.Version.HasValue, "ODataVersion must have been set by now.");

            // Make sure we have a content-type and compute the format from the settings and/or the message headers.
            // NOTE: This method will potentially also set the content type header.
            this.EnsureODataFormatAndContentType();
            return this.format;
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
                this.settings.Version = ODataUtilsInternal.GetDataServiceVersion(this.message, ODataConstants.ODataDefaultProtocolVersion);
                Debug.Assert(this.settings.Version.HasValue, "The version must have been set by now.");
            }
            else
            {
                // Set the data service version
                ODataUtilsInternal.SetDataServiceVersion(this.message, this.settings);
            }

            // If the version is set to >= 3.0 we must not have a format behavior other than the default behavior.
            // Custom format behaviors for the WCF DS client and server are only supported in versions < 3.0
            if (this.settings.Version >= ODataVersion.V3 && this.settings.WriterBehavior.FormatBehaviorKind != ODataBehaviorKind.Default)
            {
                this.settings.WriterBehavior.UseDefaultFormatBehavior();
            }
        }

        /// <summary>
        /// Ensures that the OData format is computed and set; if needed, sets the content type
        /// header of the message.
        /// </summary>
        /// <remarks>
        /// This method computes and ensures that a content type exists and computes the 
        /// OData format from it. If a content type is explicitly specified through 
        /// <see cref="Microsoft.Data.OData.ODataUtils.SetHeadersForPayload(Microsoft.Data.OData.ODataMessageWriter, Microsoft.Data.OData.ODataPayloadKind)"/>
        /// or <see cref="Microsoft.Data.OData.ODataMessageWriterSettings.SetContentType(string, string)"/> it will be used. If no
        /// content type is specified in either place, the message headers are checked for
        /// a content type header.
        /// If the content type is computed from settings, the content type header is set on the message.
        /// </remarks>
        private void EnsureODataFormatAndContentType()
        {
            Debug.Assert(this.writerPayloadKind != ODataPayloadKind.Unsupported, "Writer payload kind should have been set by now.");

            string contentType = null;

            // If neither format nor accept headers were specified in the writer settings, try to read the content type from the message headers.
            if (!this.settings.UseFormat.HasValue)
            {
                contentType = this.message.GetHeader(ODataConstants.ContentTypeHeader);
                contentType = contentType == null ? null : contentType.Trim();
            }

            // If we found a content type header, use it. Otherwise use the default behavior.
            if (!string.IsNullOrEmpty(contentType))
            {
                ODataPayloadKind computedPayloadKind;
                this.format = MediaTypeUtils.GetFormatFromContentType(contentType, new ODataPayloadKind[] { this.writerPayloadKind }, this.MediaTypeResolver, out this.mediaType, out this.encoding, out computedPayloadKind, out this.batchBoundary);
                Debug.Assert(this.writerPayloadKind == computedPayloadKind, "The payload kinds must always match.");

                if (this.settings.HasJsonPaddingFunction())
                {
                    // Note: we change the media type being written from "application/json" to "text/javascript",
                    // but we internally keep "application/json" as the value of the mediaType field.
                    contentType = MediaTypeUtils.AlterContentTypeForJsonPadding(contentType);    
            
                    // Override the header even though they set it.
                    this.message.SetHeader(ODataConstants.ContentTypeHeader, contentType);
                }
            }
            else
            {
                // Determine the content type and format from the settings. Note that if neither format nor accept headers have been specified in the settings
                // we fall back to a default (of null accept headers).
                this.format = MediaTypeUtils.GetContentTypeFromSettings(this.settings, this.writerPayloadKind, this.MediaTypeResolver, out this.mediaType, out this.encoding);

                if (this.writerPayloadKind == ODataPayloadKind.Batch)
                {
                    // Note that this serves as verification only for now, since we only support a single content type and format for $batch payloads.
                    Debug.Assert(this.format == ODataFormat.Batch, "$batch should only support batch format since it's format independent.");
                    Debug.Assert(this.mediaType.FullTypeName == MimeConstants.MimeMultipartMixed, "$batch content type is currently only supported to be multipart/mixed.");

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

                    // Compute the content type (incl. charset) and set the Content-Type header on the response message
                    contentType = HttpUtils.BuildContentType(this.mediaType, this.encoding);
                }

                if (this.settings.HasJsonPaddingFunction())
                {
                    // Note: we change the media type being written from "application/json" to "text/javascript",
                    // but we internally keep "application/json" as the value of the mediaType field.
                    contentType = MediaTypeUtils.AlterContentTypeForJsonPadding(contentType);
                }   

                // NOTE: set the content type header here since all headers have to be set before getting the stream
                this.message.SetHeader(ODataConstants.ContentTypeHeader, contentType);
            }
        }

        /// <summary>
        /// Verifies that feed writer can be created.
        /// </summary>
        private void VerifyCanCreateODataFeedWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that entry writer can be created.
        /// </summary>
        private void VerifyCanCreateODataEntryWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that collection writer can be created.
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        [SuppressMessage("Microsoft.Naming", "CA2204:LiteralsShouldBeSpelledCorrectly", Justification = "Names are correct. String can't be localized after string freeze.")]
        private void VerifyCanCreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            if (itemTypeReference != null && !(itemTypeReference.IsPrimitive() || itemTypeReference.IsComplex()))
            {
                throw new ODataException(Strings.ODataMessageWriter_NonCollectionType(itemTypeReference.ODataFullName()));
            }

            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that batch writer can be created.
        /// </summary>
        private void VerifyCanCreateODataBatchWriter()
        {
            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that parameter writer can be created.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        private void VerifyCanCreateODataParameterWriter(IEdmFunctionImport functionImport)
        {
            if (this.writingResponse)
            {
                throw new ODataException(Strings.ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage);
            }

            if (functionImport != null)
            {
                // check that we have a user model
                if (!this.model.IsUserModel())
                {
                    throw new ODataException(Strings.ODataMessageWriter_CannotSpecifyFunctionImportWithoutModel);
                }
            }

            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies headers for parameter payload.
        /// </summary>
        private void VerifyODataParameterWriterHeaders()
        {
            ODataVersionChecker.CheckParameterPayload(this.settings.Version.Value);
        }

        /// <summary>
        /// Verifies that service document can be written.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write.</param>
        private void VerifyCanWriteServiceDocument(ODataWorkspace defaultWorkspace)
        {
            ExceptionUtils.CheckArgumentNotNull(defaultWorkspace, "defaultWorkspace");

            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_ServiceDocumentInRequest);
            }

            // VerifyWriterNotDisposedAndNotUsed changes the state of the message writer, it should be called after
            // we check the error conditions above.
            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that property can be written.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void VerifyCanWriteProperty(ODataProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            if (property.Value is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteStreamPropertyAsTopLevelProperty(property.Name));
            }

            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that top-level error can be written.
        /// </summary>
        /// <param name="error">The error to write.</param>
        private void VerifyCanWriteTopLevelError(ODataError error)
        {
            ExceptionUtils.CheckArgumentNotNull(error, "error");

            if (!this.writingResponse)
            {
                // errors can only be written for response messages
                throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
            }

            // Note that this verifies that the writer is only used once, so it also verifies that
            // there's nothing written before the error.
            this.VerifyWriterNotDisposedAndNotUsed();

            // Mark it as error written so that we don't allow writing in-stream errors after this.
            this.writeErrorCalled = true;
        }

        /// <summary>
        /// Verifies that in-stream error can be written.
        /// </summary>
        /// <param name="error">The error to write.</param>
        private void VerifyCanWriteInStreamError(ODataError error)
        {
            ExceptionUtils.CheckArgumentNotNull(error, "error");

            this.VerifyNotDisposed();

            if (!this.writingResponse)
            {
                // errors can only be written for response messages
                throw new ODataException(Strings.ODataMessageWriter_ErrorPayloadInRequest);
            }

            if (this.writeErrorCalled)
            {
                throw new ODataException(Strings.ODataMessageWriter_WriteErrorAlreadyCalled);
            }

            this.writeErrorCalled = true;

            // Need to mark the writer as already used, so that we don't allow any other calls to it.
            this.writeMethodCalled = true;
        }

        /// <summary>
        /// Verifies that entity reference links can be written.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        private void VerifyCanWriteEntityReferenceLinks(ODataEntityReferenceLinks links, IEdmNavigationProperty navigationProperty)
        {
            // NOTE: we decided to not stream links for now but only make writing them async.
            ExceptionUtils.CheckArgumentNotNull(links, "links");

            if (!this.writingResponse)
            {
                throw new ODataException(Strings.ODataMessageWriter_EntityReferenceLinksInRequestNotAllowed);
            }

            // Validate that the navigation property is a collection when trying to write multiple links.
            if (navigationProperty != null && navigationProperty.Type != null && !navigationProperty.Type.IsCollection())
            {
                throw new ODataException(Strings.ODataMessageWriter_EntityReferenceLinksWithSingletonNavPropNotAllowed(navigationProperty.Name));
            }

            // VerifyWriterNotDisposedAndNotUsed changes the state of the message writer, it should be called after
            // we check the error conditions above.
            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies headers for entity reference links.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void VerifyEntityReferenceLinksHeaders(ODataEntityReferenceLinks links)
        {
            Debug.Assert(this.settings != null && this.settings.Version.HasValue, "Settings must have been created and the version set.");

            if (links.Count.HasValue)
            {
                ODataVersionChecker.CheckCount(this.settings.Version.Value);
            }

            if (links.NextPageLink != null)
            {
                ODataVersionChecker.CheckNextLink(this.settings.Version.Value);
            }
        }

        /// <summary>
        /// Verifies that entity reference link can be written.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        private void VerifyCanWriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            ExceptionUtils.CheckArgumentNotNull(link, "link");

            this.VerifyWriterNotDisposedAndNotUsed();
        }

        /// <summary>
        /// Verifies that value can be written.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>The payload kind to use when writing this value.</returns>
        private ODataPayloadKind VerifyCanWriteValue(object value)
        {
            if (value == null)
            {
                throw new ODataException(Strings.ODataMessageWriter_CannotWriteNullInRawFormat);
            }

            this.VerifyWriterNotDisposedAndNotUsed();

            // We cannot use AtomValueUtils.TryConvertPrimitiveToString for all cases since binary values are
            // converted into unencoded byte streams in the raw format 
            // (as opposed to base64 encoded byte streams in the AtomValueUtils); see OIPI 2.2.6.4.1.
            return value is byte[] ? ODataPayloadKind.BinaryValue : ODataPayloadKind.Value;
        }

        /// <summary>
        /// Verifies that metadata document can be written.
        /// </summary>
        private void VerifyCanWriteMetadataDocument()
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
        }

        /// <summary>
        /// Verifies headers for metadata document.
        /// </summary>
        private void VerifyMetadataDocumentHeaders()
        {
            // set the data service version on the model (if not already present)
            Version dataServiceVersion = this.model.GetDataServiceVersion();
            if (dataServiceVersion == null)
            {
                dataServiceVersion = this.settings.Version.Value.ToDataServiceVersion();
                this.model.SetDataServiceVersion(dataServiceVersion);
            }
        }

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
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        private void Dispose(bool disposing)
        {
            this.isDisposed = true;
            if (disposing)
            {
                try
                {
                    if (this.outputContext != null)
                    {
                        this.outputContext.Dispose();
                    }
                }
                finally
                {
                    this.outputContext = null;
                }
            }
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
        /// Creates an output context and invokes a write operation on it.
        /// </summary>
        /// <param name="payloadKind">The payload kind to write.</param>
        /// <param name="verifyHeaders">Optional action which will be called after the headers has been verified to perform payload specific verification.</param>
        /// <param name="writeAction">The write operation to invoke on the output.</param>
        private void WriteToOutput(ODataPayloadKind payloadKind, Action verifyHeaders, Action<ODataOutputContext> writeAction)
        {
            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(payloadKind);

            if (verifyHeaders != null)
            {
                verifyHeaders();
            }

            // Create the output context
            this.outputContext = this.format.CreateOutputContext(
                this.message,
                this.mediaType,
                this.encoding,
                this.settings,
                this.writingResponse,
                this.model,
                this.urlResolver);
            writeAction(this.outputContext);
        }

        /// <summary>
        /// Creates an output context and invokes a write operation on it.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the write operation.</typeparam>
        /// <param name="payloadKind">The payload kind to write.</param>
        /// <param name="verifyHeaders">Optional action which will be called after the headers has been verified to perform payload specific verification.</param>
        /// <param name="writeFunc">The write operation to invoke on the output.</param>
        /// <returns>The result of the write operation.</returns>
        private TResult WriteToOutput<TResult>(ODataPayloadKind payloadKind, Action verifyHeaders, Func<ODataOutputContext, TResult> writeFunc)
        {
            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(payloadKind);

            if (verifyHeaders != null)
            {
                verifyHeaders();
            }

            // Create the output context
            this.outputContext = this.format.CreateOutputContext(
                this.message, 
                this.mediaType,
                this.encoding,
                this.settings,
                this.writingResponse,
                this.model,
                this.urlResolver);
            return writeFunc(this.outputContext);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Creates an output context and invokes a write operation on it.
        /// </summary>
        /// <param name="payloadKind">The payload kind to write.</param>
        /// <param name="verifyHeaders">Optional action which will be called after the headers has been verified to perform payload specific verification.</param>
        /// <param name="writeAsyncAction">The write operation to invoke on the output.</param>
        /// <returns>Task which represents the pending write operation.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The output context is disposed by Dispose on the writer.")]
        private Task WriteToOutputAsync(ODataPayloadKind payloadKind, Action verifyHeaders, Func<ODataOutputContext, Task> writeAsyncAction)
        {
            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(payloadKind);

            if (verifyHeaders != null)
            {
                verifyHeaders();
            }

            // Create the output context
            return this.format.CreateOutputContextAsync(
                this.message,
                this.mediaType,
                this.encoding,
                this.settings,
                this.writingResponse,
                this.model,
                this.urlResolver)

                .FollowOnSuccessWithTask(
                    (createOutputContextTask) =>
                    {
                        this.outputContext = createOutputContextTask.Result;
                        return writeAsyncAction(this.outputContext);
                    });
        }

        /// <summary>
        /// Creates an output context and invokes a write operation on it.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the write operation.</typeparam>
        /// <param name="payloadKind">The payload kind to write.</param>
        /// <param name="verifyHeaders">Optional action which will be called after the headers has been verified to perform payload specific verification.</param>
        /// <param name="writeFunc">The write operation to invoke on the output.</param>
        /// <returns>Task which represents the pending write operation.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The output context is disposed by Dispose on the writer.")]
        private Task<TResult> WriteToOutputAsync<TResult>(ODataPayloadKind payloadKind, Action verifyHeaders, Func<ODataOutputContext, Task<TResult>> writeFunc)
        {
            // Set the content type header here since all headers have to be set before getting the stream
            this.SetOrVerifyHeaders(payloadKind);

            if (verifyHeaders != null)
                    {
                verifyHeaders();
            }

            // Create the output context
            return this.format.CreateOutputContextAsync(
                this.message,
                this.mediaType,
                this.encoding,
                this.settings,
                this.writingResponse,
                this.model,
                this.urlResolver)

                .FollowOnSuccessWithTask(
                    (createOutputContextTask) =>
                    {
                        this.outputContext = createOutputContextTask.Result;
                        return writeFunc(this.outputContext);
                    });
            }
#endif
    }
}
