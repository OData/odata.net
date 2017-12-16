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
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// JsonLight format output context.
    /// </summary>
    internal sealed class ODataJsonLightOutputContext : ODataOutputContext
    {
        /// <summary>
        /// The json metadata level (i.e., full, none, minimal) being written.
        /// </summary>
        private readonly JsonLightMetadataLevel metadataLevel;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;

        /// <summary>The text writer created for the output stream.</summary>
        private TextWriter textWriter;

        /// <summary>The JSON writer to write to.</summary>
        /// <remarks>This field is also used to determine if the output context has been disposed already.</remarks>
        private IJsonWriter jsonWriter;

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

                Stream outputStream;
                if (this.Synchronous)
                {
                    outputStream = this.messageOutputStream;
                }
                else
                {
                    this.asynchronousOutputStream = new AsyncBufferedStream(this.messageOutputStream);
                    outputStream = this.asynchronousOutputStream;
                }

                this.textWriter = new StreamWriter(outputStream, messageInfo.Encoding);

                // COMPAT 2: JSON indentation - WCFDS indents only partially, it inserts newlines but doesn't actually insert spaces for indentation
                // in here we allow the user to specify if true indentation should be used or if the limited functionality is enough.
                this.jsonWriter = CreateJsonWriter(this.Container, this.textWriter, messageInfo.MediaType.HasIeee754CompatibleSetToTrue());
            }
            catch (Exception e)
            {
                // Dispose the message stream if we failed to create the input context.
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.messageOutputStream.Dispose();
                }

                throw;
            }

            Uri metadataDocumentUri = messageWriterSettings.MetadataDocumentUri;
            this.metadataLevel = JsonLightMetadataLevel.Create(messageInfo.MediaType, metadataDocumentUri, this.Model, this.WritingResponse);
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
            this.jsonWriter = CreateJsonWriter(messageInfo.Container, textWriter, true /*isIeee754Compatible*/);
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

#if PORTABLELIB
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

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, false));
        }
#endif


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

#if PORTABLELIB
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

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, false, true));
        }
#endif

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

#if PORTABLELIB
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

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataResourceWriterImplementation(navigationSource, resourceType));
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>A running task for the created collection writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataCollectionWriter> CreateODataCollectionWriterAsync(IEdmTypeReference itemTypeReference)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataCollectionWriterImplementation(itemTypeReference));
        }
#endif

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

#if PORTABLELIB
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
#endif

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

#if PORTABLELIB

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

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataResourceSetWriterImplementation(entitySet, resourceType, true, false));
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="operation">The operation import whose parameters will be written.</param>
        /// <returns>A running task for the created parameter writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        public override Task<ODataParameterWriter> CreateODataParameterWriterAsync(IEdmOperation operation)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataParameterWriterImplementation(operation));
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write</param>
        /// <returns>A task representing the asynchronous operation of writing the property.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        public override Task WritePropertyAsync(ODataProperty property)
        {
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
        public override void WriteError(ODataError error, bool includeDebugInformation)
        {
            this.AssertSynchronous();

            this.WriteErrorImplementation(error, includeDebugInformation);
            this.Flush();
        }

#if PORTABLELIB
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
        public override Task WriteErrorAsync(ODataError error, bool includeDebugInformation)
        {
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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously flush the writer.
        /// </summary>
        /// <returns>Task which represents the pending flush operation.</returns>
        /// <remarks>The method should not throw directly if the flush operation itself fails, it should instead return a faulted task.</remarks>
        public Task FlushAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    // In the async case the underlying stream is the async buffered stream, which ignores Flush call.
                    this.jsonWriter.Flush();

                    Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
                    return this.asynchronousOutputStream.FlushAsync();
                })
                .FollowOnSuccessWithTask((asyncBufferedStreamFlushTask) => this.messageOutputStream.FlushAsync());
        }
#endif

         /// <summary>
        /// Flushes all buffered data to the underlying stream synchronously.
        /// </summary>
        internal void FlushBuffers()
        {
            if (this.asynchronousOutputStream != null)
            {
                this.asynchronousOutputStream.FlushSync();
            }
        }

#if PORTABLELIB
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
            else
            {
                return TaskUtils.CompletedTask;
            }
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously creates an <see cref="ODataBatchWriter" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>A running task for the created batch writer.</returns>
        /// <remarks>We don't plan to make this public!</remarks>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataBatchWriter> CreateODataBatchWriterAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataBatchWriterImplementation());
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
        internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
        {
            this.AssertSynchronous();

            this.WriteInStreamErrorImplementation(error, includeDebugInformation);
            this.Flush();
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
        internal override Task WriteInStreamErrorAsync(ODataError error, bool includeDebugInformation)
        {
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

#if PORTABLELIB
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

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataDeltaWriterImplementation(entitySet, entityType));
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a service document with the specified <paramref name="serviceDocument"/>
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        /// <returns>A task representing the asynchronous operation of writing the service document.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteServiceDocumentAsync(ODataServiceDocument serviceDocument)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteServiceDocumentImplementation(serviceDocument);
                    return this.FlushAsync();
                });
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        /// <returns>A task representing the asynchronous writing of the entity reference links.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinksAsync(ODataEntityReferenceLinks links)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinksImplementation(links);
                    return this.FlushAsync();
                });
        }
#endif

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

#if PORTABLELIB
        /// <summary>
        /// Asynchronously writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The link result to write as message payload.</param>
        /// <returns>A running task representing the writing of the link.</returns>
        /// <remarks>It is the responsibility of this method to flush the output before the task finishes.</remarks>
        internal override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink link)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    this.WriteEntityReferenceLinkImplementation(link);
                    return this.FlushAsync();
                });
        }
#endif

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "textWriter", Justification = "We don't dispose the jsonWriter or textWriter, instead we dispose the underlying stream directly.")]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.messageOutputStream != null)
                {
                    // JsonWriter.Flush will call the underlying TextWriter.Flush.
                    // The TextWriter.Flush (Which is in fact StreamWriter.Flush) will call the underlying Stream.Flush.
                    this.jsonWriter.Flush();

                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitly.
                    if (this.asynchronousOutputStream != null)
                    {
                        this.asynchronousOutputStream.FlushSync();
                        this.asynchronousOutputStream.Dispose();
                    }

                    // Dispose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.textWriter = null;
                this.jsonWriter = null;
            }

            base.Dispose(disposing);
        }

        private static IJsonWriter CreateJsonWriter(IServiceProvider container, TextWriter textWriter, bool isIeee754Compatible)
        {
            if (container == null)
            {
                return new JsonWriter(textWriter, isIeee754Compatible);
            }

            var jsonWriterFactory = container.GetRequiredService<IJsonWriterFactory>();
            var jsonWriter = jsonWriterFactory.CreateJsonWriter(textWriter, isIeee754Compatible);
            Debug.Assert(jsonWriter != null, "jsonWriter != null");

            return jsonWriter;
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
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataJsonLightPropertySerializer jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(this, /*initContextUriBuilder*/ true);
            jsonLightPropertySerializer.WriteTopLevelProperty(property);
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
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            ODataJsonLightEntityReferenceLinkSerializer jsonLightEntityReferenceLinkSerializer = new ODataJsonLightEntityReferenceLinkSerializer(this);
            jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links);
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
    }
}
