//---------------------------------------------------------------------
// <copyright file="ODataInputContext.cs" company="Microsoft">
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
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Base class for all input contexts, defines the interface
    /// to be implemented by the specific formats.
    /// </summary>
    public abstract class ODataInputContext : IDisposable
    {
        /// <summary>The format for this input context.</summary>
        private readonly ODataFormat format;

        /// <summary>The message reader settings to be used for reading.</summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>Set to true if this context is reading a response payload.</summary>
        private readonly bool readingResponse;

        /// <summary>true if the input should be read synchronously; false if it should be read asynchronously.</summary>
        private readonly bool synchronous;

        /// <summary>The optional URL resolver to perform custom URL resolution for URLs read from the payload.</summary>
        private readonly IODataPayloadUriConverter payloadUriConverter;

        /// <summary>The optional dependency injection container to get related services for message reading.</summary>
        private readonly IServiceProvider container;

        /// <summary>The model to use.</summary>
        private readonly IEdmModel model;

        /// <summary>The type resolver to use.</summary>
        private readonly EdmTypeResolver edmTypeResolver;

        /// <summary>The payload value converter to use.</summary>
        private readonly ODataPayloadValueConverter payloadValueConverter;

        /// <summary>
        /// The ODataSimplifiedOptions used in reader.
        /// </summary>
        private readonly ODataSimplifiedOptions odataSimplifiedOptions;

        /// <summary>Set to true if the input was disposed.</summary>
        private bool disposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        protected ODataInputContext(
            ODataFormat format,
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
        {
            ExceptionUtils.CheckArgumentNotNull(format, "format");
            ExceptionUtils.CheckArgumentNotNull(messageInfo, "messageInfo");
            ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");

            this.format = format;
            this.messageReaderSettings = messageReaderSettings;
            this.readingResponse = messageInfo.IsResponse;
            this.synchronous = !messageInfo.IsAsync;
            this.model = messageInfo.Model ?? EdmCoreModel.Instance;
            this.payloadUriConverter = messageInfo.PayloadUriConverter;
            this.container = messageInfo.Container;
            this.edmTypeResolver = new EdmTypeReaderResolver(this.Model, this.MessageReaderSettings.ClientCustomTypeResolver);
            this.payloadValueConverter = ODataPayloadValueConverter.GetPayloadValueConverter(this.container);
            this.odataSimplifiedOptions = ODataSimplifiedOptions.GetODataSimplifiedOptions(this.container, messageReaderSettings.Version);
        }

        /// <summary>
        /// The message reader settings to be used for reading.
        /// </summary>
        public ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                return this.messageReaderSettings;
            }
        }

        /// <summary>
        /// Set to true if a response is being read.
        /// </summary>
        public bool ReadingResponse
        {
            get
            {
                return this.readingResponse;
            }
        }

        /// <summary>
        /// true if the input should be read synchronously; false if it should be read asynchronously.
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
                return this.model;
            }
        }

        /// <summary>
        /// The optional URL converter to perform custom URL conversion for URLs read from the payload.
        /// </summary>
        public IODataPayloadUriConverter PayloadUriConverter
        {
            get
            {
                return this.payloadUriConverter;
            }
        }

        /// <summary>
        /// The optional dependency injection container to get related services for message reading.
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
        /// The payload value converter to use.
        /// </summary>
        internal ODataPayloadValueConverter PayloadValueConverter
        {
            get
            {
                return this.payloadValueConverter;
            }
        }

        /// <summary>
        /// The ODataSimplifiedOptions used in reader.
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
            if (this.disposed)
            {
                return;
            }

            this.Dispose(true);
            GC.SuppressFinalize(this);
            this.disposed = true;
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public virtual ODataReader CreateResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public virtual Task<ODataReader> CreateResourceSetReaderAsync(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a delta resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public virtual ODataReader CreateDeltaResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a delta resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public virtual Task<ODataReader> CreateDeltaResourceSetReaderAsync(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the resource to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public virtual ODataReader CreateResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the resource to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public virtual Task<ODataReader> CreateResourceReaderAsync(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Resource);
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>The newly created <see cref="ODataCollectionReader"/>.</returns>
        public virtual ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataCollectionReader"/>.</returns>
        public virtual Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Collection);
        }
