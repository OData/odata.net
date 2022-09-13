//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataMessageWriter which allows for transparent monitoring and changing
    /// of the writer behavior.
    /// </summary>
    public sealed class ODataMessageWriterTestWrapper : IDisposable
    {
        /// <summary>
        /// The wrapper message writer.
        /// </summary>
        private readonly ODataMessageWriter messageWriter;

        /// <summary>
        /// The test configuration to use.
        /// </summary>
        private readonly WriterTestConfiguration testConfiguration;

        /// <summary>
        /// The message to write to.
        /// </summary>
        private readonly TestMessage message;

        /// <summary>
        /// Assertion handler for the test.
        /// </summary>
        private readonly AssertionHandler assert;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageWriter">The message writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataMessageWriterTestWrapper(ODataMessageWriter messageWriter, WriterTestConfiguration testConfiguration)
            : this(messageWriter, testConfiguration, null, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageWriter">The message writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="messageStream">the stream of the message.</param>
        /// <param name="message">The message to write to.</param>
        /// <param name="assert">The assertion handler for the test.</param>
        public ODataMessageWriterTestWrapper(ODataMessageWriter messageWriter, WriterTestConfiguration testConfiguration, TestMessage message, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.messageWriter = messageWriter;
            this.testConfiguration = testConfiguration;
            this.message = message;
            this.assert = assert;

            if (message != null && message.TestStream != null && assert != null)
            {
                this.assert.AreEqual(0, message.TestStream.DisposeCount, "If the underlying message stream is a TestStream, its dispose count must be 0.");
                this.assert.AreEqual(false, message.StreamRetrieved, "GetMessage and GetMessageAsync must not be called privously on the given message.");
            }
        }

        /// <summary>
        /// Returns the underlying message writer.
        /// </summary>
        public ODataMessageWriter MessageWriter
        {
            get { return this.messageWriter; }
        }

        /// <summary>
        /// Returns the underlying test message.
        /// </summary>
        public TestMessage Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the private <see cref="ODataMessageWriterSettings"/> field of the message writer.
        /// </summary>
        /// <remarks>This method uses private reflection.</remarks>
        public ODataMessageWriterSettings PrivateSettings
        {
            get
            {
                return (ODataMessageWriterSettings)ReflectionUtils.GetField(this.messageWriter, "settings");
            }
        }

        /// <summary>
        /// Gets the private <see cref="ODataFormat"/> field of the message writer.
        /// </summary>
        /// <remarks>This method uses private reflection.</remarks>
        public ODataFormat PrivateFormat
        {
            get
            {
                return (ODataFormat)ReflectionUtils.GetField(this.messageWriter, "format");
            }
        }

        /// <summary>
        /// Gets the private <see cref="PrivateEncoding"/> field of the message writer.
        /// </summary>
        /// <remarks>This method uses private reflection.</remarks>
        public Encoding PrivateEncoding
        {
            get
            {
                return (Encoding)ReflectionUtils.GetField(this.messageWriter, "encoding");
            }
        }

        /// <summary>
        /// Gets the private boundary string field of the message writer.
        /// </summary>
        /// <remarks>This method uses private reflection.</remarks>
        public string PrivateBatchBoundary
        {
            get
            {
                return (string)ReflectionUtils.GetField(this.messageWriter, "batchBoundary");
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceSetWriter()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceSetWriter(), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceSetWriterAsync().ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceSetWriter(IEdmEntitySet entitySet)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceSetWriter(entitySet), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceSetWriterAsync(entitySet).ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }
        
        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceSetWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceSetWriter(entitySet, entityType), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceSetWriterAsync(entitySet, entityType).ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceWriter()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceWriter(), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceWriterAsync().ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceWriter(IEdmEntitySet entitySet)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceWriter(entitySet), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceWriterAsync(entitySet).ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>The created writer.</returns>
        public ODataWriter CreateODataResourceWriter(IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataWriterTestWrapper(this.messageWriter.CreateODataResourceWriter(entitySet, entityType), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataResourceWriterAsync(entitySet, entityType).ContinueWith(
                    task => new ODataWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry or a feed.
        /// </summary>
        /// <param name="isFeed">Specify whether to create a writer for a feed or an entry.</param>
        /// <returns>If <paramref name="isFeed"/> is true, return the result of CreateODataResourceSetWriter, otherwise return the result of CreateODataResourceWriter.</returns>
        internal ODataWriter CreateODataWriter(bool isFeed)
        {
            return isFeed ? this.CreateODataResourceSetWriter() : this.CreateODataResourceWriter();
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry or a feed.
        /// </summary>
        /// <param name="isFeed">Specify whether to create a writer for a feed or an entry.</param>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <returns>If <paramref name="isFeed"/> is true, return the result of CreateODataResourceSetWriter, otherwise return the result of CreateODataResourceWriter.</returns>
        internal ODataWriter CreateODataWriter(bool isFeed, IEdmEntitySet entitySet)
        {
            return isFeed ? this.CreateODataResourceSetWriter(entitySet) : this.CreateODataResourceWriter(entitySet);
        }

        /// <summary>
        /// Creates an <see cref="ODataWriter" /> to write an entry or a feed.
        /// </summary>
        /// <param name="isFeed">Specify whether to create a writer for a feed or an entry.</param>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <returns>If <paramref name="isFeed"/> is true, return the result of CreateODataResourceSetWriter, otherwise return the result of CreateODataResourceWriter.</returns>
        internal ODataWriter CreateODataWriter(bool isFeed, IEdmEntitySet entitySet, IEdmEntityType entityType)
        {
            return isFeed ? this.CreateODataResourceSetWriter(entitySet, entityType) : this.CreateODataResourceWriter(entitySet, entityType);
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection writer.</returns>
        public ODataCollectionWriter CreateODataCollectionWriter()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionWriterTestWrapper(this.messageWriter.CreateODataCollectionWriter(), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataCollectionWriterAsync().ContinueWith(
                    task => new ODataCollectionWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter" /> to write a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="collectionType">The type of the collection being written or null.</param>
        /// <returns>The created collection writer.</returns>
        public ODataCollectionWriter CreateODataCollectionWriter(IEdmTypeReference itemTypeReference)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionWriterTestWrapper(this.messageWriter.CreateODataCollectionWriter(itemTypeReference), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataCollectionWriterAsync(itemTypeReference).ContinueWith(
                    task => new ODataCollectionWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchWriterTestWrapper" /> to write a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch writer.</returns>
        public ODataBatchWriterTestWrapper CreateODataBatchWriter()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataBatchWriterTestWrapper(this.messageWriter.CreateODataBatchWriter(), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataBatchWriterAsync().ContinueWith(
                    task => new ODataBatchWriterTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterWriter" /> to write a parameter payload.
        /// </summary>
        /// <param name="functionImport">The function import whose parameters will be written.</param>
        /// <returns>The created parameter writer.</returns>
        public ODataParameterWriter CreateODataParameterWriter(IEdmOperationImport functionImport)
        {
            IEdmOperation operation = functionImport != null ? functionImport.Operation : null;
            if (this.testConfiguration.Synchronous)
            {
                return new ODataParameterWriterTestWrapper(this.messageWriter.CreateODataParameterWriter(operation), this.testConfiguration);
            }
            else
            {
                return this.messageWriter.CreateODataParameterWriterAsync(operation)
                    .ContinueWith(task => new ODataParameterWriterTestWrapper(task.Result, this.testConfiguration), TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="serviceDocument"/> 
        /// as message payload.
        /// </summary>
        /// <param name="serviceDocument">The default serviceDocument to write in the service document.</param>
        public void WriteServiceDocument(ODataServiceDocument serviceDocument)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteServiceDocument(serviceDocument);
            }
            else
            {
                this.messageWriter.WriteServiceDocumentAsync(serviceDocument).Wait();
            }
        }

        /// <summary>
        /// Writes a service document with the specified <paramref name="defaultWorkspace"/> 
        /// as message payload.
        /// </summary>
        public void WriteMetadataDocument()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteMetadataDocument();
            }
            else
            {
                throw new TaupoNotSupportedException("Asynchronous metadata writing is not supported.");
            }
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        public void WriteProperty(ODataProperty property)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteProperty(property);
            }
            else
            {
                this.messageWriter.WritePropertyAsync(property).Wait();
            }
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <param name="owningType">The owning type for the <paramref name="property"/> or null if no metadata is available.</param>
        public void WriteProperty(ODataProperty property, IEdmStructuredType owningType)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteProperty(property);
            }
            else
            {
                this.messageWriter.WritePropertyAsync(property).Wait();
            }
        }

        /// <summary>
        /// Writes an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The property to write.</param>
        /// <param name="producingFunctionImport">The function import which return value is being written.</param>
        public void WriteProperty(ODataProperty property, IEdmOperationImport producingFunctionImport)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteProperty(property);
            }
            else
            {
                this.messageWriter.WritePropertyAsync(property).Wait();
            }
        }

        /// <summary>
        /// Writes an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <param name="error">The error to write.</param>
        /// <param name="includeDebugInformation">
        /// A flag indicating whether debug information (e.g., the inner error from the <paramref name="error"/>) should 
        /// be included in the payload. This should only be used in debug scenarios.
        /// </param>
        public void WriteError(ODataError error, bool includeDebugInformation)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteError(error, includeDebugInformation);
            }
            else
            {
                this.messageWriter.WriteErrorAsync(error, includeDebugInformation).Wait();
            }
        }

        /// <summary>
        /// Writes the result of a $ref query as the message payload.
        /// </summary>
        /// <param name="links">The entity reference links to write as message payload.</param>
        public void WriteEntityReferenceLinks(ODataEntityReferenceLinks links)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteEntityReferenceLinks(links);
            }
            else
            {
                this.messageWriter.WriteEntityReferenceLinksAsync(links).Wait();
            }
        }

        /// <summary>
        /// Writes a singleton result of a $ref query as the message payload.
        /// </summary>
        /// <param name="link">The entity reference link to write as message payload.</param>
        public void WriteEntityReferenceLink(ODataEntityReferenceLink link)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteEntityReferenceLink(link);
            }
            else
            {
                this.messageWriter.WriteEntityReferenceLinkAsync(link).Wait();
            }
        }

        /// <summary>
        /// Writes a single value as the message body.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue(object value)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.messageWriter.WriteValue(value);
            }
            else
            {
                this.messageWriter.WriteValueAsync(value).Wait();
            }
        }

        /// <summary>
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write methods.
        /// If it is sufficient to set the headers when the write methods on the message writer 
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="payloadKind">The kind of payload to be written with the message writer.</param>
        /// <returns>The <see cref="ODataFormat"/> used for the specified <paramref name="payloadKind"/>.</returns>
        public ODataFormat SetHeadersForPayload(ODataPayloadKind payloadKind)
        {
            return ODataUtils.SetHeadersForPayload(this.messageWriter, payloadKind);
        }

        /// <summary>
        /// Disposes the inner message writer.
        /// </summary>
        public void Dispose()
        {
            if (this.message != null && this.message.TestStream != null)
            {
                this.Message.TestStream.FailSynchronousCalls = false;
            }

            this.messageWriter.Dispose();
            // Calling IDisposable.Dispose  should not throw.
            this.messageWriter.Dispose();
            if(message != null && message.TestStream != null && message.StreamRetrieved)
            {
                bool enableMessageStreamDisposal = testConfiguration.MessageWriterSettings.EnableMessageStreamDisposal;
                if (enableMessageStreamDisposal)
                {
                    this.assert.AreEqual(1, message.TestStream.DisposeCount, "Dispose method on the stream must be called when EnableMessageStreamDisposal is set to true.");
                }
                else
                {
                    this.assert.AreEqual(0, message.TestStream.DisposeCount, "Dispose method on the stream must not be called exactly once when EnableMessageStreamDisposal is set to false.");
                }
            }
        }
    }
}
