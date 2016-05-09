//---------------------------------------------------------------------
// <copyright file="ODataJsonLightInputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    // ReSharper disable RedundantUsingDirective
    using ODataErrorStrings = Microsoft.OData.Strings;
    // ReSharper restore RedundantUsingDirective
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for JsonLight OData format.
    /// </summary>
    internal sealed class ODataJsonLightInputContext : ODataInputContext
    {
        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        private readonly JsonLightMetadataLevel metadataLevel;

        /// <summary>The text reader created for the input stream.</summary>
        /// <remarks>
        /// The ODataJsonLightInputContext instance owns the textReader instance and thus disposes it. 
        /// We further set this field to null when the input is disposed and use it for checks whether the instance has already been disposed.
        /// </remarks>
        private TextReader textReader;

        /// <summary>The JSON reader to read from.</summary>
        private BufferingJsonReader jsonReader;

        /// <summary>Constructor.</summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        public ODataJsonLightInputContext(
            ODataMessageInfo messageInfo,
            ODataMessageReaderSettings messageReaderSettings)
            : base(ODataFormat.Json, messageInfo, messageReaderSettings)
        {
            Debug.Assert((messageInfo.MessageStream != null) ^ (messageInfo.TextReader != null),
                "Only one of MessageStream and TextReader can be set");
            Debug.Assert(messageInfo.MediaType != null, "messageInfo.MediaType != null");

            try
            {
                this.textReader = messageInfo.TextReader ?? CreateTextReaderForMessageStreamConstructor(messageInfo.MessageStream, messageInfo.Encoding);
                var innerReader = CreateJsonReader(messageInfo.Container, this.textReader, messageInfo.MediaType.HasIeee754CompatibleSetToTrue());
                if (messageInfo.MediaType.HasStreamingSetToTrue())
                {
                    this.jsonReader = new BufferingJsonReader(
                        innerReader,
                        JsonLightConstants.ODataErrorPropertyName,
                        messageReaderSettings.MessageQuotas.MaxNestingDepth);
                }
                else
                {
                    // If we have a non-streaming Json Light content type we need to use the re-ordering Json reader
                    this.jsonReader = new ReorderingJsonReader(innerReader, messageReaderSettings.MessageQuotas.MaxNestingDepth);
                }
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && this.textReader != null)
                {
                    this.textReader.Dispose();
                }

                throw;
            }

            // dont know how to get MetadataDocumentUri uri here, messageReaderSettings do not have one
            // Uri metadataDocumentUri = messageReaderSettings..MetadataDocumentUri == null ? null : messageReaderSettings.MetadataDocumentUri.BaseUri;
            // the uri here is used here to create the FullMetadataLevel can pass null in
            this.metadataLevel = JsonLightMetadataLevel.Create(messageInfo.MediaType, null, messageInfo.Model, messageInfo.IsResponse);
        }

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        public JsonLightMetadataLevel MetadataLevel
        {
            get
            {
                return this.metadataLevel;
            }
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        public BufferingJsonReader JsonReader
        {
            get
            {
                Debug.Assert(this.jsonReader != null, "Trying to get JsonReader while none is available.");
                return this.jsonReader;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the resource set.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public override ODataReader CreateResourceSetReader(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            return this.CreateResourceSetReaderImplementation(entitySet, expectedBaseEntityType);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the resource set.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public override Task<ODataReader> CreateResourceSetReaderAsync(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateResourceSetReaderImplementation(entitySet, expectedBaseEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the resource to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        public override ODataReader CreateResourceReader(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(navigationSource, expectedEntityType);

            return this.CreateResourceReaderImplementation(navigationSource, expectedEntityType);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the resource to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        public override Task<ODataReader> CreateResourceReaderAsync(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(navigationSource, expectedEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateResourceReaderImplementation(navigationSource, expectedEntityType));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        public override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateCollectionReader(expectedItemTypeReference);

            return this.CreateCollectionReaderImplementation(expectedItemTypeReference);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly create <see cref="ODataCollectionReader"/>.</returns>
        public override Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateCollectionReader(expectedItemTypeReference);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateCollectionReaderImplementation(expectedItemTypeReference));
        }
#endif

        /// <summary>
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        public override ODataProperty ReadProperty(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            this.AssertSynchronous();
            this.VerifyCanReadProperty();

            ODataJsonLightPropertyAndValueDeserializer jsonLightPropertyAndValueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(this);
            return jsonLightPropertyAndValueDeserializer.ReadTopLevelProperty(expectedPropertyTypeReference);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        public override Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            this.AssertAsynchronous();
            this.VerifyCanReadProperty();

            ODataJsonLightPropertyAndValueDeserializer jsonLightPropertyAndValueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(this);
            return jsonLightPropertyAndValueDeserializer.ReadTopLevelPropertyAsync(expectedPropertyTypeReference);
        }
#endif

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        public override ODataError ReadError()
        {
            this.AssertSynchronous();

            ODataJsonLightErrorDeserializer jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(this);
            return jsonLightErrorDeserializer.ReadTopLevelError();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        public override Task<ODataError> ReadErrorAsync()
        {
            this.AssertAsynchronous();

            ODataJsonLightErrorDeserializer jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(this);
            return jsonLightErrorDeserializer.ReadTopLevelErrorAsync();
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        public override ODataParameterReader CreateParameterReader(IEdmOperation operation)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateParameterReader(operation);

            return this.CreateParameterReaderImplementation(operation);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        public override Task<ODataParameterReader> CreateParameterReaderAsync(IEdmOperation operation)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateParameterReader(operation);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateParameterReaderImplementation(operation));
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        public IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            this.VerifyCanDetectPayloadKind();

            ODataJsonLightPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKind(detectionInfo);
        }

#if PORTABLELIB
        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task which returns an enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        public Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            this.VerifyCanDetectPayloadKind();

            ODataJsonLightPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKindAsync(detectionInfo);
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataDeltaReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>The newly created <see cref="ODataDeltaReader"/>.</returns>
        internal override ODataDeltaReader CreateDeltaReader(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            return this.CreateDeltaReaderImplementation(entitySet, expectedBaseEntityType);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataDeltaReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataDeltaReader"/>.</returns>
        internal override Task<ODataDeltaReader> CreateDeltaReaderAsync(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateDeltaReaderImplementation(entitySet, expectedBaseEntityType));
        }
#endif

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        internal override ODataServiceDocument ReadServiceDocument()
        {
            this.AssertSynchronous();

            ODataJsonLightServiceDocumentDeserializer jsonLightServiceDocumentDeserializer = new ODataJsonLightServiceDocumentDeserializer(this);
            return jsonLightServiceDocumentDeserializer.ReadServiceDocument();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataServiceDocument"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataServiceDocument"/> representing the read service document.</returns>
        internal override Task<ODataServiceDocument> ReadServiceDocumentAsync()
        {
            this.AssertAsynchronous();

            ODataJsonLightServiceDocumentDeserializer jsonLightServiceDocumentDeserializer = new ODataJsonLightServiceDocumentDeserializer(this);
            return jsonLightServiceDocumentDeserializer.ReadServiceDocumentAsync();
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            this.AssertSynchronous();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinks();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            this.AssertAsynchronous();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync();
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            this.AssertSynchronous();
            this.VerifyCanReadEntityReferenceLink();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLink();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync()
        {
            this.AssertAsynchronous();
            this.VerifyCanReadEntityReferenceLink();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync();
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.textReader != null)
                    {
                        this.textReader.Dispose();
                    }
                }
                finally
                {
                    this.textReader = null;
                    this.jsonReader = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Helper method to create a TextReader over the message stream. This is needed by the constructor to dispose the message stream if the creation fails
        /// since this is called from the constructor in place where exception handling is not possible.
        /// </summary>
        /// <param name="messageStream">The stream to read data from.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <returns>The newly created text reader.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        private static TextReader CreateTextReaderForMessageStreamConstructor(Stream messageStream, Encoding encoding)
        {
            Debug.Assert(messageStream != null, "stream != null");

            try
            {
                return new StreamReader(messageStream, encoding);
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

        private static IJsonReader CreateJsonReader(IServiceProvider container, TextReader textReader, bool isIeee754Compatible)
        {
            if (container == null)
            {
                return new JsonReader(textReader, isIeee754Compatible);
            }

            var jsonReaderFactory = container.GetRequiredService<IJsonReaderFactory>();
            var jsonReader = jsonReaderFactory.CreateJsonReader(textReader, isIeee754Compatible);
            Debug.Assert(jsonReader != null, "jsonWriter != null");

            return jsonReader;
        }

        /// <summary>
        /// Verifies that CreateParameterReader can be called.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        private void VerifyCanCreateParameterReader(IEdmOperation operation)
        {
            this.VerifyUserModel();

            if (operation == null)
            {
                throw new ArgumentNullException("operation", ODataErrorStrings.ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader("operation"));
            }
        }

        /// <summary>
        /// Verifies that CreateResourceReader or CreateResourceSetReader or CreateDeltaReader can be called.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="entityType">The expected entity type for the resource/entries to be read.</param>
        private void VerifyCanCreateODataReader(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            Debug.Assert(navigationSource == null || entityType != null, "If an navigation source is specified, the entity type must be specified as well.");

            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
                this.VerifyUserModel();

                if (navigationSource == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_NoEntitySetForRequest);
                }
            }

            // We only check that the base type of the entity set is assignable from the specified entity type.
            // If no entity set/entity type is specified in the API, we will read it from the context URI.
            IEdmEntityType entitySetElementType = this.EdmTypeResolver.GetElementType(navigationSource);
            if (navigationSource != null && entityType != null && !entityType.IsOrInheritsFrom(entitySetElementType))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType(entityType.FullName(), entitySetElementType.FullName(), navigationSource.FullNavigationSourceName()));
            }
        }

        /// <summary>
        /// Verifies that CreateCollectionReader can be called.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        private void VerifyCanCreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
                this.VerifyUserModel();

                if (expectedItemTypeReference == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests);
                }
            }
        }

        /// <summary>
        /// Verifies that ReadEntityReferenceLink can be called.
        /// </summary>
        private void VerifyCanReadEntityReferenceLink()
        {
            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
                this.VerifyUserModel();
            }
        }

        /// <summary>
        /// Verifies that ReadProperty can be called.
        /// </summary>
        private void VerifyCanReadProperty()
        {
            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
                this.VerifyUserModel();
            }
        }

        /// <summary>
        /// Verifies that DetectPayloadKind can be called.
        /// </summary>
        private void VerifyCanDetectPayloadKind()
        {
            if (!this.ReadingResponse)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_PayloadKindDetectionForRequest);
            }
        }

        /// <summary>
        /// Verifies that a user model is available for reading.
        /// </summary>
        private void VerifyUserModel()
        {
            if (!this.Model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_ModelRequiredForReading);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the resource set.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateResourceSetReaderImplementation(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataJsonLightReader(this, entitySet, expectedBaseEntityType, true);
        }

        /// <summary>
        /// Creates an <see cref="ODataDeltaReader" /> to read a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataDeltaReader CreateDeltaReaderImplementation(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataJsonLightDeltaReader(this, entitySet, expectedBaseEntityType);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the resource to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateResourceReaderImplementation(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            return new ODataJsonLightReader(this, navigationSource, expectedEntityType, false);
        }

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        private ODataCollectionReader CreateCollectionReaderImplementation(IEdmTypeReference expectedItemTypeReference)
        {
            return new ODataJsonLightCollectionReader(this, expectedItemTypeReference, null /*listener*/);
        }

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        private ODataParameterReader CreateParameterReaderImplementation(IEdmOperation operation)
        {
            return new ODataJsonLightParameterReader(this, operation);
        }
    }
}
