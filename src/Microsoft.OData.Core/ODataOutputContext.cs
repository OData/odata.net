//---------------------------------------------------------------------
// <copyright file="ODataOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for all output contexts, defines the interface
    /// to be implemented by the specific formats.
    /// </summary>
    public abstract class ODataOutputContext : IDisposable
    {
        /// <summary>The format for this output context.</summary>
        private readonly ODataFormat format;

        /// <summary>The message writer settings to be used for writing.</summary>
        private readonly ODataMessageWriterSettings messageWriterSettings;

        /// <summary>Set to true if this context is writing a response payload.</summary>
        private readonly bool writingResponse;

        /// <summary>true if the input should be written synchronously; false if it should be written asynchronously.</summary>
        private readonly bool synchronous;

        /// <summary>The model to use.</summary>
        private readonly IEdmModel model;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs written to the payload.</summary>
        private readonly IODataPayloadUriConverter payloadUriConverter;

        /// <summary>The optional dependency injection container to get related services for message writing.</summary>
        private readonly IServiceProvider container;

        /// <summary>The type resolver to use.</summary>
        private readonly EdmTypeResolver edmTypeResolver;

        /// <summary>The payload value converter to use.</summary>
        private readonly ODataPayloadValueConverter payloadValueConverter;

        /// <summary>The writer validator used in writing.</summary>
        private readonly IWriterValidator writerValidator;

        /// <summary>
        /// The simplified options used in writing.
        /// </summary>
        private readonly ODataSimplifiedOptions odataSimplifiedOptions;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        protected ODataOutputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(format, "format");
            ExceptionUtils.CheckArgumentNotNull(messageWriterSettings, "messageWriterSettings");

            this.format = format;
            this.messageWriterSettings = messageWriterSettings;
            this.writingResponse = messageInfo.IsResponse;
            this.synchronous = !messageInfo.IsAsync;
            this.model = messageInfo.Model ?? EdmCoreModel.Instance;
            this.payloadUriConverter = messageInfo.PayloadUriConverter;
            this.container = messageInfo.Container;
            this.edmTypeResolver = EdmTypeWriterResolver.Instance;
            this.payloadValueConverter = ODataPayloadValueConverter.GetPayloadValueConverter(this.container);
            this.writerValidator = messageWriterSettings.Validator;
            this.odataSimplifiedOptions = ODataSimplifiedOptions.GetODataSimplifiedOptions(this.container, messageWriterSettings.Version);
        }

        /// <summary>
        /// The message writer settings to be used for writing.
        /// </summary>
        public ODataMessageWriterSettings MessageWriterSettings
        {
            get
            {
                return this.messageWriterSettings;
            }
        }

        /// <summary>
        /// Set to true if a response is being written.
        /// </summary>
        public bool WritingResponse
        {
            get
            {
                return this.writingResponse;
            }
        }

        /// <summary>
        /// true if the output should be written synchronously; false if it should be written asynchronously.
        /// </summary>
        public bool Synchronous
        {
            get
            {
                return this.synchronous;
            }
        }

        /// <summary>
        /// The model to use or null if no metadata is available.
        /// </summary>
        public IEdmModel Model
        {
            get
            {
                Debug.Assert(this.model != null, "this.model != null");
                return this.model;
            }
        }

        /// <summary>
        /// The optional URL converter to perform custom URL conversion for URLs written to the payload.
        /// </summary>
        public IODataPayloadUriConverter PayloadUriConverter
        {
            get
            {
                return this.payloadUriConverter;
            }
        }

        /// <summary>
        /// The optional dependency injection container to get related services for message writing.
        /// </summary>
        internal IServiceProvider Container
        {
            get
            {
                return this.container;
            }
        }

        /// <summary>
        /// The type resolver to use.
        /// </summary>
        internal EdmTypeResolver EdmTypeResolver
        {
            get
            {
                return this.edmTypeResolver;
            }
        }

        /// <summary>
        /// The payload value converter to use
        /// </summary>
        internal ODataPayloadValueConverter PayloadValueConverter
        {
            get
            {
                return this.payloadValueConverter;
            }
        }

        /// <summary>
        /// The writer validator used in writing.
        /// </summary>
        internal IWriterValidator WriterValidator
        {
            get
            {
                return this.writerValidator;
            }
        }

        /// <summary>
        /// The ODataSimplifiedOptions used in writing
        /// </summary>
        internal ODataSimplifiedOptions ODataSimplifiedOptions
        {
            get
            {
                return this.odataSimplifiedOptions;
            }
        }

        /// <summary>
        /// IDisposable.Dispose() implementation to cleanup unmanaged resources of the context.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataWriter CreateODataResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataWriter> CreateODataResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType entityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a delta resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataWriter CreateODataDeltaResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a delta resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType entityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataWriter CreateODataResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataWriter> CreateODataResourceWriterAsync(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>A running task for the created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(IEdmTypeReference itemTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource into a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataWriter CreateODataUriParameterResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource into a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataWriter> CreateODataUriParameterResourceWriterAsync(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource set into a Uri operation parameter.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataWriter CreateODataUriParameterResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously Creates an <see cref="ODataWriter" /> to write a resource set into a Uri operation parameter.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataWriter> CreateODataUriParameterResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual ODataParameterWriter CreateODataParameterWriter(IEdmOperation operation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public virtual Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmOperation operation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="odataProperty">The OData property to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        public virtual void WriteProperty(ODataProperty odataProperty)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="odataProperty">The OData property to write</param>
        /// <returns>A task representing the asynchronous operation of writing the OData property.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        public virtual Task WritePropertyAsync(ODataProperty odataProperty)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }
