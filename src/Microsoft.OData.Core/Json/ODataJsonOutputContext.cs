//---------------------------------------------------------------------
// <copyright file="ODataJsonOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Json format output context.
    /// </summary>
    internal sealed class ODataJsonOutputContext : ODataOutputContext
    {
        /// <summary>
        /// An in-memory stream for writing stream properties to non-streaming json writer.
        /// </summary>
        internal MemoryStream BinaryValueStream = null;

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        private readonly JsonMetadataLevel metadataLevel;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private Stream asynchronousOutputStream;

        /// <summary>The JSON writer to write to.</summary>
        /// <remarks>This field is also used to determine if the output context has been disposed already.</remarks>
        private IJsonWriter jsonWriter;

        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private JsonTypeNameOracle typeNameOracle;

        /// <summary>
        /// The handler to manage property cache.
        /// </summary>
        private PropertyCacheHandler propertyCacheHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        public ODataJsonOutputContext(
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(ODataFormat.Json, messageInfo, messageWriterSettings)
        {
            Debug.Assert(messageInfo.MessageStream != null, "messageInfo.MessageStream != null");
            Debug.Assert(messageInfo.MediaType != null, "messageInfo.MediaType != null");

            try
            {
                this.messageOutputStream = messageInfo.MessageStream;

                bool isIeee754Compatible = messageInfo.MediaType.HasIeee754CompatibleSetToTrue();

                IJsonWriterFactory jsonWriterFactory = this.Container?.GetService<IJsonWriterFactory>();

                if (jsonWriterFactory is ODataJsonWriterFactory)
                {
                    // When using the legacy ODataJsonWriterFactory, we wrap the stream in a buffered stream
                    // when the async API is used.
                    Stream outputStream = this.InitializeOutputStream();
                    this.jsonWriter = CreateJsonWriter(this.Container, outputStream, isIeee754Compatible, messageWriterSettings, messageInfo.Encoding);
                }
                else
                {
                    // When using ODataUtf8JsonWriterFactory, we do not wrap the ouput stream in a buffered stream.
                    // The default ODataUtf8JsonWriter will be used, and that writer handles
                    // its own buffering (more efficiently). It leads to too many byte[] array allocations if we have
                    // both the BufferedStream and ODataUtf8JsonWriter allocating buffers unnecessarily.
                    this.asynchronousOutputStream = this.messageOutputStream;
                    this.jsonWriter = CreateJsonWriter(
                        this.Container,
                        this.messageOutputStream,
                        isIeee754Compatible,
                        messageWriterSettings,
                        messageInfo.Encoding
                    );
                }
                
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the output context.
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.messageOutputStream.Dispose();
                }

                throw;
            }

            Uri metadataDocumentUri = messageWriterSettings.MetadataDocumentUri;
            bool alwaysAddTypeAnnotationsForDerivedTypes = messageWriterSettings.AlwaysAddTypeAnnotationsForDerivedTypes;
            this.metadataLevel = JsonMetadataLevel.Create(messageInfo.MediaType, metadataDocumentUri, alwaysAddTypeAnnotationsForDerivedTypes, this.Model, this.WritingResponse);
            this.propertyCacheHandler = new PropertyCacheHandler();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataJsonOutputContext(
            Stream stream,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(ODataFormat.Json, messageInfo, messageWriterSettings)
        {
            Debug.Assert(!this.WritingResponse, "Expecting WritingResponse to always be false for this constructor, so no need to validate the MetadataDocumentUri on the writer settings.");
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            bool ieee754CompatibleSetToTrue = (messageInfo.MediaType != null) ? messageInfo.MediaType.HasIeee754CompatibleSetToTrue() : false;

            this.jsonWriter = CreateJsonWriter(messageInfo.ServiceProvider, stream, ieee754CompatibleSetToTrue, messageWriterSettings, messageInfo.Encoding);

            this.metadataLevel = new JsonMinimalMetadataLevel();
            this.propertyCacheHandler = new PropertyCacheHandler();
        }

        /// <summary>
        /// Returns the <see cref="IJsonWriter"/> which is to be used to write the content of the message.
        /// </summary>
        public IJsonWriter JsonWriter
        {
            get
            {
                Debug.Assert(this.jsonWriter != null, "Trying to get JsonWriter while none is available.");
                return this.jsonWriter;
            }
        }

        /// <summary>
        /// Returns the oracle to use when determining the type name to write for entries and values.
        /// </summary>
        public JsonTypeNameOracle TypeNameOracle
        {
            get
            {
                if (this.typeNameOracle == null)
                {
                    this.typeNameOracle = this.MetadataLevel.GetTypeNameOracle();
                }

                return this.typeNameOracle;
            }
        }

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        public JsonMetadataLevel MetadataLevel
        {
            get
            {
                return this.metadataLevel;
            }
        }

        /// <summary>
        /// The handler to manage property cache.
        /// </summary>
        public PropertyCacheHandler PropertyCacheHandler
        {
            get { return propertyCacheHandler; }
        }

        /// <summary>
        /// Returns whether to write control information without the odata prefix.
        /// </summary>
        internal bool OmitODataPrefix
        {
            get
            {
                return this.MessageWriterSettings.GetOmitODataPrefix(this.MessageWriterSettings.Version ?? ODataVersion.V4);
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource set.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataWriter CreateODataResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertSynchronous();

            return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, false);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataWriter> CreateODataResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                entitySetParam,
                resourceTypeParam) => thisParam.CreateODataResourceSetWriterImplementation(
                    entitySetParam,
                    resourceTypeParam,
                    writingParameter: false,
                    writingDelta: false),
                this,
                entitySet,
                resourceType);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a delta resource set.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataWriter CreateODataDeltaResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertSynchronous();

            return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, true);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a delta resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataWriter> CreateODataDeltaResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                entitySetParam,
                resourceTypeParam) => thisParam.CreateODataResourceSetWriterImplementation(
                    entitySetParam,
                    resourceTypeParam,
                    writingParameter: false,
                    writingDelta: true),
                this,
                entitySet,
                resourceType);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="resourceType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataWriter CreateODataResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            this.AssertSynchronous();

            return this.CreateODataResourceWriterImplementation(navigationSource, resourceType);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="resourceType">The structured type for the resources in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataWriter> CreateODataResourceWriterAsync(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                navigationSourceParam,
                resourceTypeParam) => thisParam.CreateODataResourceWriterImplementation(
                    navigationSourceParam,
                    resourceTypeParam),
                this,
                navigationSource,
                resourceType);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            this.AssertSynchronous();

            return this.CreateODataCollectionWriterImplementation(itemTypeReference);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>A running task for the created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(IEdmTypeReference itemTypeReference)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                itemTypeReferenceParam) => thisParam.CreateODataCollectionWriterImplementation(
                    itemTypeReferenceParam),
                this,
                itemTypeReference);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource into a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource for.</param>
        /// <param name="resourceType">The structured type for the resources in the resource set to be written.</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataWriter CreateODataUriParameterResourceWriter(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            return this.CreateODataResourceWriter(navigationSource, resourceType);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write a resource into a Uri operation parameter.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource for.</param>
        /// <param name="resourceType">The structured type for the resources in the resource set to be written.</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataWriter> CreateODataUriParameterResourceWriterAsync(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            return this.CreateODataResourceWriterAsync(navigationSource, resourceType);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource set into a Uri operation parameter.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataWriter CreateODataUriParameterResourceSetWriter(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertSynchronous();

            return this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, true, false);
        }

        /// <summary>
        /// Asynchronously Creates an <see cref="ODataWriter" /> to write a resource set into a Uri operation parameter.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write resources for.</param>
        /// <param name="resourceType">The resource type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataWriter> CreateODataUriParameterResourceSetWriterAsync(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                entitySetParam,
                resourceTypeParam) => thisParam.CreateODataResourceSetWriterImplementation(
                    entitySetParam,
                    resourceTypeParam,
                    writingParameter: true,
                    writingDelta: false),
                this,
                entitySet,
                resourceType);
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override ODataParameterWriter CreateODataParameterWriter(IEdmOperation operation)
        {
            this.AssertSynchronous();

            return this.CreateODataParameterWriterImplementation(operation);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation import whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmOperation operation)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                operationParam) => thisParam.CreateODataParameterWriterImplementation(
                    operationParam),
                this,
                operation);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        public override void WriteProperty(ODataProperty property)
        {
            this.AssertSynchronous();

            this.WritePropertyImplementation(property);
            this.Flush();
        }

        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A task representing the asynchronous operation of writing the property.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        public override async Task WritePropertyAsync(ODataProperty property)
        {
            this.AssertAsynchronous();

            await this.WritePropertyImplementationAsync(property)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        public override void WriteError(ODataError error, bool includeDebugInformation)
        {
            this.AssertSynchronous();

            this.WriteErrorImplementation(error, includeDebugInformation);
            this.Flush();
        }

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
        public override async Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
        {
            this.AssertAsynchronous();

            await this.WriteErrorImplementationAsync(error, includeDebugInformation)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        public void VerifyNotDisposed()
        {
            if (this.messageOutputStream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        public void Flush()
        {
            this.AssertSynchronous();

            // JsonWriter.Flush will call the underlying TextWriter.Flush.
            // The TextWriter.Flush (which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
            // In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
            this.jsonWriter.Flush();
        }

        /// <summary>
        /// Asynchronously flush the writer.
        /// </summary>
        /// <returns>Task which represents the pending flush operation.</returns>
        /// <remarks>The method should not throw directly if the flush operation itself fails, it should instead return a faulted task.</remarks>
        public async Task FlushAsync()
        {
            this.AssertAsynchronous();

            // JsonWriter.FlushAsync will call the underlying TextWriter.FlushAsync.
            // The TextWriter.FlushAsync will call the underlying Stream.FlushAsync.
            // The underlying stream is the async buffered stream, which ignores FlushAsync call.
            await this.jsonWriter.FlushAsync()
                .ConfigureAwait(false);

            Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
            await this.asynchronousOutputStream.FlushAsync()
                .ConfigureAwait(false);
            await this.messageOutputStream.FlushAsync()
                .ConfigureAwait(false);
        }

         /// <summary>
        /// Flushes all buffered data to the underlying stream synchronously.
        /// </summary>
        internal void FlushBuffers()
        {
            if (this.asynchronousOutputStream != null)
            {
                this.asynchronousOutputStream.Flush();
            }
        }

        /// <summary>
        /// Flushes all buffered data to the underlying stream asynchronously.
        /// </summary>
        /// <returns>Task which represents the pending operation.</returns>
        internal Task FlushBuffersAsync()
        {
            if (this.asynchronousOutputStream != null)
            {
                return this.asynchronousOutputStream.FlushAsync();
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// The output stream to write the payload to.
        /// </summary>
        /// <returns>The output stream.</returns>
        internal Stream GetOutputStream()
        {
            return this.Synchronous
                ? this.messageOutputStream
                : this.asynchronousOutputStream;
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses in Json.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataBatchWriter CreateODataBatchWriter()
        {
            this.AssertSynchronous();

            return this.CreateODataBatchWriterImplementation();
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(
                thisParam => thisParam.CreateODataBatchWriterImplementation(),
                this);
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
            this.AssertSynchronous();

            this.WriteInStreamErrorImplementation(error, includeDebugInformation);
            this.Flush();
        }

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
        internal override async Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
        {
            this.AssertAsynchronous();

            await this.WriteInStreamErrorImplementationAsync(error, includeDebugInformation)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Creates an <see cref="ODataDeltaWriter" /> to write a delta response.
        /// </summary>
        /// <returns>The created writer.</returns>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataDeltaWriter CreateODataDeltaWriter(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            this.AssertSynchronous();
            return this.CreateODataDeltaWriterImplementation(entitySet, entityType);
        }

        /// <summary>
        /// Asynchronously creates an <see cref="ODataDeltaWriter" /> to write a delta response.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataDeltaWriter> CreateODataDeltaWriterAsync(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation((
                thisParam,
                entitySetParam,
                entityTypeParam) => thisParam.CreateODataDeltaWriterImplementation(
                    entitySetParam,
                    entityTypeParam),
                this,
                entitySet,
                entityType);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteServiceDocument(ODataServiceDocument serviceDocument)
        {
            this.AssertSynchronous();

            this.WriteServiceDocumentImplementation(serviceDocument);
            this.Flush();
        }

        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override async Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument)
        {
            this.AssertAsynchronous();

            await this.WriteServiceDocumentImplementationAsync(serviceDocument)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            this.AssertSynchronous();

            this.WriteEntityReferenceLinksImplementation(links);
            this.Flush();
        }

        /// <summary>
        /// Asynchronously writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override async Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            this.AssertAsynchronous();

            await this.WriteEntityReferenceLinksImplementationAsync(links)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            this.AssertSynchronous();

            this.WriteEntityReferenceLinkImplementation(link);
            this.Flush();
        }

        /// <summary>
        /// Asynchronously writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override async Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            this.AssertAsynchronous();

            await this.WriteEntityReferenceLinkImplementationAsync(link)
                .ConfigureAwait(false);
            await this.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "textWriter", Justification = "We don't dispose the jsonWriter or textWriter, instead we dispose the underlying stream directly.")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.messageOutputStream != null)
                    {
                        // The IJsonWriter will flush the underlying stream
                        this.jsonWriter.Flush();

                        if (this.jsonWriter is IDisposable disposableWriter)
                        {
                            disposableWriter.Dispose();
                        }

                        // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitly.
                        if (this.asynchronousOutputStream != null)
                        {
                            this.asynchronousOutputStream.Flush();
                            // We are working with a BufferedStream here. We flushed it already, so there is nothing else to dispose. And it would dispose the 
                            // inner stream as well.
                        }

                        // Dispose the message stream (note that we OWN this stream, so we always dispose it).
                        this.messageOutputStream.Dispose();
                    }

                    if (this.BinaryValueStream != null)
                    {
                        this.BinaryValueStream.Flush();
                        this.BinaryValueStream.Dispose();
                    }
                }
                finally
                {
                    this.messageOutputStream = null;
                    this.asynchronousOutputStream = null;
                    this.BinaryValueStream = null;
                    this.jsonWriter = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            try
            {
                if (this.messageOutputStream != null)
                {

                    // The IJsonWriter will flush the underlying stream
                    await this.jsonWriter.FlushAsync().ConfigureAwait(false);

                    if (this.jsonWriter is IAsyncDisposable asyncDisposableWriter)
                    {
                        await asyncDisposableWriter.DisposeAsync().ConfigureAwait(false);
                    }

                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitly.
                    if (this.asynchronousOutputStream != null)
                    {
                        await this.asynchronousOutputStream.FlushAsync().ConfigureAwait(false);
                        // We are working with a BufferedStream here. We flushed it already, so there is nothing else to dispose. And it would dispose the 
                        // inner stream as well.
                    }

                    // Dispose the message stream (note that we OWN this stream, so we always dispose it).
                    await this.messageOutputStream.DisposeAsync().ConfigureAwait(false);
                }

                if (this.BinaryValueStream != null)
                {
                    await this.BinaryValueStream.FlushAsync().ConfigureAwait(false);
                    await this.BinaryValueStream.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.BinaryValueStream = null;
                this.jsonWriter = null;
            }
        }

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="container">The dependency injection container to get related services.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="isIeee754Compatible">true if the writer should write large integers as strings.</param>
        /// <param name="writerSettings">Configuration settings for the OData writer.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>An <see cref="IJsonWriter"/> instance.</returns>
        private static IJsonWriter CreateJsonWriter(
            IServiceProvider container,
            Stream stream,
            bool isIeee754Compatible,
            ODataMessageWriterSettings writerSettings,
            Encoding encoding)
        {
            IJsonWriter jsonWriter;

            if (container == null)
            {
                jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding, leaveStreamOpen: true);
            }
            else
            {
                IJsonWriterFactory jsonWriterFactory = container.GetRequiredService<IJsonWriterFactory>();
                jsonWriter = jsonWriterFactory.CreateJsonWriter(stream, isIeee754Compatible, encoding);

                if (jsonWriter == null)
                {
                    throw new ODataException(Strings.ODataMessageWriter_JsonWriterFactory_ReturnedNull(isIeee754Compatible, encoding.WebName));
                }
            }

            if (jsonWriter is JsonWriter defaultJsonWriter)
            {
                defaultJsonWriter.ArrayPool = writerSettings.ArrayPool;
            }

            return jsonWriter;
        }

        /// <summary>
        /// Initializes the output stream.
        /// </summary>
        /// <param name="stream">The supplied message stream.</param>
        /// <returns>The output stream.</returns>
        private Stream InitializeOutputStream()
        {
            if (this.Synchronous)
            {
                return this.messageOutputStream;
            }

            this.asynchronousOutputStream = new BufferedStream(this.messageOutputStream, this.MessageWriterSettings.BufferSize);

            return this.asynchronousOutputStream;
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource set.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingParameter">true means writing a resource set into a uri operation parameter, false writing a resource set in other payloads.</param>
        /// <param name="writingDelta">true means writing a delta resource set.</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataResourceSetWriterImplementation(IEdmEntitySetBase entitySet, IEdmStructuredType resourceType, bool writingParameter, bool writingDelta)
        {
            ODataJsonWriter odataJsonWriter = new ODataJsonWriter(this, entitySet, resourceType, /*writingResourceSet*/true, writingParameter, writingDelta);
            this.outputInStreamErrorListener = odataJsonWriter;
            return odataJsonWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataDeltaWriter" /> to write a delta response.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataDeltaWriter CreateODataDeltaWriterImplementation(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            ODataJsonDeltaWriter odataJsonDeltaWriter = new ODataJsonDeltaWriter(this, entitySet, entityType);
            this.outputInStreamErrorListener = odataJsonDeltaWriter;
            return odataJsonDeltaWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a resource.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write resource set for.</param>
        /// <param name="resourceType">The structured type for the items in the resource set to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataResourceWriterImplementation(IEdmNavigationSource navigationSource, IEdmStructuredType resourceType)
        {
            ODataJsonWriter odataJsonWriter = new ODataJsonWriter(this, navigationSource, resourceType, /*writingResourceSet*/false);
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
            ODataJsonCollectionWriter jsonCollectionWriter = new ODataJsonCollectionWriter(this, itemTypeReference);
            this.outputInStreamErrorListener = jsonCollectionWriter;
            return jsonCollectionWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        private ODataParameterWriter CreateODataParameterWriterImplementation(IEdmOperation operation)
        {
            ODataJsonParameterWriter jsonParameterWriter = new ODataJsonParameterWriter(this, operation);
            this.outputInStreamErrorListener = jsonParameterWriter;
            return jsonParameterWriter;
        }

        /// <summary>
        /// Creates a concrete <see cref="ODataJsonBatchWriter" /> instance.
        /// </summary>
        /// <returns>The newly created batch writer.</returns>
        private ODataBatchWriter CreateODataBatchWriterImplementation()
        {
            ODataBatchWriter batchWriter = new ODataJsonBatchWriter(this);
            this.outputInStreamErrorListener = batchWriter;
            return batchWriter;
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

            JsonInstanceAnnotationWriter instanceAnnotationWriter = new JsonInstanceAnnotationWriter(new ODataJsonValueSerializer(this), this.TypeNameOracle);
            ODataJsonWriterUtils.WriteError(
                this.JsonWriter,
                instanceAnnotationWriter.WriteInstanceAnnotationsForError,
                error, includeDebugInformation,
                this.MessageWriterSettings);
        }

        /// <summary>
        /// Asynchronously writes an in-stream error.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteInStreamErrorImplementationAsync(ODataError error, bool includeDebugInformation)
        {
            if (this.outputInStreamErrorListener != null)
            {
                await this.outputInStreamErrorListener.OnInStreamErrorAsync()
                    .ConfigureAwait(false);
            }

            JsonInstanceAnnotationWriter instanceAnnotationWriter = new JsonInstanceAnnotationWriter(new ODataJsonValueSerializer(this), this.TypeNameOracle);
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.jsonWriter,
                instanceAnnotationWriter.WriteInstanceAnnotationsForErrorAsync,
                error,
                includeDebugInformation,
                this.MessageWriterSettings).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataJsonPropertySerializer jsonPropertySerializer = new ODataJsonPropertySerializer(this, /*initContextUriBuilder*/ true);
            jsonPropertySerializer.WriteTopLevelProperty(property);
        }

        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WritePropertyImplementationAsync(ODataProperty property)
        {
            ODataJsonPropertySerializer jsonPropertySerializer = new ODataJsonPropertySerializer(this, /*initContextUriBuilder*/ true);
            await jsonPropertySerializer.WriteTopLevelPropertyAsync(property)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        private void WriteServiceDocumentImplementation(ODataServiceDocument serviceDocument)
        {
            ODataJsonServiceDocumentSerializer jsonServiceDocumentSerializer = new ODataJsonServiceDocumentSerializer(this);
            jsonServiceDocumentSerializer.WriteServiceDocument(serviceDocument);
        }

        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteServiceDocumentImplementationAsync(ODataServiceDocument serviceDocument)
        {
            ODataJsonServiceDocumentSerializer jsonServiceDocumentSerializer = new ODataJsonServiceDocumentSerializer(this);
            await jsonServiceDocumentSerializer.WriteServiceDocumentAsync(serviceDocument)
                .ConfigureAwait(false);
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
            ODataJsonSerializer jsonSerializer = new ODataJsonSerializer(this, false);
            jsonSerializer.WriteTopLevelError(error, includeDebugInformation);
        }

        /// <summary>
        /// Asynchronously writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteErrorImplementationAsync(ODataError error, bool includeDebugInformation)
        {
            ODataJsonSerializer jsonSerializer = new ODataJsonSerializer(this, false);
            await jsonSerializer.WriteTopLevelErrorAsync(error, includeDebugInformation)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            jsonEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links);
        }

        /// <summary>
        /// Asynchronously writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteEntityReferenceLinksImplementationAsync(ODataEntityReferenceLinks links)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            await jsonEntityReferenceLinkSerializer.WriteEntityReferenceLinksAsync(links)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            jsonEntityReferenceLinkSerializer.WriteEntityReferenceLink(link);
        }

        /// <summary>
        /// Asynchronously writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteEntityReferenceLinkImplementationAsync(ODataEntityReferenceLink link)
        {
            ODataJsonEntityReferenceLinkSerializer jsonEntityReferenceLinkSerializer = new ODataJsonEntityReferenceLinkSerializer(this);
            await jsonEntityReferenceLinkSerializer.WriteEntityReferenceLinkAsync(link)
                .ConfigureAwait(false);
        }
    }
}
