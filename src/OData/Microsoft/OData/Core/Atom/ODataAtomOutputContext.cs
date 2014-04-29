//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// ATOM format output context.
    /// </summary>
    internal sealed class ODataAtomOutputContext : ODataOutputContext
    {
        /// <summary>
        /// The oracle to use to determine the type name to write for entries and values.
        /// </summary>
        private readonly AtomAndVerboseJsonTypeNameOracle typeNameOracle = new AtomAndVerboseJsonTypeNameOracle();

        /// <summary>The message output stream.</summary>
        private Stream messageOutputStream;

        /// <summary>The asynchronous output stream if we're writing asynchronously.</summary>
        private AsyncBufferedStream asynchronousOutputStream;

        /// <summary>The XML writer to write to.</summary>
        /// <remarks>In case of customized writers are used, this is the current writer to write to.</remarks>
        private XmlWriter xmlWriter;

        /// <summary>An in-stream error listener to notify when in-stream error is to be written. Or null if we don't need to notify anybody.</summary>
        private IODataOutputInStreamErrorListener outputInStreamErrorListener;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">The format for this output context.</param>
        /// <param name="messageStream">The message stream to write the payload to.</param>
        /// <param name="encoding">The encoding to use for the payload.</param>
        /// <param name="messageWriterSettings">Configuration settings of the OData writer.</param>
        /// <param name="writingResponse">true if writing a response message; otherwise false.</param>
        /// <param name="synchronous">true if the output should be written synchronously; false if it should be written asynchronously.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        internal ODataAtomOutputContext(
            ODataFormat format,
            Stream messageStream,
            Encoding encoding,
            ODataMessageWriterSettings messageWriterSettings,
            bool writingResponse,
            bool synchronous,
            IEdmModel model,
            IODataUrlResolver urlResolver)
            : base(format, messageWriterSettings, writingResponse, synchronous, model, urlResolver)
        {
            Debug.Assert(messageStream != null, "stream != null");

            try
            {
                this.messageOutputStream = messageStream;

                Stream outputStream;
                if (synchronous)
                {
                    outputStream = messageStream;
                }
                else
                {
                    this.asynchronousOutputStream = new AsyncBufferedStream(messageStream);
                    outputStream = this.asynchronousOutputStream;
                }

                this.xmlWriter = ODataAtomWriterUtils.CreateXmlWriter(outputStream, messageWriterSettings, encoding);
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
        /// Returns the <see cref="XmlWriter"/> which is to be used to write the content of the message.
        /// </summary>
        internal XmlWriter XmlWriter
        {
            get
            {
                Debug.Assert(this.xmlWriter != null, "Trying to get XmlWriter while none is available.");
                return this.xmlWriter;
            }
        }

        /// <summary>
        /// Returns the oracle to use when determining the type name to write for entries and values.
        /// </summary>
        internal AtomAndVerboseJsonTypeNameOracle TypeNameOracle
        {
            get
            {
                return this.typeNameOracle;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        internal void VerifyNotDisposed()
        {
            if (this.messageOutputStream == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Synchronously flush the writer.
        /// </summary>
        internal void Flush()
        {
            this.AssertSynchronous();

            // XmlWriter.Flush will call the underlying Stream.Flush.
            // In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
            this.xmlWriter.Flush();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flush the writer.
        /// </summary>
        /// <returns>Task which represents the pending flush operation.</returns>
        /// <remarks>The method should not throw directly if the flush operation itself fails, it should instead return a faulted task.</remarks>
        internal Task FlushAsync()
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperationReturningTask(
                () =>
                {
                    // XmlWriter.Flush will call the underlying Stream.Flush.
                    // In the async case the underlying stream is the async buffered stream, which ignores Flush call.
                    this.xmlWriter.Flush();
            
                    Debug.Assert(this.asynchronousOutputStream != null, "In async writing we must have the async buffered stream.");
                    return this.asynchronousOutputStream.FlushAsync();
                })
                .FollowOnSuccessWithTask((asyncBufferedStreamFlushTask) => this.messageOutputStream.FlushAsync());
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
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataFeedWriter(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
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
        internal override Task<ODataWriter> CreateODataFeedWriterAsync(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataFeedWriterImplementation(entitySet, entityType));
        }
#endif

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override ODataWriter CreateODataEntryWriter(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            this.AssertSynchronous();

            return this.CreateODataEntryWriterImplementation(navigationSource, entityType);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>A running task for the created writer.</returns>
        /// <remarks>The write must flush the output when it's finished (inside the last Write call).</remarks>
        internal override Task<ODataWriter> CreateODataEntryWriterAsync(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataEntryWriterImplementation(navigationSource, entityType));
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
            this.AssertAsynchronous();

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateODataCollectionWriterImplementation(itemTypeReference));
        }
#endif

        //// ATOM format doesn't support parameter payloads

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

#if ODATALIB_ASYNC
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
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <remarks>It is the responsibility of this method to flush the output before the method returns.</remarks>
        internal override void WriteProperty(ODataProperty property)
        {
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

#if ODATALIB_ASYNC
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

#if ODATALIB_ASYNC
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

        //// Raw value is not supported by ATOM.

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            try
            {
                if (this.messageOutputStream != null)
                {
                    // XmlWriter.Flush will call the underlying Stream.Flush.
                    this.xmlWriter.Flush();

                    // XmlWriter.Dispose calls XmlWriter.Close which writes missing end elements.
                    // Thus we can't dispose the XmlWriter since that might end up writing more data into the stream right here
                    // and thus callers would have no way to prevent us from writing synchronously into the underlying stream.
                    // (note that all other cases where we write to the stream can be followed by FlushAsync which will perform
                    //  async write to the stream, but Dispose is the only case where that's not true).
                    // Also in case of in-stream error we intentionally want to not write the end elements to keep the payload invalid.
                    // In the async case the underlying stream is the async buffered stream, so we have to flush that explicitely.
                    if (this.asynchronousOutputStream != null)
                    {
                        this.asynchronousOutputStream.FlushSync();
                        this.asynchronousOutputStream.Dispose();
                    }

                    // Dipose the message stream (note that we OWN this stream, so we always dispose it).
                    this.messageOutputStream.Dispose();
                }
            }
            finally
            {
                this.messageOutputStream = null;
                this.asynchronousOutputStream = null;
                this.xmlWriter = null;
            }
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

            ODataAtomWriterUtils.WriteError(this.xmlWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataFeedWriterImplementation(IEdmEntitySetBase entitySet, IEdmEntityType entityType)
        {
            ODataAtomWriter atomWriter = new ODataAtomWriter(this, entitySet, entityType, /*writingFeed*/true);
            this.outputInStreamErrorListener = atomWriter;
            return atomWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        private ODataWriter CreateODataEntryWriterImplementation(IEdmNavigationSource navigationSource, IEdmEntityType entityType)
        {
            ODataAtomWriter atomWriter = new ODataAtomWriter(this, navigationSource, entityType, /*writingFeed*/false);
            this.outputInStreamErrorListener = atomWriter;
            return atomWriter;
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        /// <returns>The created collection writer.</returns>
        private ODataCollectionWriter CreateODataCollectionWriterImplementation(IEdmTypeReference itemTypeReference)
        {
            ODataAtomCollectionWriter atomCollectionWriter = new ODataAtomCollectionWriter(this, itemTypeReference);
            this.outputInStreamErrorListener = atomCollectionWriter;
            return atomCollectionWriter;
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        private void WritePropertyImplementation(ODataProperty property)
        {
            ODataAtomPropertyAndValueSerializer atomPropertyAndValueSerializer = new ODataAtomPropertyAndValueSerializer(this);
            atomPropertyAndValueSerializer.WriteTopLevelProperty(property);
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/> 
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The service document to write.</param>
        private void WriteServiceDocumentImplementation(ODataServiceDocument serviceDocument)
        {
            ODataAtomServiceDocumentSerializer atomServiceDocumentSerializer = new ODataAtomServiceDocumentSerializer(this);
            atomServiceDocumentSerializer.WriteServiceDocument(serviceDocument);
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
            ODataAtomSerializer atomSerializer = new ODataAtomSerializer(this);
            atomSerializer.WriteTopLevelError(error, includeDebugInformation);
        }

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        private void WriteEntityReferenceLinksImplementation(ODataEntityReferenceLinks links)
        {
            ODataAtomEntityReferenceLinkSerializer atomEntityReferenceLinkSerializer = new ODataAtomEntityReferenceLinkSerializer(this);
            atomEntityReferenceLinkSerializer.WriteEntityReferenceLinks(links);
        }

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink link)
        {
            ODataAtomEntityReferenceLinkSerializer atomEntityReferenceLinkSerializer = new ODataAtomEntityReferenceLinkSerializer(this);
            atomEntityReferenceLinkSerializer.WriteEntityReferenceLink(link);
        }
    }
}