#endif

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="odataError">The OData error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="odataError"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        public virtual void WriteError(ODataError odataError, bool includeDebugInformation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="odataError">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="odataError"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>A task representing the asynchronous operation of writing the error.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        public virtual Task WriteErrorAsync(ODataError odataError, bool includeDebugInformation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
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
        internal virtual void WriteInStreamError(ODataError error, bool includeDebugInformation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }

#if PORTABLELIB
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
        internal virtual Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataAsynchronousWriter" /> to write an async response.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal virtual ODataAsynchronousWriter CreateODataAsynchronousWriter()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataAsynchronousWriter" /> to write an async response.
        /// </summary>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal virtual Task<ODataAsynchronousWriter> CreateODataAsynchronousWriterAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataDeltaWriter" /> to write a delta response.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal virtual ODataDeltaWriter CreateODataDeltaWriter(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataDeltaWriter" /> to write a delta response.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal virtual Task<ODataDeltaWriter> CreateODataDeltaWriterAsync(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>
        /// The write must flush the output when it's finished (inside the last Write call).
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual ODataBatchWriter CreateODataBatchWriter()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>
        /// The write must flush the output when it's finished (inside the last Write call).
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }
#endif

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write in the service document.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal virtual void WriteServiceDocument(ODataServiceDocument serviceDocument)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write in the service document.</param>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal virtual Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }
#endif

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal virtual void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal virtual Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }
#endif

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal virtual void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal virtual Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }
#endif

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal virtual void WriteValue(object value)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns>A running task representing the writing of the value.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal virtual Task WriteValueAsync(object value)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }
#endif

        /// <summary>
        /// Writes the metadata document as the message body.
        /// </summary>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal virtual void WriteMetadataDocument()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.MetadataDocument);
        }

        /// <summary>
        /// Asserts that the input context was created for synchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertSynchronous()
        {
#if DEBUG
            Debug.Assert(this.synchronous, "The method should only be called on a synchronous output context.");
#endif
        }

        /// <summary>
        /// Asserts that the input context was created for asynchronous operation.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to access this in debug only.")]
        internal void AssertAsynchronous()
        {
#if DEBUG
            Debug.Assert(!this.synchronous, "The method should only be called on an asynchronous output context.");
#endif
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Creates an exception which reports that the specified payload kind if not support by this format.
        /// </summary>
        /// <param name="payloadKind">The payload kind which is not supported.</param>
        /// <returns>An exception to throw.</returns>
        private ODataException CreatePayloadKindNotSupportedException(ODataPayloadKind payloadKind)
        {
            return new ODataException(Strings.ODataOutputContext_UnsupportedPayloadKindForFormat(this.format.ToString(), payloadKind.ToString()));
        }
    }
}
