//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    // ReSharper disable RedundantUsingDirective
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
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

        /// <summary>JSON Light specific state stored during payload kind detection.</summary>
        private readonly ODataJsonLightPayloadKindDetectionState payloadKindDetectionState;

        /// <summary>The text reader created for the input stream.</summary>
        /// <remarks>
        /// The ODataJsonLightInputContext instance owns the textReader instance and thus disposes it. 
        /// We further set this field to null when the input is disposed and use it for checks whether the instance has already been disposed.
        /// </remarks>
        private TextReader textReader;

        /// <summary>The JSON reader to read from.</summary>
        private BufferingJsonReader jsonReader;

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="messageStream">The stream to read data from.</param>
        /// <param name="contentType">The content type of the message to read.</param>
        /// <param name="encoding">The encoding to use to read the input.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="payloadKindDetectionState">JSON Ligth specific state stored during payload kind detection (or null if no payload kind detection happened).</param>
        internal ODataJsonLightInputContext(
            ODataFormat format,
            Stream messageStream,
            MediaType contentType,
            Encoding encoding,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataJsonLightPayloadKindDetectionState payloadKindDetectionState)
            : this(format, CreateTextReaderForMessageStreamConstructor(messageStream, encoding), contentType, messageReaderSettings, version, readingResponse, synchronous, model, urlResolver, payloadKindDetectionState)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="format">The format for this input context.</param>
        /// <param name="reader">The reader to use.</param>
        /// <param name="contentType">The content type of the message to read.</param>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <param name="version">The OData protocol version to be used for reading the payload.</param>
        /// <param name="readingResponse">true if reading a response message; otherwise false.</param>
        /// <param name="synchronous">true if the input should be read synchronously; false if it should be read asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs read from the payload.</param>
        /// <param name="payloadKindDetectionState">JSON Ligth specific state stored during payload kind detection (or null if no payload kind detection happened).</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        internal ODataJsonLightInputContext(
            ODataFormat format,
            TextReader reader,
            MediaType contentType,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool readingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver,
            ODataJsonLightPayloadKindDetectionState payloadKindDetectionState)
            : base(format, messageReaderSettings, version, readingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(contentType != null, "contentType != null");

            try
            {
                ExceptionUtils.CheckArgumentNotNull(format, "format");
                ExceptionUtils.CheckArgumentNotNull(messageReaderSettings, "messageReaderSettings");
            }
            catch (ArgumentNullException)
            {
                // Dispose the message stream if we failed to create the input context.
                reader.Dispose();
                throw;
            }

            try
            {
                this.textReader = reader;

                if (contentType.HasStreamingSetToTrue())
                {
                    this.jsonReader = new BufferingJsonReader(
                        this.textReader,
                        JsonLightConstants.ODataErrorPropertyName,
                        messageReaderSettings.MessageQuotas.MaxNestingDepth,
                        ODataFormat.Json,
                        contentType.HasIeee754CompatibleSetToTrue());
                }
                else
                {
                    // If we have a non-streaming Json Light content type we need to use the re-ordering Json reader
                    this.jsonReader = new ReorderingJsonReader(this.textReader, messageReaderSettings.MessageQuotas.MaxNestingDepth, contentType.HasIeee754CompatibleSetToTrue());
                }
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e) && reader != null)
                {
                    reader.Dispose();
                }

                throw;
            }

            this.payloadKindDetectionState = payloadKindDetectionState;

            // dont know how to get MetadataDocumentUri uri here, messageReaderSettings do not have one
            // Uri metadataDocumentUri = messageReaderSettings..MetadataDocumentUri == null ? null : messageReaderSettings.MetadataDocumentUri.BaseUri;
            // the uri here is used here to create the FullMetadataLevel can pass null in
            this.metadataLevel = JsonLight.JsonLightMetadataLevel.Create(contentType, null, model, readingResponse);
        }

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        internal JsonLightMetadataLevel MetadataLevel
        {
            get
            {
                return this.metadataLevel;
            }
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                Debug.Assert(this.jsonReader != null, "Trying to get JsonReader while none is available.");
                return this.jsonReader;
            }
        }

        /// <summary>
        /// JSON Light specific state stored during payload kind detection.
        /// </summary>
        internal ODataJsonLightPayloadKindDetectionState PayloadKindDetectionState
        {
            get
            {
                return this.payloadKindDetectionState;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateFeedReader(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            return this.CreateFeedReaderImplementation(entitySet, expectedBaseEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal override Task<ODataReader> CreateFeedReaderAsync(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateFeedReaderImplementation(entitySet, expectedBaseEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataDeltaReader" /> to read a feed.
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

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataDeltaReader" /> to read a feed.
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
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateEntryReader(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(navigationSource, expectedEntityType);

            return this.CreateEntryReaderImplementation(navigationSource, expectedEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal override Task<ODataReader> CreateEntryReaderAsync(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(navigationSource, expectedEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateEntryReaderImplementation(navigationSource, expectedEntityType));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateCollectionReader(expectedItemTypeReference);

            return this.CreateCollectionReaderImplementation(expectedItemTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Task which when completed returns the newly create <see cref="ODataCollectionReader"/>.</returns>
        internal override Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateCollectionReader(expectedItemTypeReference);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateCollectionReaderImplementation(expectedItemTypeReference));
        }
#endif
        
        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        internal override ODataParameterReader CreateParameterReader(IEdmOperation operation)
        {
            this.AssertSynchronous();
            this.VerifyCanCreateParameterReader(operation);

            return this.CreateParameterReaderImplementation(operation);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="operation">The operation whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        internal override Task<ODataParameterReader> CreateParameterReaderAsync(IEdmOperation operation)
        {
            this.AssertAsynchronous();
            this.VerifyCanCreateParameterReader(operation);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateParameterReaderImplementation(operation));
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

#if ODATALIB_ASYNC
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
        /// This method creates an reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal override ODataProperty ReadProperty(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
        {
            this.AssertSynchronous();
            this.VerifyCanReadProperty();

            ODataJsonLightPropertyAndValueDeserializer jsonLightPropertyAndValueDeserializer = new ODataJsonLightPropertyAndValueDeserializer(this);
            return jsonLightPropertyAndValueDeserializer.ReadTopLevelProperty(expectedPropertyTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read the property from the input and 
        /// return an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>Task which when completed returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal override Task<ODataProperty> ReadPropertyAsync(IEdmStructuralProperty property, IEdmTypeReference expectedPropertyTypeReference)
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
        internal override ODataError ReadError()
        {
            this.AssertSynchronous();

            ODataJsonLightErrorDeserializer jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(this);
            return jsonLightErrorDeserializer.ReadTopLevelError();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level error.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataError"/> representing the read error.</returns>
        internal override Task<ODataError> ReadErrorAsync()
        {
            this.AssertAsynchronous();

            ODataJsonLightErrorDeserializer jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(this);
            return jsonLightErrorDeserializer.ReadTopLevelErrorAsync();
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            Debug.Assert(this.ReadingResponse, "Should have verified that we are reading a response.");
            this.AssertSynchronous();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinks();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync()
        {
            Debug.Assert(this.ReadingResponse, "Should have verified that we are reading a response.");
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

#if ODATALIB_ASYNC
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
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            this.VerifyCanDetectPayloadKind();

            ODataJsonLightPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKind(detectionInfo);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task which returns an enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            this.VerifyCanDetectPayloadKind();

            ODataJsonLightPayloadKindDetectionDeserializer payloadKindDetectionDeserializer = new ODataJsonLightPayloadKindDetectionDeserializer(this);
            return payloadKindDetectionDeserializer.DetectPayloadKindAsync(detectionInfo);
        }
#endif

        /// <summary>
        /// Disposes the input context.
        /// </summary>
        protected override void DisposeImplementation()
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
        /// Verifies that CreateEntryReader or CreateFeedReader or CreateDeltaReader can be called.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="entityType">The expected entity type for the entry/entries to be read.</param>
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
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateFeedReaderImplementation(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataJsonLightReader(this, entitySet, expectedBaseEntityType, true, null /*listener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataDeltaReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the delta response.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataDeltaReader CreateDeltaReaderImplementation(IEdmEntitySetBase entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataJsonLightDeltaReader(this, entitySet, expectedBaseEntityType);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateEntryReaderImplementation(IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType)
        {
            return new ODataJsonLightReader(this, navigationSource, expectedEntityType, false, null /*listener*/);
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
        /// <param name="operationImport">The operation import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        private ODataParameterReader CreateParameterReaderImplementation(IEdmOperationImport operationImport)
        {
            IEdmOperation operation = operationImport != null ? operationImport.Operation : null;
            return new ODataJsonLightParameterReader(this, operation);
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
