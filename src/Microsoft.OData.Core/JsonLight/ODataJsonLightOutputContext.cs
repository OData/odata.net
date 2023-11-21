//---------------------------------------------------------------------
// <copyright file="ODataJsonLightOutputContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// JsonLight format output context.
    /// </summary>
    internal sealed class ODataJsonLightOutputContext : ODataOutputContext
    {
        /// <summary>
        /// An in-memory stream for writing stream properties to non-streaming json writer.
        /// </summary>
        internal MemoryStream BinaryValueStream = null;

        /// <summary>
        /// An in-memory StringWriter for writing string properties to non-streaming json writer.
        /// </summary>
        internal StringWriter StringWriter = null;

        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        private readonly JsonLightMetadataLevel metadataLevel;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

#if NETSTANDARD1_1
        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;
#else
        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private Stream asynchronousOutputStream;
#endif

        /// <summary>The text writer created for the output stream.</summary>
        private TextWriter textWriter;

        /// <summary>The JSON writer to write to.</summary>
        /// <remarks>This field is also used to determine if the output context has been disposed already.</remarks>
        private IJsonWriter jsonWriter;

        /// <summary>The asynchronous JSON writer to write to.</summary>
        private IJsonWriterAsync asynchronousJsonWriter;

        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private JsonLightTypeNameOracle typeNameOracle;

        /// <summary>
        /// The handler to manage property cache.
        /// </summary>
        private PropertyCacheHandler propertyCacheHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        public ODataJsonLightOutputContext(
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

                // We first attempt to create a JSON writer which directly writes to the stream
                IStreamBasedJsonWriterFactory streamBasedJsonWriterFactory = this.Container?.GetService<IStreamBasedJsonWriterFactory>();

                if (streamBasedJsonWriterFactory != null)
                {
#if !NETSTANDARD1_1
                    // When using IStreamBasedJsonWriterFactory, we do not wrap the ouput stream in a buffered stream.
                    // The assumption is that the default ODataUtf8JsonWriter will be used, and that writer handles
                    // its own buffering (more efficiently). It leads to too many byte[] array allocations if we have
                    // both the BufferedStream and ODataUtf8JsonWriter allocating buffers unnecessarily.
                    this.asynchronousOutputStream = this.messageOutputStream;
#endif

                    this.asynchronousJsonWriter = CreateAsynchronousJsonWriter(
                        streamBasedJsonWriterFactory,
                        this.messageOutputStream,
                        isIeee754Compatible,
                        messageInfo.Encoding);

                    // the IStreamBasedJsonWriterFactory expects that the async writer also implements
                    // the synchronous interface. The EnsureJsonWritersReferenceTheSameInstance() method verifies this.
                }

                // Then fallback to the TextWriter-based approach
                if (this.asynchronousJsonWriter == null && this.jsonWriter == null)
                {
                    Stream outputStream;
                    if (this.Synchronous)
                    {
                        outputStream = this.messageOutputStream;
                    }
                    else
                    {
#if NETSTANDARD1_1
                        this.asynchronousOutputStream = new AsyncBufferedStream(this.messageOutputStream);
#else
                        this.asynchronousOutputStream = new BufferedStream(this.messageOutputStream, this.MessageWriterSettings.BufferSize);
#endif
                        outputStream = this.asynchronousOutputStream;
                    }

                    this.textWriter = new StreamWriter(outputStream, messageInfo.Encoding);
                    this.jsonWriter = CreateJsonWriter(this.Container, this.textWriter, isIeee754Compatible, messageWriterSettings);

                    if (!(this.jsonWriter is IJsonWriterAsync))
                    {
                        this.asynchronousJsonWriter = CreateAsynchronousJsonWriter(
                        this.Container,
                        this.textWriter,
                        isIeee754Compatible,
                        messageWriterSettings);
                    }
                }

                this.EnsureJsonWritersReferenceTheSameInstance();
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
            this.metadataLevel = JsonLightMetadataLevel.Create(messageInfo.MediaType, metadataDocumentUri, alwaysAddTypeAnnotationsForDerivedTypes, this.Model, this.WritingResponse);
            this.propertyCacheHandler = new PropertyCacheHandler();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <param name="messageInfo">The context information for the message.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        internal ODataJsonLightOutputContext(
            TextWriter textWriter,
            ODataMessageInfo messageInfo,
            ODataMessageWriterSettings messageWriterSettings)
            : base(ODataFormat.Json, messageInfo, messageWriterSettings)
        {
            Debug.Assert(!this.WritingResponse, "Expecting WritingResponse to always be false for this constructor, so no need to validate the MetadataDocumentUri on the writer settings.");
            Debug.Assert(textWriter != null, "textWriter != null");
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            this.textWriter = textWriter;
            bool ieee754CompatibleSetToTrue = (messageInfo.MediaType != null) ? messageInfo.MediaType.HasIeee754CompatibleSetToTrue() : false;
            this.asynchronousJsonWriter = CreateAsynchronousJsonWriter(messageInfo.Container, textWriter, ieee754CompatibleSetToTrue, messageWriterSettings);
            
            if (this.asynchronousJsonWriter == null)
            {
                this.jsonWriter = CreateJsonWriter(messageInfo.Container, textWriter, ieee754CompatibleSetToTrue, messageWriterSettings);
            }
            
            this.EnsureJsonWritersReferenceTheSameInstance();
            this.metadataLevel = new JsonMinimalMetadataLevel();
            this.propertyCacheHandler = new PropertyCacheHandler();
        }

        /// <summary>
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message.
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
        /// Returns the <see cref="JsonWriter"/> which is to be used to write the content of the message asynchronously.
        /// </summary>
        public IJsonWriterAsync AsynchronousJsonWriter
        {
            get
            {
                Debug.Assert(this.asynchronousJsonWriter != null, "Trying to get asynchronous JsonWriter while none is available.");
                return this.asynchronousJsonWriter;
            }
        }

        /// <summary>
        /// Returns the oracle to use when determining the type name to write for entries and values.
        /// </summary>
        public JsonLightTypeNameOracle TypeNameOracle
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
        public JsonLightMetadataLevel MetadataLevel
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
                return this.ODataSimplifiedOptions.GetOmitODataPrefix(this.MessageWriterSettings.Version ?? ODataVersion.V4);
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
            await this.asynchronousJsonWriter.FlushAsync()
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
#if NETSTANDARD1_1
                this.asynchronousOutputStream.FlushSync();
#else
                this.asynchronousOutputStream.Flush();
#endif
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
#if NETSTANDARD1_1
                return this.asynchronousOutputStream.FlushAsync();
#else
                return this.asynchronousOutputStream.FlushAsync();
#endif
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
#if NETSTANDARD1_1
                            this.asynchronousOutputStream.FlushSync();
                            this.asynchronousOutputStream.Dispose();
#else
                            this.asynchronousOutputStream.Flush();
                            // We are working with a BufferedStream here. We flushed it already, so there is nothing else to dispose. And it would dispose the 
                            // inner stream as well.
#endif
                        }

                        // Dispose the message stream (note that we OWN this stream, so we always dispose it).
                        this.messageOutputStream.Dispose();
                    }

                    if (this.BinaryValueStream != null)
                    {
                        this.BinaryValueStream.Flush();
                        this.BinaryValueStream.Dispose();
                    }

                    if (this.StringWriter != null)
                    {
                        this.StringWriter.Flush();
                        this.StringWriter.Dispose();
                    }
                }
                finally
                {
                    this.messageOutputStream = null;
                    this.asynchronousOutputStream = null;
                    this.BinaryValueStream = null;
                    this.textWriter = null;
                    this.jsonWriter = null;
                    this.StringWriter = null;
                }
            }

            base.Dispose(disposing);
        }

