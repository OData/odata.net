//---------------------------------------------------------------------
// <copyright file="MessageToObjectModelReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Contracts;

    #endregion Namespaces

    /// <summary>
    /// Uses ODataMessageReader to read a specified payload kind into the OData OM.
    /// </summary>
    public class MessageToObjectModelReader
    {
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// If set to true the reader will add annotation storing the payload order.
        /// </summary>
        private bool storePayloadOrder;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageToObjectModelReader()
            : this(false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="storePayloadOrder">true if the payload order should be stored, false otherwise.</param>
        public MessageToObjectModelReader(bool storePayloadOrder)
        {
            this.storePayloadOrder = storePayloadOrder;
        }

        /// <summary>
        /// Reads the content of the <paramref name="payloadKind"/> from <paramref name="messageReader"/>
        /// and turns it into an annotated OData object model.
        /// </summary>
        /// <param name="messageReader">The message reader to use for reading.</param>
        /// <param name="payloadKind">The kind of payload to read.</param>
        /// <param name="payloadModel">The model for the payload of the message; used for reading batch parts.</param>
        /// <param name="readerMetadata">The metadata information to be used when reading the actual payload.</param>
        /// <param name="batchPayload">The expected batch payload element; this is used to guide the payload kind detection of the parts.</param>
        /// <param name="testConfiguration">The test configuration under which to read the message.</param>
        /// <returns>The OData object model with possible annotations.</returns>
        public virtual object ReadMessage(
            ODataMessageReaderTestWrapper messageReader,
            ODataPayloadKind payloadKind,
            IEdmModel payloadModel,
            IReaderMetadata readerMetadata,
            ODataPayloadElement batchPayload,
            ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageReader, "messageReader");

            ObjectModelReader reader = new ObjectModelReader(this, payloadModel, this.storePayloadOrder, testConfiguration);

            if (readerMetadata == null)
            {
                // Use empty reader metadata when none is specified to avoid repetitive checks below.
                readerMetadata = PayloadReaderTestDescriptor.ReaderMetadata.None;
            }

            switch (payloadKind)
            {
                case ODataPayloadKind.Property:
                    if (readerMetadata.StructuralProperty != null)
                    {
                        return messageReader.ReadProperty(readerMetadata.StructuralProperty);
                    }
                    else if (readerMetadata.FunctionImport != null)
                    {
                        return messageReader.ReadProperty(readerMetadata.FunctionImport);
                    }
                    else
                    {
                        return messageReader.ReadProperty(readerMetadata.ExpectedType);
                    }

                case ODataPayloadKind.ResourceSet:
                    return reader.ReadTopLevelFeed(messageReader, readerMetadata.EntitySet, readerMetadata.ExpectedType);

                case ODataPayloadKind.Resource:
                    return reader.ReadTopLevelEntry(messageReader, readerMetadata.EntitySet, readerMetadata.ExpectedType);

                case ODataPayloadKind.Collection:
                    return reader.ReadCollection(messageReader, readerMetadata.FunctionImport, readerMetadata.ExpectedType);

                case ODataPayloadKind.ServiceDocument:
                    return reader.ReadServiceDocument(messageReader);

                case ODataPayloadKind.MetadataDocument:
                    return reader.ReadMetadataDocument(messageReader);

                case ODataPayloadKind.Error:
                    return reader.ReadError(messageReader);

                case ODataPayloadKind.EntityReferenceLink:
                    return reader.ReadEntityReferenceLink(messageReader);

                case ODataPayloadKind.EntityReferenceLinks:
                    return reader.ReadEntityReferenceLinks(messageReader);

                case ODataPayloadKind.Value:
                case ODataPayloadKind.BinaryValue:
                    return reader.ReadValue(messageReader, readerMetadata.ExpectedType);

                case ODataPayloadKind.Batch:
                    return reader.ReadBatch(messageReader, batchPayload);

                case ODataPayloadKind.Parameter:
                    return reader.ReadParameters(messageReader, readerMetadata.FunctionImport);

                default:
                    ExceptionUtilities.Assert(false, "The payload kind '{0}' is not yet supported by MessageToObjectModelReader.", payloadKind);
                    return null;
            }
        }

        /// <summary>
        /// Private instance class which performs the reading. Used to store per-read variables.
        /// </summary>
        private sealed class ObjectModelReader
        {
            /// <summary>
            /// The reader used for reading and converting the parts of a batch payload.
            /// </summary>
            private readonly MessageToObjectModelReader messageToObjectModelReader;

            /// <summary>
            /// Assertion handler to use.
            /// </summary>
            private readonly AssertionHandler assert;

            /// <summary>
            /// The model used for reading the message content; currently only used for reading batch parts.
            /// </summary>
            private readonly IEdmModel payloadModel;

            /// <summary>
            /// If set to true the reader will add annotation storing the payload order.
            /// </summary>
            private readonly bool storePayloadOrder;

            /// <summary>
            /// The test configuration under which to read the message.
            /// </summary>
            private readonly ReaderTestConfiguration testConfiguration;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="assert">Assertion handler to use.</param>
            /// <param name="messageToObjectModelReader">The model used for reading the message content; currently only used for reading batch parts.</param>
            /// <param name="storePayloadOrder">If set to true the reader will add annotation storing the payload order.</param>
            /// <param name="testConfiguration">The test configuration under which to read the message.</param>
            public ObjectModelReader(
                MessageToObjectModelReader messageToObjectModelReader,
                IEdmModel payloadModel,
                bool storePayloadOrder,
                ReaderTestConfiguration testConfiguration)
            {
                this.messageToObjectModelReader = messageToObjectModelReader;
                this.payloadModel = payloadModel;
                this.assert = messageToObjectModelReader.Assert;
                this.storePayloadOrder = storePayloadOrder;
                this.testConfiguration = testConfiguration;
            }

            /// <summary>
            /// Read a feed as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="entitySet">The entity set to use for reading entry or feed payloads.</param>
            /// <param name="expectedBaseEntityType">The expected base entity type for the entities in the feed.</param>
            /// <returns>An <see cref="ODataResourceSet"/>, possibly with annotations.</returns>
            public ODataResourceSet ReadTopLevelFeed(ODataMessageReaderTestWrapper messageReader, IEdmEntitySet entitySet, IEdmTypeReference expectedBaseEntityType)
            {
                IEdmStructuredType entityType;
                if (expectedBaseEntityType == null)
                {
                    entityType = null;
                }
                else
                {
                    var tmpType = expectedBaseEntityType.Definition as IEdmCollectionType;
                    if (tmpType != null)
                    {
                        var entityTypeRef = tmpType.ElementType as IEdmStructuredTypeReference;
                        if (entityTypeRef != null)
                        {
                            entityType = entityTypeRef.Definition as IEdmStructuredType;
                        }
                        else
                        {
                            entityType = null;
                        }
                    }
                    else
                    {
                        entityType = expectedBaseEntityType.Definition as IEdmStructuredType;
                    }
                }
                ODataReader feedReader = messageReader.CreateODataResourceSetReader(entitySet, entityType);
                try
                {
                    // read the start of the feed
                    feedReader.Read();
                    this.assert.AreEqual(ODataReaderState.ResourceSetStart, feedReader.State, "Reader states don't match.");

                    ODataResourceSet feed = this.ReadFeed(feedReader);
                    this.assert.AreEqual(ODataReaderState.ResourceSetEnd, feedReader.State, "Reader states don't match.");

                    // read once more to the end-of-input
                    bool moreToRead = feedReader.Read();
                    this.assert.IsFalse(moreToRead, "Expected to reach the end of the input.");

                    return feed;
                }
                catch (Exception e)
                {
                    if (ExceptionUtilities.IsCatchable(e))
                    {
                        this.assert.AreEqual(ODataReaderState.Exception, feedReader.State, "Expected the reader to be in 'Exception' state.");
                    }

                    throw;
                }
            }

            /// <summary>
            /// Read an entry as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="entitySet">The entity set to use for reading entry or feed payloads.</param>
            /// <param name="expectedEntityTypeReference">The expected entity type to pass to the reader.</param>
            /// <returns>An <see cref="ODataResource"/>, possibly with annotations.</returns>
            public ODataResource ReadTopLevelEntry(ODataMessageReaderTestWrapper messageReader, IEdmEntitySet entitySet, IEdmTypeReference expectedEntityTypeReference)
            {
                IEdmStructuredType entityType = expectedEntityTypeReference == null ? null : expectedEntityTypeReference.Definition as IEdmStructuredType;
                ODataReader entryReader = messageReader.CreateODataResourceReader(entitySet, entityType);
                try
                {
                    // read the start of the entry
                    entryReader.Read();
                    this.assert.AreEqual(ODataReaderState.ResourceStart, entryReader.State, "Reader states don't match.");

                    ODataResource entry = this.ReadEntry(entryReader);
                    this.assert.AreEqual(ODataReaderState.ResourceEnd, entryReader.State, "Reader states don't match.");

                    // read once more to the end-of-input
                    bool moreToRead = entryReader.Read();
                    this.assert.IsFalse(moreToRead, "Expected to reach the end of the input.");

                    return entry;
                }
                catch (Exception e)
                {
                    if (ExceptionUtilities.IsCatchable(e))
                    {
                        this.assert.AreEqual(ODataReaderState.Exception, entryReader.State, "Expected the reader to be in 'Exception' state.");
                    }

                    throw;
                }
            }

            /// <summary>
            /// Read a singleton top-level entity reference link as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <returns>An <see cref="ODataEntityReferenceLink"/>, possibly with annotations.</returns>
            public ODataEntityReferenceLink ReadEntityReferenceLink(ODataMessageReaderTestWrapper messageReader)
            {
                return messageReader.ReadEntityReferenceLink();
            }

            /// <summary>
            /// Read top-level entity reference links (collection) as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <returns>An <see cref="ODataEntityReferenceLinks"/>, possibly with annotations.</returns>
            public ODataEntityReferenceLinks ReadEntityReferenceLinks(ODataMessageReaderTestWrapper messageReader)
            {
                return messageReader.ReadEntityReferenceLinks();
            }

            /// <summary>
            /// Read top-level raw value as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="expectedValueTypeReference">The expected value type to pass to the reader.</param>
            /// <returns>An <see cref="System.Object"/> representing the raw value.</returns>
            public object ReadValue(ODataMessageReaderTestWrapper messageReader, IEdmTypeReference expectedValueTypeReference)
            {
                return messageReader.ReadValue(expectedValueTypeReference);
            }

            /// <summary>
            /// Read a collection as the message content.
            /// </summary>
            /// <param name="collectionReader">The collection reader to use for reading.</param>
            /// <returns>An <see cref="ODataCollectionStart"/>, possibly with annotations.</returns>
            public object ReadCollection(ODataCollectionReader collectionReader)
            {
                try
                {
                    // read the start of the collection
                    collectionReader.Read();
                    this.assert.AreEqual(ODataCollectionReaderState.CollectionStart, collectionReader.State, "Reader states don't match.");

                    // NOTE: collection names are only present in ATOM; for JSON the name will be 'null'
                    ODataCollectionStart collectionStart = (ODataCollectionStart)collectionReader.Item;

                    ODataCollectionItemsObjectModelAnnotation itemsAnnotation = new ODataCollectionItemsObjectModelAnnotation();
                    while (collectionReader.Read())
                    {
                        if (collectionReader.State == ODataCollectionReaderState.Value)
                        {
                            object item = collectionReader.Item;
                            itemsAnnotation.Add(item);
                        }
                        else
                        {
                            break;
                        }
                    }

                    this.assert.AreEqual(ODataCollectionReaderState.CollectionEnd, collectionReader.State, "Reader states don't match.");

                    collectionReader.Read();
                    this.assert.AreEqual(ODataCollectionReaderState.Completed, collectionReader.State, "Reader states don't match.");

                    // Exception is expected if try to read after completion.
                    this.assert.ThrowsException<ODataException>(() => { collectionReader.Read(); },
                            "Exception type ODataException is expected but is not.");

                    this.assert.AreEqual(ODataCollectionReaderState.Completed, collectionReader.State, "Reader should be in Completed state, but it is not.");

                    // Excpetion is expected if try to read in Exception state.
                    this.assert.ThrowsException<ODataException>(() => { collectionReader.Read(); },
                            "Exception type ODataException is expected but is not.");

                    this.assert.AreEqual(ODataCollectionReaderState.Completed, collectionReader.State, "Reader should be in Exception state, but it is not.");

                    // attach the items to the collection and return it
                    collectionStart.SetAnnotation(itemsAnnotation);

                    return collectionStart;
                }
                catch (Exception e)
                {
                    if (ExceptionUtilities.IsCatchable(e))
                    {
                        this.assert.AreEqual(ODataCollectionReaderState.Exception, collectionReader.State, "Expected the reader to be in 'Exception' state.");
                    }

                    throw;
                }
            }

            /// <summary>
            /// Read a collection as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="producingFunctionImport">The function import producing the collection to be read.</param>
            /// <param name="expectedItemTypeReference">The expected item type to pass to the reader.</param>
            /// <returns>An <see cref="ODataCollectionStart"/>, possibly with annotations.</returns>
            public object ReadCollection(ODataMessageReaderTestWrapper messageReader, IEdmOperationImport producingFunctionImport, IEdmTypeReference expectedItemTypeReference)
            {
                ODataCollectionReader collectionReader = producingFunctionImport != null
                    ? messageReader.CreateODataCollectionReader(producingFunctionImport)
                    : messageReader.CreateODataCollectionReader(expectedItemTypeReference);
                return this.ReadCollection(collectionReader);
            }

            /// <summary>
            /// Read parameters as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="functionImport">The function import to pass to the parameter reader.</param>
            /// <returns>A ComplexInstance representing parameters.</returns>
            public object ReadParameters(ODataMessageReaderTestWrapper messageReader, IEdmOperationImport functionImport)
            {
                // TODO: ODataLib test item: Add new ODataPayloadElement for parameters payload
                ODataParameterReader parameterReader = messageReader.CreateODataParameterReader(functionImport);
                ODataParameters odataParameters = new ODataParameters();
                List<ODataProperty> parameters = new List<ODataProperty>();
                try
                {
                    // read the start of the parameters
                    this.assert.AreEqual(ODataParameterReaderState.Start, parameterReader.State, "Reader states don't match.");
                    while (parameterReader.Read())
                    {
                        switch (parameterReader.State)
                        {
                            case ODataParameterReaderState.Value:
                                odataParameters.Add(new KeyValuePair<string, object>(parameterReader.Name, parameterReader.Value));
                                break;

                            case ODataParameterReaderState.Resource:
                                ODataReader entryReader = parameterReader.CreateResourceReader();
                                entryReader.Read();
                                this.assert.AreEqual(ODataReaderState.ResourceStart, entryReader.State, "Reader states don't match.");
                                odataParameters.Add(new KeyValuePair<string, object>(parameterReader.Name, this.ReadEntry(entryReader)));
                                this.assert.AreEqual(ODataReaderState.ResourceEnd, entryReader.State, "Reader states don't match.");
                                this.assert.IsFalse(entryReader.Read(), "Read() should return false after EntryEnd.");
                                this.assert.AreEqual(ODataReaderState.Completed, entryReader.State, "Reader states don't match.");
                                break;

                            case ODataParameterReaderState.ResourceSet:
                                ODataReader feedReader = parameterReader.CreateResourceSetReader();
                                feedReader.Read();
                                this.assert.AreEqual(ODataReaderState.ResourceSetStart, feedReader.State, "Reader states don't match.");
                                odataParameters.Add(new KeyValuePair<string, object>(parameterReader.Name, this.ReadFeed(feedReader)));
                                this.assert.AreEqual(ODataReaderState.ResourceSetEnd, feedReader.State, "Reader states don't match.");
                                this.assert.IsFalse(feedReader.Read(), "Read() should return false after EntryEnd.");
                                this.assert.AreEqual(ODataReaderState.Completed, feedReader.State, "Reader states don't match.");
                                break;

                            case ODataParameterReaderState.Collection:
                                ODataCollectionReader collectionReader = parameterReader.CreateCollectionReader();
                                odataParameters.Add(new KeyValuePair<string, object>(parameterReader.Name, this.ReadCollection(collectionReader)));
                                break;

                            default:
                                this.assert.Fail("Unexpected state: " + parameterReader.State);
                                break;
                        }
                    }

                    this.assert.AreEqual(ODataParameterReaderState.Completed, parameterReader.State, "Reader states don't match.");

                    return odataParameters;
                }
                catch (Exception e)
                {
                    if (ExceptionUtilities.IsCatchable(e))
                    {
                        this.assert.AreEqual(ODataParameterReaderState.Exception, parameterReader.State, "Expected the reader to be in 'Exception' state.");
                    }

                    throw;
                }
            }

            /// <summary>
            /// Read a service document as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <returns>An <see cref="ODataServiceDocument"/>, possibly with annotations.</returns>
            public object ReadServiceDocument(ODataMessageReaderTestWrapper messageReader)
            {
                return messageReader.ReadServiceDocument();
            }

            /// <summary>
            /// Read a metadata document as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <returns>An <see cref="IEdmModel"/>, possibly with annotations.</returns>
            public IEdmModel ReadMetadataDocument(ODataMessageReaderTestWrapper messageReader)
            {
                return messageReader.ReadMetadataDocument();
            }

            /// <summary>
            /// Read an error as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <returns>An <see cref="ODataError"/>, possibly with annotations.</returns>
            public ODataError ReadError(ODataMessageReaderTestWrapper messageReader)
            {
                return messageReader.ReadError();
            }

            /// <summary>
            /// Read a batch payload as the message content.
            /// </summary>
            /// <param name="messageReader">The message reader to use for reading.</param>
            /// <param name="expectedBatchPayload">The expected batch payload element; this is used to guide the payload kind detection of the parts.</param>
            /// <returns>An <see cref="ODataBatch"/>, possibly with annotations.</returns>
            public ODataBatch ReadBatch(ODataMessageReaderTestWrapper messageReader, ODataPayloadElement expectedBatchPayload)
            {
                this.assert.IsNotNull(expectedBatchPayload, "Must have a batch payload element.");
                this.assert.IsTrue(
                    expectedBatchPayload is BatchRequestPayload && this.testConfiguration.IsRequest || expectedBatchPayload is BatchResponsePayload && !this.testConfiguration.IsRequest,
                    "Expected a request or a response batch payload depending on what we are reading.");

                ODataBatchReaderTestWrapper batchReader = messageReader.CreateODataBatchReader();
                this.assert.AreEqual(ODataBatchReaderState.Initial, batchReader.State, "Reader states don't match.");

                IMimePart[] expectedParts = this.testConfiguration.IsRequest
                    ? ((BatchRequestPayload)expectedBatchPayload).Parts.ToArray()
                    : ((BatchResponsePayload)expectedBatchPayload).Parts.ToArray();
                int expectedPartIndex = 0;

                List<ODataBatchPart> batchParts = new List<ODataBatchPart>();
                List<ODataBatchOperation> changeSetOperations = null;
                bool errorReadingPartPayload = false;
                try
                {
                    bool isCompleted = false;
                    while (batchReader.Read())
                    {
                        this.assert.IsFalse(isCompleted, "We should not be able to successfully read once we reached the 'Completed' state.");

                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Initial:
                                this.assert.Fail("We should never be in state 'Initial' after we started reading.");
                                break;

                            case ODataBatchReaderState.Operation:
                                IMimePart expectedPart = expectedParts[expectedPartIndex];
                                int indexInPart = changeSetOperations == null ? -1 : changeSetOperations.Count;

                                try
                                {
                                    ODataBatchOperation operation = this.testConfiguration.IsRequest
                                        ? this.GetRequestOperation(batchReader, expectedPart, indexInPart)
                                        : this.GetResponseOperation(batchReader, expectedPart, indexInPart);

                                    if (changeSetOperations == null)
                                    {
                                        batchParts.Add(operation);
                                        expectedPartIndex++;
                                    }
                                    else
                                    {
                                        changeSetOperations.Add(operation);
                                    }
                                }
                                catch (Exception)
                                {
                                    // NOTE: The batch reader will not enter exception state when an error is raised while
                                    //       reading a part payload. The batch reader might skip to the next part and continue reading.
                                    errorReadingPartPayload = true;
                                    throw;
                                }

                                break;

                            case ODataBatchReaderState.ChangesetStart:
                                this.assert.IsNull(changeSetOperations, "Must not have pending change set operations when detecting another changeset.");
                                changeSetOperations = new List<ODataBatchOperation>();
                                break;

                            case ODataBatchReaderState.ChangesetEnd:
                                ODataBatchChangeset changeset = new ODataBatchChangeset
                                {
                                    Operations = changeSetOperations,
                                };
                                changeSetOperations = null;
                                batchParts.Add(changeset);
                                expectedPartIndex++;
                                break;

                            case ODataBatchReaderState.Completed:
                                isCompleted = true;
                                break;

                            case ODataBatchReaderState.Exception:
                                this.assert.Fail("We should never be in state 'Exception' without an exception being thrown.");
                                break;

                            default:
                                this.assert.Fail("Unsupported batch reader state '" + batchReader.State + "' detected.");
                                break;
                        }
                    }

                    this.assert.AreEqual(ODataBatchReaderState.Completed, batchReader.State, "Expected state 'Completed' when done reading.");
                }
                catch (Exception e)
                {
                    // NOTE: The batch reader will not enter exception state when an error is raised while
                    //       reading a part payload. The batch reader might skip to the next part and continue reading.
                    if (ExceptionUtilities.IsCatchable(e) && !errorReadingPartPayload)
                    {
                        this.assert.AreEqual(ODataBatchReaderState.Exception, batchReader.State, "Expected the reader to be in 'Exception' state.");
                    }

                    throw;
                }

                return new ODataBatch { Parts = batchParts };
            }

            /// <summary>
            /// Reads a batch operation request message from the batch reader.
            /// </summary>
            /// <param name="batchReader">The batch reader to read from.</param>
            /// <param name="expectedPart">The expected part representing the operation in the test OM; used to determine the expected payload kind.</param>
            /// <param name="indexInPart">The index of the operation in the current changeset or -1 for a top-level operation.</param>
            /// <returns>The <see cref="ODataBatchOperation"/> read from the batch reader.</returns>
            private ODataBatchOperation GetRequestOperation(ODataBatchReaderTestWrapper batchReader, IMimePart expectedPart, int indexInPart)
            {
                IMimePart requestPart = indexInPart < 0
                    ? ((MimePartData<IHttpRequest>)expectedPart).Body
                    : ((BatchRequestChangeset)expectedPart).Operations.ElementAt(indexInPart);

                ODataPayloadElement expectedPartElement = null;
                ODataRequest odataRequest = requestPart as ODataRequest;
                if (odataRequest != null)
                {
                    expectedPartElement = odataRequest.Body == null ? null : odataRequest.Body.RootElement;
                }

                ODataBatchOperationRequestMessage requestMessage = batchReader.CreateOperationRequestMessage();
                object partPayload = null;

                if (expectedPartElement != null)
                {
                    ODataPayloadKind expectedPartKind = expectedPartElement.GetPayloadKindFromPayloadElement();
                    ODataMessageReaderSettings messageReaderSettings = this.testConfiguration.MessageReaderSettings.Clone();
                    messageReaderSettings.EnableMessageStreamDisposal = true;

                    using (ODataMessageReader partMessageReader = new ODataMessageReader(requestMessage, messageReaderSettings, this.payloadModel))
                    {
                        ODataMessageReaderTestWrapper partMessageReaderWrapper =
                            new ODataMessageReaderTestWrapper(
                                partMessageReader,
                                messageReaderSettings,
                                this.testConfiguration);
                        partPayload = this.messageToObjectModelReader.ReadMessage(
                            partMessageReaderWrapper,
                            expectedPartKind,
                            this.payloadModel,
                            PayloadReaderTestDescriptor.ReaderMetadata.None,
                            /*batchPayload*/ null,
                            this.testConfiguration);
                    }
                }

                return new ODataBatchRequestOperation
                {
                    HttpMethod = requestMessage.Method,
                    Headers = requestMessage.Headers,
                    Url = requestMessage.Url,
                    Payload = partPayload
                };
            }

            /// <summary>
            /// Reads a batch operation response message from the batch reader.
            /// </summary>
            /// <param name="batchReader">The batch reader to read from.</param>
            /// <param name="expectedPart">The expected part representing the operation in the test OM; used to determine the expected payload kind.</param>
            /// <param name="indexInPart">The index of the operation in the current changeset or -1 for a top-level operation.</param>
            /// <returns>The <see cref="ODataBatchOperation"/> read from the batch reader.</returns>
            private ODataBatchOperation GetResponseOperation(ODataBatchReaderTestWrapper batchReader, IMimePart expectedPart, int indexInPart)
            {
                IMimePart responsePart = indexInPart < 0
                    ? ((MimePartData<HttpResponseData>)expectedPart).Body
                    : ((BatchResponseChangeset)expectedPart).Operations.ElementAt(indexInPart);

                ODataPayloadElement expectedPartElement = null;
                ODataResponse odataResponse = responsePart as ODataResponse;
                if (odataResponse != null)
                {
                    expectedPartElement = odataResponse.Body == null ? null : odataResponse.RootElement;
                }

                object partPayload = null;
                ODataBatchOperationResponseMessage responseMessage = batchReader.CreateOperationResponseMessage();
                if (expectedPartElement != null)
                {
                    ODataPayloadKind expectedPartKind = expectedPartElement.GetPayloadKindFromPayloadElement();
                    ODataMessageReaderSettings messageReaderSettings = this.testConfiguration.MessageReaderSettings.Clone();
                    messageReaderSettings.EnableMessageStreamDisposal = true;

                    using (ODataMessageReader partMessageReader = new ODataMessageReader(responseMessage, messageReaderSettings, this.payloadModel))
                    {
                        ODataMessageReaderTestWrapper partMessageReaderWrapper =
                            new ODataMessageReaderTestWrapper(
                                partMessageReader,
                                messageReaderSettings,
                                this.testConfiguration);
                        partPayload = this.messageToObjectModelReader.ReadMessage(
                            partMessageReaderWrapper,
                            expectedPartKind,
                            this.payloadModel,
                            PayloadReaderTestDescriptor.ReaderMetadata.None,
                            /*batchPayload*/ null,
                            this.testConfiguration);
                    }
                }

                return new ODataBatchResponseOperation
                {
                    StatusCode = responseMessage.StatusCode,
                    Headers = responseMessage.Headers,
                    Payload = partPayload,
                };
            }

            /// <summary>
            /// Read a feed with an <see cref="ODataReader"/> positioned on the feed start.
            /// </summary>
            /// <param name="reader">The <see cref="ODataReader"/> to use for reading the feed.</param>
            /// <returns>An <see cref="ODataResourceSet"/>, possibly with annotations.</returns>
            private ODataResourceSet ReadFeed(ODataReader reader)
            {
                this.assert.AreEqual(ODataReaderState.ResourceSetStart, reader.State, "Reader states don't match.");

                ODataResourceSet resourceCollection = (ODataResourceSet)reader.Item;
                ODataFeedPayloadOrderObjectModelAnnotation payloadOrderItems = this.storePayloadOrder ? new ODataFeedPayloadOrderObjectModelAnnotation() : null;
                ODataFeedEntriesObjectModelAnnotation feedEntries = new ODataFeedEntriesObjectModelAnnotation();

                AddFeedPayloadOrderItems(resourceCollection, payloadOrderItems);
                if (payloadOrderItems != null)
                {
                    payloadOrderItems.AddStartFeed();
                }

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceStart)
                    {
                        AddFeedPayloadOrderItems(resourceCollection, payloadOrderItems);

                        ODataResource entry = this.ReadEntry(reader);
                        if (payloadOrderItems != null)
                        {
                            payloadOrderItems.AddEntry(entry);
                        }

                        feedEntries.Add(entry);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        // we are done reading the resourceCollection
                        break;
                    }
                    else
                    {
                        this.assert.Fail("Unexpected reader state '" + reader.State + "' for reading a resourceCollection.");
                    }
                }

                this.assert.AreEqual(ODataReaderState.ResourceSetEnd, reader.State, "Reader states don't match.");
                ODataResourceSet feedAtEnd = (ODataResourceSet)reader.Item;
                this.assert.AreSame(resourceCollection, feedAtEnd, "Expected the same resourceCollection instance.");

                AddFeedPayloadOrderItems(resourceCollection, payloadOrderItems);

                resourceCollection.SetAnnotation(feedEntries);
                if (this.storePayloadOrder)
                {
                    resourceCollection.SetAnnotation(payloadOrderItems);
                }

                return resourceCollection;
            }

            /// <summary>
            /// Read an entry with an <see cref="ODataReader"/> positioned on the entry start.
            /// </summary>
            /// <param name="reader">The <see cref="ODataReader"/> to use for reading the entry.</param>
            /// <returns>An <see cref="ODataResource"/>, possibly with annotations.</returns>
            private ODataResource ReadEntry(ODataReader reader)
            {
                this.assert.AreEqual(ODataReaderState.ResourceStart, reader.State, "Reader states don't match.");

                ODataResource entry = (ODataResource)reader.Item;
                ODataEntryPayloadOrderObjectModelAnnotation payloadOrderItems = this.storePayloadOrder ? new ODataEntryPayloadOrderObjectModelAnnotation() : null;
                ODataEntryNavigationLinksObjectModelAnnotation navigationLinks = new ODataEntryNavigationLinksObjectModelAnnotation();

                AddEntryPayloadOrderItems(entry, payloadOrderItems);
                if (payloadOrderItems != null)
                {
                    payloadOrderItems.AddStartResource();
                }

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.NestedResourceInfoStart)
                    {
                        int positionInProperties = entry.Properties.Count();

                        AddEntryPayloadOrderItems(entry, payloadOrderItems);

                        ODataNestedResourceInfo navigationLinkAtStart = (ODataNestedResourceInfo)reader.Item;
                        if (payloadOrderItems != null)
                        {
                            payloadOrderItems.AddODataNavigationLink(navigationLinkAtStart);
                        }

                        ODataNestedResourceInfo navigationLink = this.ReadNavigationLink(reader);
                        this.assert.AreSame(navigationLinkAtStart, navigationLink, "Expected the same navigation link instance.");
                        navigationLinks.Add(navigationLink, positionInProperties + navigationLinks.Count);
                    }
                    else if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        // we are done reading this entry
                        break;
                    }
                    else
                    {
                        this.assert.Fail("Unexpected reader state '" + reader.State + "' for reading an entry.");
                    }
                }

                this.assert.AreEqual(ODataReaderState.ResourceEnd, reader.State, "Reader states don't match.");
                ODataResource entryAtEnd = (ODataResource)reader.Item;
                this.assert.AreSame(entry, entryAtEnd, "Expected the same entry instance.");

                AddEntryPayloadOrderItems(entry, payloadOrderItems);

                entry.SetAnnotation(navigationLinks);
                if (this.storePayloadOrder)
                {
                    entry.SetAnnotation(payloadOrderItems);
                }

                return entry;
            }

            /// <summary>
            /// Read a navigation link with an <see cref="ODataReader"/> positioned on the navigation link start.
            /// </summary>
            /// <param name="reader">The <see cref="ODataReader"/> to use for reading the navigation link.</param>
            /// <returns>An <see cref="ODataNestedResourceInfo"/>, possibly with annotations.</returns>
            /// <returns></returns>
            private ODataNestedResourceInfo ReadNavigationLink(ODataReader reader)
            {
                this.assert.AreEqual(ODataReaderState.NestedResourceInfoStart, reader.State, "Reader states don't match.");

                reader.Read();

                List<ODataItem> expandedItems = new List<ODataItem>();
                while (reader.State != ODataReaderState.NestedResourceInfoEnd)
                {
                    if (reader.State == ODataReaderState.ResourceStart)
                    {
                        if (reader.Item == null)
                        {
                            expandedItems.Add(null);

                            // Read over the entry start
                            reader.Read();
                        }
                        else
                        {
                            expandedItems.Add(this.ReadEntry(reader));
                        }

                        this.assert.AreEqual(ODataReaderState.ResourceEnd, reader.State, "Reader states don't match.");
                    }
                    else if (reader.State == ODataReaderState.ResourceSetStart)
                    {
                        expandedItems.Add(this.ReadFeed(reader));
                        this.assert.AreEqual(ODataReaderState.ResourceSetEnd, reader.State, "Reader states don't match.");
                    }
                    else if (reader.State == ODataReaderState.EntityReferenceLink)
                    {
                        this.assert.IsTrue(this.testConfiguration.IsRequest, "EntityReferenceLink state should only be returned in requests.");
                        ODataEntityReferenceLink entityReferenceLink = (ODataEntityReferenceLink)reader.Item;
                        this.assert.IsNotNull(entityReferenceLink, "Reader should never report null entity reference link.");
                        expandedItems.Add(entityReferenceLink);
                    }
                    else
                    {
                        this.assert.Fail("Unexpected reader state '" + reader.State + "' for reading a navigation link.");
                    }

                    reader.Read();
                }

                this.assert.AreEqual(ODataReaderState.NestedResourceInfoEnd, reader.State, "Reader states don't match.");

                ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo)reader.Item;

                ODataNavigationLinkExpandedItemObjectModelAnnotation expandedLinkAnnotation = null;
                if (this.testConfiguration.IsRequest)
                {
                    this.assert.IsTrue(expandedItems.Count > 0, "Deferred link (navigation link without any content) should only be returned in responses.");
                    expandedLinkAnnotation = expandedItems.Count > 1
                        ? new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = expandedItems }
                        : new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = expandedItems[0] };
                }
                else
                {
                    this.assert.IsTrue(expandedItems.Count <= 1, "In response each navigation link may contain at most one item in its content (entry or feed).");
                    if (expandedItems.Count == 1)
                    {
                        expandedLinkAnnotation = new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = expandedItems[0] };
                    }
                }

                if (expandedLinkAnnotation != null)
                {
                    navigationLink.SetAnnotation(expandedLinkAnnotation);
                }

                return navigationLink;
            }

            /// <summary>
            /// Adds all properties found on an entry to the payload order items.
            /// </summary>
            /// <param name="entry">The entry to inspect.</param>
            /// <param name="payloadOrderItems">The payload order items to add to, or null, if nothing should be done.</param>
            private static void AddEntryPayloadOrderItems(ODataResource entry, ODataEntryPayloadOrderObjectModelAnnotation payloadOrderItems)
            {
                if (payloadOrderItems == null) return;

                if (entry.Id != null) payloadOrderItems.AddEntryProperty("Id");
                if (entry.TypeName != null) payloadOrderItems.AddEntryProperty("TypeName");
                if (entry.EditLink != null) payloadOrderItems.AddEntryProperty("EditLink");
                if (entry.ReadLink != null) payloadOrderItems.AddEntryProperty("ReadLink");
                if (entry.ETag != null) payloadOrderItems.AddEntryProperty("ETag");
                if (entry.MediaResource != null)
                {
                    payloadOrderItems.AddEntryProperty("MediaResource");
                    AddStreamReferenceValuePayloadOrderItems("MediaResource_", entry.MediaResource, payloadOrderItems);
                }

                if (entry.Actions != null)
                {
                    foreach (ODataAction action in entry.Actions)
                    {
                        payloadOrderItems.AddAction(action);
                    }
                }

                if (entry.Functions != null)
                {
                    foreach (ODataFunction function in entry.Functions)
                    {
                        payloadOrderItems.AddFunction(function);
                    }
                }

                if (entry.Properties != null)
                {
                    foreach (ODataProperty property in entry.Properties)
                    {
                        payloadOrderItems.AddODataProperty(property);
                        ODataStreamReferenceValue streamPropertyValue = property.Value as ODataStreamReferenceValue;
                        if (streamPropertyValue != null)
                        {
                            AddStreamReferenceValuePayloadOrderItems(property.Name + "_", streamPropertyValue, payloadOrderItems);
                        }
                    }
                }
            }

            /// <summary>
            /// Adds all properties found on a feed to the payload order items.
            /// </summary>
            /// <param name="entry">The feed to inspect.</param>
            /// <param name="payloadOrderItems">The payload order items to add to, or null, if nothing should be done.</param>
            private static void AddFeedPayloadOrderItems(ODataResourceSet feed, ODataFeedPayloadOrderObjectModelAnnotation payloadOrderItems)
            {
                if (payloadOrderItems == null) return;

                if (feed.Id != null) payloadOrderItems.AddFeedProperty("Id");
                if (feed.Count != null) payloadOrderItems.AddFeedProperty("Count");
                if (feed.NextPageLink != null) payloadOrderItems.AddFeedProperty("NextPageLink");
            }

            /// <summary>
            /// Adds all properties found on a stream reference value to the payload order items.
            /// </summary>
            /// <param name="prefix">The prefix to add to all property names found.</param>
            /// <param name="streamReferenceValue">The stream reference value to inspect.</param>
            /// <param name="payloadOrderItems">The payload order items to add to, or null, if nothing should be done.</param>
            private static void AddStreamReferenceValuePayloadOrderItems(string prefix, ODataStreamReferenceValue streamReferenceValue, ODataEntryPayloadOrderObjectModelAnnotation payloadOrderItems)
            {
                if (payloadOrderItems == null) return;
                if (streamReferenceValue.ReadLink != null) payloadOrderItems.AddEntryProperty(prefix + "ReadLink");
                if (streamReferenceValue.ContentType != null) payloadOrderItems.AddEntryProperty(prefix + "ContentType");
                if (streamReferenceValue.EditLink != null) payloadOrderItems.AddEntryProperty(prefix + "EditLink");
                if (streamReferenceValue.ETag != null) payloadOrderItems.AddEntryProperty(prefix + "ETag");
            }
        }
    }
}
