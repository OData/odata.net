//---------------------------------------------------------------------
// <copyright file="JsonBatchRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using Xunit;
    using Microsoft.OData.Edm;

    public class JsonBatchRoundTripTests
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        public JsonBatchRoundTripTests()
        {
            this.userModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            this.webType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.userModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);
        }

        [Fact]
        public void BatchJsonLightAtomicGroupIdAndTopLevelDataModificationRequestTest()
        {
            string atomicGroupIdFromRequest;
            byte[] requestPayload = this.CreateBatchRequestWithAtomicGroup(out atomicGroupIdFromRequest);

            byte[] responsePayload = this.ServiceReadBatchRequestAndWriteResponse(requestPayload);

            this.ClientReadBatchResponse(responsePayload, atomicGroupIdFromRequest, null);
        }

        [Fact]
        public void BatchJsonLightMultipleAtomicGroupsIdTest()
        {
            string atomicGroupIdFromRequest, atomicGroupAIdFromRequest;
            byte[] requestPayload = this.CreateBatchRequestWithMultipleAtomicGroups(
                out atomicGroupIdFromRequest,
                out atomicGroupAIdFromRequest);

            byte[] responsePayload = this.ServiceReadBatchRequestAndWriteResponse(requestPayload);

            this.ClientReadBatchResponse(responsePayload,
                atomicGroupIdFromRequest,
                atomicGroupAIdFromRequest);
        }

        [Fact]
        public void BatchJsonLightAtomicGroupCannotCreateGroupIdWithNullValue()
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                ArgumentNullException ane = Assert.Throws<ArgumentNullException>(() => batchWriter.WriteStartChangeset(null));
                Assert.True(ane.Message.Contains("changesetId"));
            }
        }

        /// <summary>
        /// Create a batch request that contains one atomic groups.
        /// </summary>
        /// <param name="atomicGroupIdFromRequest">Output string for the atomic group id from the batch created.</param>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateBatchRequestWithAtomicGroup(out string atomicGroupIdFromRequest)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                batchWriter.WriteStartChangeset();

                // Create operation.
                ODataBatchOperationRequestMessage createOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PUT",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                    "1");

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(createOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[]
                        {
                            new ODataProperty() { Name = "WebId", Value = 10 },
                            new ODataProperty() { Name = "Name", Value = "SingletonWebForBatchJsonLightAtomicGroupIdTest" }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                // A PATCH operation that depends on the preceding PUT operation.
                List<string> dependsOnIds = new List<string> { "1" };

                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "$1")),
                    "2",
                    BatchPayloadUriOption.AbsoluteUri,
                    dependsOnIds);

                // Verify that input values are copied into a new list.
                Assert.Equal(dependsOnIds, updateOperationMessage.DependsOnIds);
                Assert.NotSame(dependsOnIds, updateOperationMessage.DependsOnIds);

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 11 } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                Assert.Equal(createOperationMessage.GroupId, updateOperationMessage.GroupId);
                batchWriter.WriteEndChangeset();

                ODataBatchOperationRequestMessage update2OperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                    "3",
                    BatchPayloadUriOption.AbsoluteUri);

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(update2OperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 12 } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }
                batchWriter.WriteEndBatch();
                atomicGroupIdFromRequest = createOperationMessage.GroupId;

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadBatchRequestAndWriteResponse(byte[] requestPayload)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, settings, this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
                ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage);
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            ODataBatchOperationRequestMessage operationMessage = batchReader.CreateOperationRequestMessage();

                            ODataBatchOperationResponseMessage response =
                                batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);

                            if (operationMessage.Method == "PUT")
                            {
                                response.StatusCode = 201;
                                response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "PATCH")
                            {
                                response.StatusCode = 204;
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", "application/json;");
                                ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
                                writerSettings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (var operationMessageWriter = new ODataMessageWriter(response, writerSettings, this.userModel))
                                {
                                    var entryWriter = operationMessageWriter.CreateODataResourceWriter(this.singleton, this.webType);
                                    var entry = new ODataResource()
                                    {
                                        TypeName = "NS.Web",
                                        Properties = new[]
                                        {
                                            new ODataProperty() { Name = "WebId", Value = 11 },
                                            new ODataProperty() { Name = "Name", Value = "SingletonWebForBatchJsonLightAtomicGroupIdTest" }
                                        }
                                    };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }

                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            // Set the group Id on the writer side to correlate with request.
                            string atomicGroupId = batchReader.CurrentGroupId;
                            batchWriter.WriteStartChangeset(atomicGroupId);
                            break;
                        case ODataBatchReaderState.ChangesetEnd:
                            batchWriter.WriteEndChangeset();
                            break;
                    }
                }

                batchWriter.WriteEndBatch();
                responseStream.Position = 0;
                return responseStream.ToArray();
            }
        }

        private void ClientReadBatchResponse(byte[] responsePayload, string expectedAtomicGroupId, string expectedAtomicGroupAId)
        {
            IODataResponseMessage responseMessage = new InMemoryMessage() { Stream = new MemoryStream(responsePayload) };
            responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            using (var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), this.userModel))
            {
                var batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            ODataBatchOperationResponseMessage operationMessage = batchReader.CreateOperationResponseMessage();
                            string responseId = operationMessage.ContentId;
                            if (responseId.Equals("1") || responseId.Equals("2"))
                            {
                                // Verify the group id of the responses is correlated to the group id from the data modification requests.
                                Assert.Equal(operationMessage.GroupId, expectedAtomicGroupId);
                                Assert.True(operationMessage.StatusCode == 201 || operationMessage.StatusCode == 204);
                            }
                            else if (responseId.Equals("1A") || responseId.Equals("2A"))
                            {
                                // Verify the group id of the responses is correlated to the group id from the data modification requests.
                                Assert.Equal(operationMessage.GroupId, expectedAtomicGroupAId);
                                Assert.True(operationMessage.StatusCode == 201 || operationMessage.StatusCode == 204);
                            }
                            else if (responseId.Equals("3")||responseId.Equals("3A"))
                            {
                                // Verify the group id of the query response.
                                Assert.Null(operationMessage.GroupId);
                            }
                            else
                            {
                                Assert.True(false, "Unexpected response id received: " + responseId);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Create a batch request that contains atomic groups.
        /// </summary>
        /// <param name="atomicGroupIdFromRequest">Output string for the first atomic group id from the batch created.</param>
        /// <param name="atomicGroupAIdFromRequest">Output string for the second atomic group id from the batch created.</param>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateBatchRequestWithMultipleAtomicGroups(
            out string atomicGroupIdFromRequest,
            out string atomicGroupAIdFromRequest)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                // Lexical scope for first half: query ("3"), create("1"), patch("2")
                {
                    // Query operation
                    ODataBatchOperationRequestMessage queryOperationMessage =
                        batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "MySingleton"),
                            "3");

                    queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                    batchWriter.WriteStartChangeset();

                    // Create operation.
                    ODataBatchOperationRequestMessage createOperationMessage = batchWriter.CreateOperationRequestMessage
                        (
                            "PUT",
                            new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                            "1");

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(createOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[]
                            {
                                new ODataProperty() {Name = "WebId", Value = 10},
                                new ODataProperty()
                                {
                                    Name = "Name",
                                    Value = "SingletonWebForBatchJsonLightAtomicGroupIdTest"
                                }
                            }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    // A PATCH operation that depends on the preceding PUT operation.
                    string[] dependsOnIds = new string[] {"1"};

                    ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage
                        (
                            "PATCH",
                            new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "$1")),
                            "2",
                            BatchPayloadUriOption.AbsoluteUri,
                            dependsOnIds);

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[] {new ODataProperty() {Name = "WebId", Value = 11}}
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    Assert.Equal(createOperationMessage.GroupId, updateOperationMessage.GroupId);
                    batchWriter.WriteEndChangeset();

                    atomicGroupIdFromRequest = createOperationMessage.GroupId;
                }

                // Lexical scope for second half: create("1A"), patch("2A"), query("3A")
                {
                    batchWriter.WriteStartChangeset();

                    // Create operation.
                    ODataBatchOperationRequestMessage createOperationMessage = batchWriter.CreateOperationRequestMessage
                        (
                            "PUT",
                            new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                            "1A");

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(createOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[]
                            {
                                new ODataProperty() {Name = "WebId", Value = 10},
                                new ODataProperty()
                                {
                                    Name = "Name",
                                    Value = "SingletonWebForBatchJsonLightAtomicGroupIdTest"
                                }
                            }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    // A PATCH operation that depends on the preceding PUT operation.
                    string[] dependsOnIds = new string[] { "1A" };

                    ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage
                        (
                            "PATCH",
                            new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "$1A")),
                            "2A",
                            BatchPayloadUriOption.AbsoluteUri,
                            dependsOnIds);

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[] { new ODataProperty() { Name = "WebId", Value = 11 } }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    Assert.Equal(createOperationMessage.GroupId, updateOperationMessage.GroupId);
                    batchWriter.WriteEndChangeset();

                    atomicGroupAIdFromRequest = createOperationMessage.GroupId;

                    // Query operation
                    ODataBatchOperationRequestMessage queryOperationMessage =
                        batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "MySingleton"),
                            "3A");

                    queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                }

                batchWriter.WriteEndBatch();

                Assert.NotEqual(atomicGroupIdFromRequest,
                                atomicGroupAIdFromRequest);

                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