#if NETCOREAPP3_1_OR_GREATER
        protected override async ValueTask DisposeAsyncCore()
        {
            try
            {
                if (this.messageOutputStream != null)
                {

                    // The IJsonWriterAsync will flush the underlying stream
                    await this.asynchronousJsonWriter.FlushAsync().ConfigureAwait(false);

                    if (this.asynchronousJsonWriter is IAsyncDisposable asyncDisposableWriter)
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

                if (this.StringWriter != null)
                {
                    await this.StringWriter.FlushAsync().ConfigureAwait(false);
                    await this.StringWriter.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.BinaryValueStream = null;
                this.textWriter = null;
                this.jsonWriter = null;
                this.StringWriter = null;
            }
        }
#endif

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/>.
        /// </summary>
        /// <param name="container">The dependency injection container to get related services.</param>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <param name="isIeee754Compatible">true if the writer should write large integers as strings.</param>
        /// <param name="writerSettings">Configuration settings for the OData writer.</param>
        /// <returns>A JSON writer.</returns>
        /// <remarks>Asynchronous support is not implemented in Microsoft.Spatial library.
        /// To write spatial data, we rely on the synchronous PrimitiveConverter.Instance.WriteJsonLight(object, IJsonWriter) method.
        /// When writing asynchronously we wrap this method in a Task. WriteJsonLight method takes an
        /// IJsonWriter parameter while the asynchronous writer is declared as IJsonWriterAsync.
        /// However, JsonWriter class implements both IJsonWriter and IJsonWriterAsync.
        /// To guarantee that when the synchronous writer is called to write spatial data it shares
        /// the same scope(s) as the asynchronous writer, we initialize them to the same concrete JsonWriter instance.
        /// We only do this when the dependency injection container is uninitialized or
        /// when the JSON writer factory is DefaultJsonWriterFactory - since both CreateJsonWriter and
        /// CreateAsynchronousJsonWriter methods of that factory return an instance of JsonWriter.
        /// Merging IJsonWriter and IJsonWriterAsync interface in a major release will simplify this.</remarks>
        private static IJsonWriter CreateJsonWriter(
            IServiceProvider container,
            TextWriter textWriter,
            bool isIeee754Compatible,
            ODataMessageWriterSettings writerSettings)
        {
            IJsonWriter jsonWriter;
            if (container == null)
            {
                jsonWriter = new JsonWriter(textWriter, isIeee754Compatible);
            }
            else
            {
                IJsonWriterFactory jsonWriterFactory = container.GetRequiredService<IJsonWriterFactory>();
                jsonWriter = jsonWriterFactory.CreateJsonWriter(textWriter, isIeee754Compatible);
                Debug.Assert(jsonWriter != null, "jsonWriter != null");
            }

            if (jsonWriter is Json.JsonWriter defaultJsonWriter)
            {
                defaultJsonWriter.ArrayPool = writerSettings.ArrayPool;
            }

            return jsonWriter;
        }

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriter"/> that can write
        /// directly to the output stream. Returns null if unable to create
        /// such a writer with the given constraints.
        /// </summary>
        /// <param name="factory">The factory used to create the <see cref="IJsonWriter"/> instance.</param>
        /// <param name="outputStream">The output stream to write to.</param>
        /// <param name="isIeee754Compatible">True if the writer should write decimals and longs as strings.</param>
        /// <param name="encoding">The text encoding of the output data.</param>
        /// <returns>The JSON writer instance, or null if none could be created.</returns>
        private static IJsonWriter CreateJsonWriter(
            IStreamBasedJsonWriterFactory factory,
            Stream outputStream,
            bool isIeee754Compatible,
            Encoding encoding)
        {
            IJsonWriter jsonWriter = factory.CreateJsonWriter(outputStream, isIeee754Compatible, encoding);

            if (jsonWriter == null)
            {
                throw new ODataException(Strings.ODataMessageWriter_StreamBasedJsonWriterFactory_ReturnedNull(encoding.WebName, isIeee754Compatible));
            }

            return jsonWriter;
        }

        /// <summary>
        /// Creates a new JSON writer of <see cref="IJsonWriterAsync"/> with support for writing asynchronously.
        /// </summary>
        /// <param name="container">The dependency injection container to get related services.</param>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <param name="isIeee754Compatible">true if the writer should write large integers as strings.</param>
        /// <param name="writerSettings">Configuration settings for the OData writer.</param>
        /// <returns>An asynchronous JSON writer.</returns>
        private static IJsonWriterAsync CreateAsynchronousJsonWriter(
            IServiceProvider container,
            TextWriter textWriter,
            bool isIeee754Compatible,
            ODataMessageWriterSettings writerSettings)
        {
            IJsonWriterAsync asynchronousJsonWriter;
            if (container == null)
            {
                // we don't create a default JsonWriter here because that's already handled by the CreateJsonWriter() method.
                return null;
            }

            IJsonWriterFactoryAsync asynchronousJsonWriterFactory = container.GetRequiredService<IJsonWriterFactoryAsync>();
            asynchronousJsonWriter = asynchronousJsonWriterFactory.CreateAsynchronousJsonWriter(textWriter, isIeee754Compatible);
            Debug.Assert(asynchronousJsonWriter != null, "asynchronousJsonWriter != null");

            if (asynchronousJsonWriter is Json.JsonWriter defaultJsonWriter)
            {
                defaultJsonWriter.ArrayPool = writerSettings.ArrayPool;
            }

            return asynchronousJsonWriter;
        }

        private static IJsonWriterAsync CreateAsynchronousJsonWriter(
            IStreamBasedJsonWriterFactory factory,
            Stream outputStream,
            bool isIeee754Compatible,
            Encoding encoding)
        {
            IJsonWriterAsync jsonWriter = factory.CreateAsynchronousJsonWriter(outputStream, isIeee754Compatible, encoding);

            if (jsonWriter == null)
            {
                throw new ODataException(Strings.ODataMessageWriter_StreamBasedJsonWriterFactory_ReturnedNull(encoding.WebName, isIeee754Compatible));
            }

            return jsonWriter;
        }

        /// <summary>
        /// Ensures that both <see cref="jsonWriter"/> and <see cref="asynchronousJsonWriter"/>
        /// members are set and they refer to the same instance, otherwise it throws
        /// an exception.
        /// </summary>
        /// <exception cref="ODataException"></exception>
        /// <remarks>
        /// Asynchronous support is not implemented in Microsoft.Spatial library.
        /// To write spatial data, we rely on the synchronous PrimitiveConverter.Instance.WriteJsonLight(object, IJsonWriter) method.
        /// When writing asynchronously we wrap this method in a Task. WriteJsonLight method takes an
        /// IJsonWriter parameter while the asynchronous writer is declared as IJsonWriterAsync.
        /// When writing asynchronously, we have to ensure that the IJsonWriter that is used
        /// for writing spatial data is the same instance as the IJsonWriterAsync used for writing
        /// everything else in order to guarantee that the writer state is correctly maintained
        /// throughout the writing process (e.g. keeping track of the writer's current scope).
        /// When an IJsonWriterAsync is provided, it must also implement IJsonWriter so that the same instance
        /// can be reused for spatial data. If we somehow end up with 2 separate instances, then we fail early with an exception.
        /// Merging IJsonWriter and IJsonWriterAsync interface in a major release will simplify this.
        /// </remarks>
        private void EnsureJsonWritersReferenceTheSameInstance()
        {
            if (this.jsonWriter != null
                && this.asynchronousJsonWriter == null
                && this.jsonWriter is IJsonWriterAsync jsonWriterAsync)
            {
                this.asynchronousJsonWriter = jsonWriterAsync;
                return;
            }

            if (this.asynchronousJsonWriter != null && this.jsonWriter == null)
            {
                if (this.asynchronousJsonWriter is IJsonWriter syncJsonWriter)
                {
                    this.jsonWriter = syncJsonWriter;
                    return;
                }

                throw new ODataException(Strings.ODataMessageWriter_IJsonWriterAsync_Must_Implement_IJsonWriter);
            }

            if (this.asynchronousJsonWriter != null && !object.ReferenceEquals(this.asynchronousJsonWriter, this.jsonWriter))
            {
                throw new ODataException(Strings.ODataMessageWriter_IJsonWriter_And_IJsonWriterAsync_Are_Different_Instances);
            }
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
            ODataJsonLightWriter odataJsonWriter = new ODataJsonLightWriter(this, entitySet, resourceType, /*writingResourceSet*/true, writingParameter, writingDelta);
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
            ODataJsonLightDeltaWriter odataJsonDeltaWriter = new ODataJsonLightDeltaWriter(this, entitySet, entityType);
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
            ODataJsonLightWriter odataJsonWriter = new ODataJsonLightWriter(this, navigationSource, resourceType, /*writingResourceSet*/false);
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
        /// <param name="operation">The operation whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        private ODataParameterWriter CreateODataParameterWriterImplementation(IEdmOperation operation)
        {
            ODataJsonLightParameterWriter jsonLightParameterWriter = new ODataJsonLightParameterWriter(this, operation);
            this.outputInStreamErrorListener = jsonLightParameterWriter;
            return jsonLightParameterWriter;
        }

        /// <summary>
        /// Creates a concrete <see cref="ODataJsonLightBatchWriter" /> instance.
        /// </summary>
        /// <returns>The newly created batch writer.</returns>
        private ODataBatchWriter CreateODataBatchWriterImplementation()
        {
            ODataBatchWriter batchWriter = new ODataJsonLightBatchWriter(this);
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

            JsonLightInstanceAnnotationWriter instanceAnnotationWriter = new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(this), this.TypeNameOracle);
            ODataJsonWriterUtils.WriteError(this.JsonWriter, instanceAnnotationWriter.WriteInstanceAnnotationsForError, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, /*writingJsonLight*/ true);
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

            JsonLightInstanceAnnotationWriter instanceAnnotationWriter = new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(this), this.TypeNameOracle);
            await ODataJsonWriterUtils.WriteErrorAsync(
                this.AsynchronousJsonWriter,
                instanceAnnotationWriter.WriteInstanceAnnotationsForErrorAsync,
                error,
                includeDebugInformation,
                this.MessageWriterSettings.MessageQuotas.MaxNestingDepth).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataJsonLightPropertySerializer jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this, /*initContextUriBuilder*/ true);
            jsonLightPropertySerializer.WriteTopLevelProperty(property);
        }

        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WritePropertyImplementationAsync(ODataProperty property)
        {
            ODataJsonLightPropertySerializer jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this, /*initContextUriBuilder*/ true);
            await jsonLightPropertySerializer.WriteTopLevelPropertyAsync(property)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        private void WriteServiceDocumentImplementation(ODataServiceDocument serviceDocument)
        {
            ODataJsonLightServiceDocumentSerializer jsonLightServiceDocumentSerializer = new ODataJsonLightServiceDocumentSerializer(this);
            jsonLightServiceDocumentSerializer.WriteServiceDocument(serviceDocument);
        }

        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteServiceDocumentImplementationAsync(ODataServiceDocument serviceDocument)
        {
            ODataJsonLightServiceDocumentSerializer jsonLightServiceDocumentSerializer = new ODataJsonLightServiceDocumentSerializer(this);
            await jsonLightServiceDocumentSerializer.WriteServiceDocumentAsync(serviceDocument)
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
            ODataJsonLightSerializer jsonLightSerializer = new ODataJsonLightSerializer(this, false);
            jsonLightSerializer.WriteTopLevelError(error, includeDebugInformation);
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
            ODataJsonLightSerializer jsonLightSerializer = new ODataJsonLightSerializer(this, false);
            await jsonLightSerializer.WriteTopLevelErrorAsync(error, includeDebugInformation)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links);
        }

        /// <summary>
        /// Asynchronously writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteEntityReferenceLinksImplementationAsync(ODataEntityReferenceLinks links)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            await jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinksAsync(links)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLink(link);
        }

        /// <summary>
        /// Asynchronously writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteEntityReferenceLinkImplementationAsync(ODataEntityReferenceLink link)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            await jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinkAsync(link)
                .ConfigureAwait(false);
        }
    }
}
