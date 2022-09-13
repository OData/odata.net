//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataMessageReader which allows for transparent monitoring and changing
    /// of the reader behavior.
    /// </summary>
    public sealed class ODataMessageReaderTestWrapper : IDisposable
    {
        /// <summary>
        /// The wrapped message reader.
        /// </summary>
        private readonly ODataMessageReader messageReader;

        /// <summary>
        /// The message reader settings of the wrapped message reader.
        /// </summary>
        private readonly ODataMessageReaderSettings messageReaderSettings;

        /// <summary>
        /// The test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// The message to read from.
        /// </summary>
        private readonly TestMessage message;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageReader">The message reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <remarks>
        /// This constructor is used if not special checks against a test message should be performed. 
        /// Use the constructor overload that takes a <see cref="TestMessage"/> argument to enforce checks
        /// around disposal of the message.
        /// </remarks>
        public ODataMessageReaderTestWrapper(ODataMessageReader messageReader, ODataMessageReaderSettings messageReaderSettings, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageReader, "messageReader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.messageReader = messageReader;
            this.messageReaderSettings = messageReaderSettings;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageReader">The message reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="message">The message to read from.</param>
        /// <remarks>
        /// This constructor is used if special checks against <paramref name="message"/> should be performed. 
        /// Use the constructor overload that does not take a <see cref="TestMessage"/> argument to prevent any checks
        /// around disposal of the message.
        /// </remarks>
        public ODataMessageReaderTestWrapper(ODataMessageReader messageReader, ODataMessageReaderSettings messageReaderSettings, ReaderTestConfiguration testConfiguration, TestMessage message)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageReader, "messageReader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");
            ExceptionUtilities.CheckArgumentNotNull(message, "message");

            this.messageReader = messageReader;
            this.messageReaderSettings = messageReaderSettings;
            this.testConfiguration = testConfiguration;
            this.message = message;

            ExceptionUtilities.Assert(this.message.TestStream == null || this.message.TestStream.DisposeCount == 0, "If the underlying message stream is a TestStream, its dispose count must be 0.");
            ExceptionUtilities.Assert(!this.message.StreamRetrieved, "GetMessage and GetMessageAsync must not be called previously on the given message.");
        }

        /// <summary>
        /// Returns the underlyin message reader. Use only when necessary.
        /// </summary>
        public ODataMessageReader MessageReader
        {
            get { return this.messageReader; }
        }

        /// <summary>
        /// The message reader settings of the wrapped message reader.
        /// </summary>
        public ODataMessageReaderSettings MessageReaderSettings
        {
            get
            {
                return this.messageReaderSettings;
            }
        }

        /// <summary>
        /// Determines the potential payload kinds and formats of the payload being read and returns it.
        /// </summary>
        /// <returns>The set of potential payload kinds and formats for the payload being read by this reader.</returns>
        public IEnumerable<ODataPayloadKindDetectionResult> DetectPayloadKind()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.DetectPayloadKind();
            }
            else
            {
                return this.messageReader.DetectPayloadKindAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceSetReader()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceSetReader(), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceSetReaderAsync().ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceSetReader(IEdmStructuredType expectedBaseEntityType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceSetReader(expectedBaseEntityType), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceSetReaderAsync(expectedBaseEntityType).ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read a feed.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceSetReader(IEdmEntitySet entitySet, IEdmStructuredType expectedBaseEntityType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceSetReader(entitySet, expectedBaseEntityType), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceSetReaderAsync(entitySet, expectedBaseEntityType).ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceReader()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceReader(), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceReaderAsync().ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="expectedResourceType">The expected entity type for the entry to be read.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceReader(IEdmStructuredType expectedResourceType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceReader(expectedResourceType), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceReaderAsync(expectedResourceType).ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataReader" /> to read an entry.
        /// </summary>
        /// <param name="entitySet">The entity set we are going to read entities for.</param>
        /// <param name="expectedResourceType">The expected entity type for the entry to be read.</param>
        /// <returns>The created reader.</returns>
        public ODataReader CreateODataResourceReader(IEdmEntitySet entitySet, IEdmStructuredType expectedResourceType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataReaderTestWrapper(this.messageReader.CreateODataResourceReader(entitySet, expectedResourceType), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataResourceReaderAsync(entitySet, expectedResourceType).ContinueWith(
                    task => new ODataReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <returns>The created collection reader.</returns>
        public ODataCollectionReader CreateODataCollectionReader()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionReaderTestWrapper(this.messageReader.CreateODataCollectionReader(), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataCollectionReaderAsync().ContinueWith(
                    task => new ODataCollectionReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="expectedItemType">The expected item type to pass to the reader.</param>
        /// <returns>The created collection reader.</returns>
        public ODataCollectionReader CreateODataCollectionReader(IEdmTypeReference expectedItemType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionReaderTestWrapper(this.messageReader.CreateODataCollectionReader(expectedItemType), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataCollectionReaderAsync(expectedItemType).ContinueWith(
                    task => new ODataCollectionReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader" /> to read a collection of primitive or complex values (as result of a service operation invocation).
        /// </summary>
        /// <param name="producingFunctionImport">The function import producing the collection to be read.</param>
        /// <returns>The created collection reader.</returns>
        public ODataCollectionReader CreateODataCollectionReader(IEdmOperationImport producingFunctionImport)
        {
            IEdmTypeReference itemTypeReference = ((IEdmCollectionTypeReference)producingFunctionImport.Operation.ReturnType).GetCollectionItemType();
            if (this.testConfiguration.Synchronous)
            {
                return new ODataCollectionReaderTestWrapper(this.messageReader.CreateODataCollectionReader(itemTypeReference), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataCollectionReaderAsync(itemTypeReference).ContinueWith(
                    task => new ODataCollectionReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataBatchReader" /> to read a batch of requests or responses.
        /// </summary>
        /// <returns>The created batch reader.</returns>
        public ODataBatchReaderTestWrapper CreateODataBatchReader()
        {
            if (this.testConfiguration.Synchronous)
            {
                return new ODataBatchReaderTestWrapper(this.messageReader.CreateODataBatchReader(), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataBatchReaderAsync().ContinueWith(
                    task => new ODataBatchReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataParameterReader" /> to read the parameters for <paramref name="expectedFunctionImport"/>.
        /// </summary>
        /// <param name="expectedFunctionImport">The expected function import whose parameters are being read.</param>
        /// <returns>The created parameter reader.</returns>
        public ODataParameterReaderTestWrapper CreateODataParameterReader(IEdmOperationImport expectedFunctionImport)
        {
            IEdmOperation operation = expectedFunctionImport != null ? expectedFunctionImport.Operation : null;
            if (this.testConfiguration.Synchronous)
            {
                return new ODataParameterReaderTestWrapper(this.messageReader.CreateODataParameterReader(operation), this.testConfiguration);
            }
            else
            {
                return this.messageReader.CreateODataParameterReaderAsync(operation).ContinueWith(
                    task => new ODataParameterReaderTestWrapper(task.Result, this.testConfiguration),
                    TaskContinuationOptions.ExecuteSynchronously)
                    .WaitForResult();
            }
        }

        /// <summary>
        /// Reads a service document payload.
        /// </summary>
        /// <returns>The service document read.</returns>
        public ODataServiceDocument ReadServiceDocument()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadServiceDocument();
            }
            else
            {
                return this.messageReader.ReadServiceDocumentAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Reads a metadata document payload.
        /// </summary>
        /// <returns>The metadata document read.</returns>
        public IEdmModel ReadMetadataDocument()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadMetadataDocument();
            }
            else
            {
                throw new ODataTestException("Asynchronous metadata reading is not supported.");
            }
        }

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadProperty();
            }
            else
            {
                return this.messageReader.ReadPropertyAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="expectedPropertyType">The expected type of the property to read.</param>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty(IEdmTypeReference expectedPropertyType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadProperty(expectedPropertyType);
            }
            else
            {
                return this.messageReader.ReadPropertyAsync(expectedPropertyType).WaitForResult();
            }
        }

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="property">The metadata of the property to read.</param>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty(IEdmStructuralProperty property)
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadProperty(property);
            }
            else
            {
                return this.messageReader.ReadPropertyAsync(property).WaitForResult();
            }
        }

        /// <summary>
        /// Reads an <see cref="ODataProperty"/> as message payload.
        /// </summary>
        /// <param name="producingFunctionImport">The function import producing the collection to be read.</param>
        /// <returns>The property read from the payload.</returns>
        public ODataProperty ReadProperty(IEdmOperationImport producingFunctionImport)
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadProperty(producingFunctionImport.Operation.ReturnType);
            }
            else
            {
                return this.messageReader.ReadPropertyAsync(producingFunctionImport.Operation.ReturnType).WaitForResult();
            }
        }

        /// <summary>
        /// Reads an <see cref="ODataError"/> as the message payload.
        /// </summary>
        /// <returns>The <see cref="ODataError"/> read from the message payload.</returns>
        public ODataError ReadError()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadError();
            }
            else
            {
                return this.messageReader.ReadErrorAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Reads the result of a $ref query (entity reference links) as the message payload.
        /// </summary>
        /// <returns>The entity reference links read as message payload.</returns>
        public ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadEntityReferenceLinks();
            }
            else
            {
                return this.messageReader.ReadEntityReferenceLinksAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Reads a singleton result of a $ref query (entity reference link) as the message payload.
        /// </summary>
        /// <returns>The entity reference link read from the message payload.</returns>
        public ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadEntityReferenceLink();
            }
            else
            {
                return this.messageReader.ReadEntityReferenceLinkAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Reads a single value as the message body.
        /// </summary>
        /// <param name="expectedValueType">The (primitive) type of the value to be read; or null if not expected type is available.</param>
        /// <returns>The read value.</returns>
        public object ReadValue(IEdmTypeReference expectedValueType)
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.messageReader.ReadValue(expectedValueType);
            }
            else
            {
                return this.messageReader.ReadValueAsync(expectedValueType).WaitForResult();
            }
        }

        /// <summary>
        /// Disposes the inner message reader.
        /// </summary>
        public void Dispose()
        {
            this.messageReader.Dispose();
            // Calling IDisposable.Dispose  twice should not throw.
            this.messageReader.Dispose();
            if (this.message != null && this.message.TestStream != null && this.message.StreamRetrieved)
            {
                bool enableMessageStreamDisposal = testConfiguration.MessageReaderSettings.EnableMessageStreamDisposal;
                TestStream messageStream = this.message.TestStream;
                if (enableMessageStreamDisposal == false && messageStream != null)
                {
                    ExceptionUtilities.Assert(messageStream.DisposeCount == 0, "Dispose method on the stream must not be called when EnableMessageStreamDisposal is set to false.");
                }
                else
                {
                    ExceptionUtilities.Assert(messageStream.DisposeCount == 1, "Dispose method on the stream must be called exactly once when EnableMessageStreamDisposal is set to true.");
                }
            }
        }
    }
}
