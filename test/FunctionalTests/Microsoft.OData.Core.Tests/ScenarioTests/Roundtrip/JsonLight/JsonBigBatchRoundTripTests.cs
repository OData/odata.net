//---------------------------------------------------------------------
// <copyright file="JsonBigBatchRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using Xunit;
    using Xunit.Abstractions;
    using Microsoft.OData.Edm;

    public class JsonBigBatchRoundTripTests
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        private readonly string aVeryLongString = null;

        private readonly ITestOutputHelper output;

        public JsonBigBatchRoundTripTests(ITestOutputHelper output)
        {
            this.output = output;

            this.userModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            this.webType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.userModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);

            int len = 1 << 16;
            StringBuilder sb = new StringBuilder(len);
            sb.Append('a', len);
            aVeryLongString = sb.ToString();
        }

        [Fact]
        public void BigBatchSingleRoundTripTest()
        {
            // Create one big batch request and perform round trip testing.
            DoOneRoundTrip();
        }

//        [Fact(Skip = "Runtime is 7 minutes, too long.")]
        public void BigBatchNCreationsTest()
        {
            // Create multiple (N) big batches.
            // Each iteration's memory foot print is about 700kB.
            // 64k iterations ==> >40GB in data stream * round trip
            // Memory usage should stay relatively flat.
            const int N = 1 << 16;
            for (int i = 0; i < N; i++)
            {
                PrintProgress(i);
                this.CreateBigBatchRequest(10);
            }
        }

//        [Fact(Skip = "Runtime is >60 minutes, too long.")]
        public void BigBatchNRoundTripsTest()
        {
            // Create multiple (N) big batches and perform round trip testing
            // Memory usage should stay relatively flat.
            const int N = 1 << 16;
            for (int i = 0; i < N; i++)
            {
                PrintProgress(i);
                DoOneRoundTrip();
            }
        }

        private void DoOneRoundTrip()
        {
            byte[] requestPayload = this.CreateBigBatchRequest(10);
            byte[] responsePayload = this.ServiceReadReferenceUriBatchRequestAndWriteResponse(requestPayload);
            this.ClientReadBatchResponse(responsePayload);
        }

        private void PrintProgress(int i)
        {
            if (i % 100 == 0)
            {
                this.output.WriteLine("iteration: " + i);
            }
        }

        private byte[] CreateBigBatchRequest(int repeatCount)
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

                // Each iteration generates a change set with two operations, followed by one top level operation.
                // Operations count in each iteration is three.
                for (int idx = 0; idx < repeatCount*3; idx += 3)
                {
                    batchWriter.WriteStartChangeset();

                    ODataBatchOperationRequestMessage createOperationMessage = batchWriter.CreateOperationRequestMessage(
                        "PUT",
                        new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                        idx.ToString());

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(createOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[]
                            {
                                new ODataProperty() { Name = "WebId", Value = "webid_" + idx },
                                new ODataProperty() { Name = "Name", Value = this.aVeryLongString }
                            }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    // A PATCH operation that depends on the preceding PUT operation.
                    List<string> dependsOnIds = new List<string> { idx.ToString() };

                    ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                        "PATCH",
                        new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "$" + idx)),
                        (idx+1).ToString(),
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
                            Properties = new[] { new ODataProperty() { Name = "WebId", Value = "webid_" + (idx+1) } }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    Assert.Equal(createOperationMessage.GroupId, updateOperationMessage.GroupId);
                    batchWriter.WriteEndChangeset();

                    ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                        "GET",
                        new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                        (idx+2).ToString());
                    queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                }

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadReferenceUriBatchRequestAndWriteResponse(byte[] requestPayload)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            byte[] responseBytes = null;

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, settings, this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

                using (ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage))
                {
                    ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                    batchWriter.WriteStartBatch();

                    ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                    while (batchReader.Read())
                    {
                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                // Encountered an operation (either top-level or in a change set)
                                ODataBatchOperationRequestMessage operationMessage =
                                    batchReader.CreateOperationRequestMessage();

                                ODataBatchOperationResponseMessage response =
                                    batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);

                                if (operationMessage.Method == "PUT")
                                {
                                    using (ODataMessageReader operationMessageReader = new ODataMessageReader(
                                        operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                    {
                                        ODataReader reader = operationMessageReader.CreateODataResourceReader();
                                        Assert.NotNull(reader);
                                    }

                                    response.StatusCode = 201;
                                    response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                                }
                                else if (operationMessage.Method == "PATCH")
                                {
                                    using (ODataMessageReader operationMessageReader = new ODataMessageReader(
                                        operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                    {
                                        ODataReader reader = operationMessageReader.CreateODataResourceReader();
                                        Assert.NotNull(reader);
                                    }

                                    response.StatusCode = 204;
                                }
                                else if (operationMessage.Method == "GET")
                                {
                                    response.StatusCode = 200;
                                    response.SetHeader("Content-Type", "application/json;");
                                    ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings();
                                    writerSettings.ODataUri.ServiceRoot = new Uri(serviceDocumentUri);
                                    using (
                                        ODataMessageWriter operationMessageWriter = new ODataMessageWriter(response,
                                            writerSettings, this.userModel))
                                    {
                                        ODataWriter entryWriter =
                                            operationMessageWriter.CreateODataResourceWriter(this.singleton,
                                                this.webType);
                                        ODataResource entry = new ODataResource()
                                        {
                                            TypeName = "NS.Web",
                                            Properties = new[]
                                            {
                                                new ODataProperty() {Name = "WebId", Value = -1},
                                                new ODataProperty() {Name = "Name", Value = aVeryLongString}
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
                    responseBytes =  responseStream.ToArray();
                }

                return responseBytes;
            }
        }

        private void ClientReadBatchResponse(byte[] responsePayload)
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
                            if (operationMessage.StatusCode == 200)
                            {
                                using (ODataMessageReader innerMessageReader = new ODataMessageReader(
                                    operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                {
                                    ODataReader reader = innerMessageReader.CreateODataResourceReader();

                                    while (reader.Read()) {}
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