#endif

        /// <summary>
        /// Read the EDM structural property from the input and
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="edmStructuralProperty">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        public virtual ODataProperty ReadProperty(IEdmStructuralProperty edmStructuralProperty, IEdmTypeReference expectedPropertyTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read the EDM structural property from the input and
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="edmStructuralProperty">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        public virtual Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty edmStructuralProperty, IEdmTypeReference expectedPropertyTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Property);
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        public virtual ODataError ReadError()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        public virtual Task<ODataError> ReadErrorAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Error);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource in a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected resource type for the resource to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public virtual ODataReader CreateUriParameterResourceReader(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource in a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the resource to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public virtual Task<ODataReader> CreateUriParameterResourceReaderAsync(IEdmNavigationSource navigationSource, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource set in a Uri operation parameter.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public virtual ODataReader CreateUriParameterResourceSetReader(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource set in a Uri operation parameter.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read resources for.</param>
        /// <param name="expectedResourceType">The expected structured type for the items in the resource set.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public virtual Task<ODataReader> CreateUriParameterResourceSetReaderAsync(IEdmEntitySetBase entitySet, IEdmStructuredType expectedResourceType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif
        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        public virtual ODataParameterReader CreateParameterReader(IEdmOperation operation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        public virtual Task<ODataParameterReader> CreateParameterReaderAsync(IEdmOperation operation)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Parameter);
        }
#endif

        /// <summary>
        /// Create an <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataAsynchronousReader"/>.</returns>
        internal virtual ODataAsynchronousReader CreateAsynchronousReader()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create an <see cref="ODataAsynchronousReader"/>.
        /// </summary>
        /// <returns>Task which when completed returns the newly created <see cref="ODataAsynchronousReader"/>.</returns>
        internal virtual Task<ODataAsynchronousReader> CreateAsynchronousReaderAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Asynchronous);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataDeltaReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>The newly created <see cref="ODataDeltaReader"/>.</returns>
        internal virtual ODataDeltaReader CreateDeltaReader(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataDeltaReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataDeltaReader"/>.</returns>
        internal virtual Task<ODataDeltaReader> CreateDeltaReaderAsync(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ResourceSet);
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <returns>The newly created <see cref="ODataBatchReader"/>.</returns>
        /// <remarks>
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual ODataBatchReader CreateBatchReader()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataBatchReader"/>.
        /// </summary>
        /// <returns>Task which when completed returns the newly created <see cref="ODataBatchReader"/>.</returns>
        /// <remarks>
        /// Since we don't want to support batch format extensibility (at least not yet) this method should remain internal.
        /// </remarks>
        internal virtual Task<ODataBatchReader> CreateBatchReaderAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Batch);
        }
#endif

        /// <summary>
        /// Read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        internal virtual ODataServiceDocument ReadServiceDocument()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a service document.
        /// This method reads the service document from the input and returns
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        internal virtual Task<ODataServiceDocument> ReadServiceDocumentAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.ServiceDocument);
        }
#endif

        /// <summary>
        /// Read a metadata document.
        /// This method reads the metadata document from the input and returns
        /// an <see cref="IEdmModel"/> that represents the read metadata document.
        /// </summary>
        /// <param name="getReferencedModelReaderFunc">The function to load referenced model xml. If null, will stop loading the referenced models. Normally it should throw no exception.</param>
        /// <returns>An <see cref="IEdmModel"/> representing the read metadata document.</returns>
        internal virtual IEdmModel ReadMetadataDocument(Func<Uri, XmlReader> getReferencedModelReaderFunc)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.MetadataDocument);
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal virtual ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal virtual Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLinks);
        }
#endif

        /// <summary>
        /// Read a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal virtual ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal virtual Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.EntityReferenceLink);
        }
#endif

        /// <summary>
        /// Read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>An <see cref="object"/> representing the read value.</returns>
        internal virtual object ReadValue(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a top-level value.
        /// </summary>
        /// <param name="expectedPrimitiveTypeReference">The expected type reference for the value to be read; null if no expected type is available.</param>
        /// <returns>Task which when completed returns an <see cref="object"/> representing the read value.</returns>
        internal virtual Task<object> ReadValueAsync(IEdmPrimitiveTypeReference expectedPrimitiveTypeReference)
        {
            throw this.CreatePayloadKindNotSupportedException(ODataPayloadKind.Value);
        }
#endif

        /// <summary>
        /// Check if the object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the object has already been disposed.</exception>
        internal void VerifyNotDisposed()
        {
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
#if DEBUG
            Debug.Assert(!this.synchronous, "The method should only be called on an asynchronous input context.");
#endif
        }

        /// <summary>
        /// Creates a new instance of a duplicate property names checker.
        /// </summary>
        /// <returns>The newly created instance of duplicate property names checker.</returns>
        internal PropertyAndAnnotationCollector CreatePropertyAndAnnotationCollector()
        {
            return messageReaderSettings.Validator.CreatePropertyAndAnnotationCollector();
        }

        /// <summary>
        /// Method to use the custom URL resolver to resolve a base URI and a payload URI.
        /// This method returns null if not custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        internal Uri ResolveUri(Uri baseUri, Uri payloadUri)
        {
            Debug.Assert(payloadUri != null, "uri != null");

            if (this.PayloadUriConverter != null)
            {
                return this.PayloadUriConverter.ConvertPayloadUri(baseUri, payloadUri);
            }

            return null;
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
            return new ODataException(Strings.ODataInputContext_UnsupportedPayloadKindForFormat(this.format.ToString(), payloadKind.ToString()));
        }
    }
}
