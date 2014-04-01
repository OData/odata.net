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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// JsonLight format output context.
    /// </summary>
    internal sealed class ODataJsonLightOutputContext : ODataJsonOutputContextBase
    {
        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        private readonly JsonLightMetadataLevel metadataLevel;

        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private JsonLightTypeNameOracle typeNameOracle;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="model">The model to use.</param>
        internal ODataJsonLightOutputContext(
            ODataFormat format,
            TextWriter textWriter,
            ODataMessageWriterSettings messageWriterSettings,
            IEdmModel model)
            : base(format, textWriter, messageWriterSettings, model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!this.WritingResponse, "Expecting WritingResponse to always be false for this constructor, so no need to validate the MetadataDocumentUri on the writer settings.");
            Debug.Assert(textWriter != null, "textWriter != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            this.metadataLevel = new JsonMinimalMetadataLevel();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageStream">The message stream to write the payload to.</param>
        /// <param name="mediaType">The specific media type being written.</param>
        /// <param name="encoding">The encoding to use for the payload.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="synchronous">true if the output should be written synchronously; false if it should be written asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        internal ODataJsonLightOutputContext(ODataFormat format, Stream messageStream, MediaType mediaType, Encoding encoding, ODataMessageWriterSettings messageWriterSettings, bool writingResponse, bool synchronous, IEdmModel model, IODataUrlResolver urlResolver)
            : base(format, messageStream, encoding, messageWriterSettings, writingResponse, synchronous, model, urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageStream != null, "messageStream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");
            Debug.Assert(mediaType != null, "mediaType != null");
            Uri metadataDocumentUri = messageWriterSettings.MetadataDocumentUri == null ? null : messageWriterSettings.MetadataDocumentUri.BaseUri;
            this.metadataLevel = JsonLightMetadataLevel.Create(mediaType, metadataDocumentUri, model, writingResponse);
        }

        /// <summary>
        /// Returns the oracle to use when determining the type name to write for entries and values.
        /// </summary>
        internal JsonLightTypeNameOracle TypeNameOracle
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                if (this.typeNameOracle == null)
                {
                    this.typeNameOracle = this.MetadataLevel.GetTypeNameOracle(this.MessageWriterSettings.AutoComputePayloadMetadataInJson);
                }

                return this.typeNameOracle;
            }
        }

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        internal JsonLightMetadataLevel MetadataLevel
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataLevel;
            }
        }

        /// <summary>
        /// Creates a metadata uri builder for the current output context.
        /// </summary>
        /// <returns>The metadata uri builder to use when writing.</returns>
        internal ODataJsonLightContextUriBuilder CreateMetadataUriBuilder()
        {
            DebugUtils.CheckNoExternalCallers();
            return ODataJsonLightContextUriBuilder.CreateFromSettings(this.MetadataLevel, this.WritingResponse, this.MessageWriterSettings, this.Model);
        }

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
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataFeedWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataFeedWriterImplementation(entitySet, entityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataWriter> CreateODataFeedWriterAsync(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataFeedWriterImplementation(entitySet, entityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataEntryWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataEntryWriterImplementation(entitySet, entityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataWriter> CreateODataEntryWriterAsync(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataEntryWriterImplementation(entitySet, entityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataCollectionWriterImplementation(itemTypeReference);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>A running task for the created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(IEdmTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataCollectionWriterImplementation(itemTypeReference));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operationImport">The operation import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataParameterWriter CreateODataParameterWriter(IEdmOperationImport operationImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            return this.CreateODataParameterWriterImplementation(operationImport);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operationImport">The operation import whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmOperationImport operationImport)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataParameterWriterImplementation(operationImport));
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
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLinks(ODataEntityReferenceLinks links, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteEntityReferenceLinksImplementation(links, entitySet, navigationProperty);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinksImplementation(links, entitySet, navigationProperty);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference link is being written, or null if none is available.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLink(ODataEntityReferenceLink link, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertSynchronous();

            this.WriteEntityReferenceLinkImplementation(link, entitySet, navigationProperty);
            this.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference link is being written, or null if none is available.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinkImplementation(link, entitySet, navigationProperty);
                    return this.FlushAsync();
                });
        }
#endif

        //// Raw value is not supported by JsonLight.

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataFeedWriterImplementation(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            ODataJsonLightWriter odataJsonWriter = new ODataJsonLightWriter(this, entitySet, entityType, /*writingFeed*/true);
            this.outputInStreamErrorListener = odataJsonWriter;
            return odataJsonWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataEntryWriterImplementation(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            ODataJsonLightWriter odataJsonWriter = new ODataJsonLightWriter(this, entitySet, entityType, /*writingFeed*/false);
            this.outputInStreamErrorListener = odataJsonWriter;
            return odataJsonWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        private ODataCollectionWriter CreateODataCollectionWriterImplementation(IEdmTypeReference itemTypeReference)
        {
            ODataJsonLightCollectionWriter jsonLightCollectionWriter = new ODataJsonLightCollectionWriter(this, itemTypeReference);
            this.outputInStreamErrorListener = jsonLightCollectionWriter;
            return jsonLightCollectionWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operationImport">The operation import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        private ODataParameterWriter CreateODataParameterWriterImplementation(IEdmOperationImport operationImport)
        {
            ODataJsonLightParameterWriter jsonLightParameterWriter = new ODataJsonLightParameterWriter(this, operationImport);
            this.outputInStreamErrorListener = jsonLightParameterWriter;
            return jsonLightParameterWriter;
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

            JsonLightInstanceAnnotationWriter instanceAnnotationWriter = new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(this), this.TypeNameOracle);
            ODataJsonWriterUtils.WriteError(this.JsonWriter, instanceAnnotationWriter.WriteInstanceAnnotations, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, /*writingJsonLight*/ true);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataJsonLightPropertySerializer jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this);
            jsonLightPropertySerializer.WriteTopLevelProperty(property);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        /// <param name="defaultWorkspace">The default workspace to write in the service document.</param>
        private void WriteServiceDocumentImplementation(ODataWorkspace defaultWorkspace)
        {
            ODataJsonLightServiceDocumentSerializer jsonLightServiceDocumentSerializer = new ODataJsonLightServiceDocumentSerializer(this);
            jsonLightServiceDocumentSerializer.WriteServiceDocument(defaultWorkspace);
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
            ODataJsonLightSerializer jsonLightSerializer = new ODataJsonLightSerializer(this);
            jsonLightSerializer.WriteTopLevelError(error, includeDebugInformation);
        }

        /// <summary>
        /// Writes the result of a $links query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference links are being written, or null if none is available.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links, entitySet, navigationProperty);
        }

        /// <summary>
        /// Writes a singleton result of a $links query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <param name="entitySet">The entity set of the navigation property.</param>
        /// <param name="navigationProperty">The navigation property for which the entity reference link is being written, or null if none is available.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLink(link, entitySet, navigationProperty);
        }
    }
}
