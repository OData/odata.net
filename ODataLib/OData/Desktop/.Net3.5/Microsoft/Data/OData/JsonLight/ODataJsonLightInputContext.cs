//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.JsonLight
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
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
// ReSharper disable RedundantUsingDirective
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
// ReSharper restore RedundantUsingDirective
    #endregion Namespaces

    /// <summary>
    /// Implementation of the OData input for JsonLight OData format.
    /// </summary>
    internal sealed class ODataJsonLightInputContext : ODataInputContext
    {
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
                        ODataAnnotationNames.ODataError,
                        messageReaderSettings.MessageQuotas.MaxNestingDepth,
                        ODataFormat.Json);
                }
                else
                {
                    // If we have a non-streaming Json Light content type we need to use the re-ordering Json reader
                    this.jsonReader = new ReorderingJsonReader(this.textReader, messageReaderSettings.MessageQuotas.MaxNestingDepth);
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
        }

        /// <summary>
        /// Returns the <see cref="BufferingJsonReader"/> which is to be used to read the content of the message.
        /// </summary>
        internal BufferingJsonReader JsonReader
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.payloadKindDetectionState;
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entries in the feed.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateFeedReader(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
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
        internal override Task<ODataReader> CreateFeedReaderAsync(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedBaseEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateFeedReaderImplementation(entitySet, expectedBaseEntityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        internal override ODataReader CreateEntryReader(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedEntityType);

            return this.CreateEntryReaderImplementation(entitySet, expectedEntityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataReader"/>.</returns>
        internal override Task<ODataReader> CreateEntryReaderAsync(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            this.VerifyCanCreateODataReader(entitySet, expectedEntityType);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateEntryReaderImplementation(entitySet, expectedEntityType));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataCollectionReader"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <returns>Newly create <see cref="ODataCollectionReader"/>.</returns>
        internal override ODataCollectionReader CreateCollectionReader(IEdmTypeReference expectedItemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            this.VerifyCanCreateCollectionReader(expectedItemTypeReference);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateCollectionReaderImplementation(expectedItemTypeReference));
        }
#endif

        /// <summary>
        /// Create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        internal override ODataParameterReader CreateParameterReader(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();
            this.VerifyCanCreateParameterReader(functionImport);

            return this.CreateParameterReaderImplementation(functionImport);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously create a <see cref="ODataParameterReader"/>.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>Task which when completed returns the newly created <see cref="ODataParameterReader"/>.</returns>
        internal override Task<ODataParameterReader> CreateParameterReaderAsync(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            this.VerifyCanCreateParameterReader(functionImport);

            // Note that the reading is actually synchronous since we buffer the entire input when getting the stream from the message.
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateParameterReaderImplementation(functionImport));
        }
#endif

        /// <summary>
        /// Read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>An <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal override ODataWorkspace ReadServiceDocument()
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            ODataJsonLightServiceDocumentDeserializer jsonLightServiceDocumentDeserializer = new ODataJsonLightServiceDocumentDeserializer(this);
            return jsonLightServiceDocumentDeserializer.ReadServiceDocument();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a service document. 
        /// This method reads the service document from the input and returns 
        /// an <see cref="ODataWorkspace"/> that represents the read service document.
        /// </summary>
        /// <returns>Task which when completed returns an <see cref="ODataWorkspace"/> representing the read service document.</returns>
        internal override Task<ODataWorkspace> ReadServiceDocumentAsync()
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            ODataJsonLightErrorDeserializer jsonLightErrorDeserializer = new ODataJsonLightErrorDeserializer(this);
            return jsonLightErrorDeserializer.ReadTopLevelErrorAsync();
        }
#endif

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override ODataEntityReferenceLinks ReadEntityReferenceLinks(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.ReadingResponse, "Should have verified that we are reading a response.");
            this.AssertSynchronous();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinks(navigationProperty);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a set of top-level entity reference links.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links.</param>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal override Task<ODataEntityReferenceLinks> ReadEntityReferenceLinksAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.ReadingResponse, "Should have verified that we are reading a response.");
            this.AssertAsynchronous();

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync(navigationProperty);
        }
#endif

        /// <summary>
        /// Reads a top-level entity reference link.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override ODataEntityReferenceLink ReadEntityReferenceLink(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();
            this.VerifyCanReadEntityReferenceLink(navigationProperty);

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLink(navigationProperty);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously read a top-level entity reference link.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <returns>Task which when completed returns an <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal override Task<ODataEntityReferenceLink> ReadEntityReferenceLinkAsync(IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();
            this.VerifyCanReadEntityReferenceLink(navigationProperty);

            ODataJsonLightEntityReferenceLinkDeserializer jsonLightEntityReferenceLinkDeserializer = new ODataJsonLightEntityReferenceLinkDeserializer(this);
            return jsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync(navigationProperty);
        }
#endif

        /// <summary>
        /// Detects the payload kind(s) from the message stream.
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        private void VerifyCanCreateParameterReader(IEdmFunctionImport functionImport)
        {
            this.VerifyUserModel();

            if (functionImport == null)
            {
                throw new ArgumentNullException("functionImport", ODataErrorStrings.ODataJsonLightInputContext_FunctionImportCannotBeNullForCreateParameterReader("functionImport"));
            }
        }

        /// <summary>
        /// Verifies that CreateEntryReader or CreateFeedReader can be called.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="entityType">The expected entity type for the entry/entries to be read.</param>
        private void VerifyCanCreateODataReader(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            Debug.Assert(entitySet == null || entityType != null, "If an entity set is specified, the entity type must be specified as well.");

            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
                this.VerifyUserModel();

                if (entitySet == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_NoEntitySetForRequest);
                }
            }

            // We only check that the base type of the entity set is assignable from the specified entity type.
            // If no entity set/entity type is specified in the API, we will read it from the metadata URI.
            IEdmEntityType entitySetElementType = this.EdmTypeResolver.GetElementType(entitySet);
            if (entitySet != null && entityType != null && !entityType.IsOrInheritsFrom(entitySetElementType))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_EntityTypeMustBeCompatibleWithEntitySetBaseType(entityType.FullName(), entitySetElementType.FullName(), entitySet.FullName()));
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
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        private void VerifyCanReadEntityReferenceLink(IEdmNavigationProperty navigationProperty)
        {
            // We require metadata information for reading requests.
            if (!this.ReadingResponse)
            {
            this.VerifyUserModel();

                if (navigationProperty == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightInputContext_NavigationPropertyRequiredForReadEntityReferenceLinkInRequests);
            }
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
            DebugUtils.CheckNoExternalCallers();
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
        private ODataReader CreateFeedReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedBaseEntityType)
        {
            return new ODataJsonLightReader(this, entitySet, expectedBaseEntityType, true, null /*listener*/);
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the entry to be read.</param>
        /// <returns>The newly created <see cref="ODataReader"/>.</returns>
        private ODataReader CreateEntryReaderImplementation(IEdmEntitySet entitySet, IEdmEntityType expectedEntityType)
        {
            return new ODataJsonLightReader(this, entitySet, expectedEntityType, false, null /*listener*/);
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
        /// <param name="functionImport">The function import whose parameters are being read.</param>
        /// <returns>The newly created <see cref="ODataParameterReader"/>.</returns>
        private ODataParameterReader CreateParameterReaderImplementation(IEdmFunctionImport functionImport)
        {
            return new ODataJsonLightParameterReader(this, functionImport);
        }
    }
}
