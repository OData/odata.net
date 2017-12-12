//---------------------------------------------------------------------
// <copyright file="BatchRoundTripPerfTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using global::Xunit.Abstractions;
    using Microsoft.Xunit.Performance;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Performance tests for Json & Multipart batch formats using big payload.
    /// Currently Json tests can only be run locally since Json batch is introduced in OData 7.4.
    /// To run Json tests locally:
    /// 1. Enable the annotations {Benchmark, MeasureGCAllocations} of the Json batch tests.
    /// 2. Run power shell command <code>PerformanceBuild.ps1 -Config Release</code> from the
    ///   repository's root directory.
    /// </summary>
    public class BatchRoundTripPerfTest
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private const string batchContentTypeMultipartMime = "multipart/mixed; boundary=batch_542504a5-2c3e-4b66-9c53-959f74a9ac73";

        // Number of units in a batch.
        private const int NumOfUnitsPerBatch = 10;

        // Each unit consists of one change set with two requests, followed by one top level request.
        private const int NumOfRequestPerUnit = 3;

        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        private readonly string aLongString;
        private readonly ITestOutputHelper output;

        private string currentBatchContentType = null;

        public BatchRoundTripPerfTest(ITestOutputHelper output)
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

            int len = 1 << 10;
            StringBuilder sb = new StringBuilder(len);
            sb.Append('a', len);
            this.aLongString = sb.ToString();
        }

//        [Benchmark]
//        [MeasureGCAllocations]
        public void JsonBatchNCreationsTest()
        {
            BatchNCreationsTest(batchContentTypeApplicationJson);
        }

//        [Benchmark]
//        [MeasureGCAllocations]
        public void JsonBatchNRoundTripsTest()
        {
            BatchNRoundTripsTest(batchContentTypeApplicationJson);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void MultipartBatchNCreationsTest()
        {
            BatchNCreationsTest(batchContentTypeMultipartMime);
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void MultipartBatchNRoundTripsTest()
        {
            BatchNRoundTripsTest(batchContentTypeMultipartMime);
        }

        private void BatchNCreationsTest(string batchContentType)
        {
            this.currentBatchContentType = batchContentType;

            try
            {
                // Create a batch which has memory foot print of about 10kB.
                // Repeat N times.
                const int N = 1 << 10;
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {

                        for (int i = 0; i < N; i++)
                        {
                            PrintProgress(i);
                            this.CreateBatchRequest(this.currentBatchContentType, NumOfUnitsPerBatch);
                        }
                    }
                }
            }
            finally
            {
                this.currentBatchContentType = null;
            }
        }

        private void BatchNRoundTripsTest(string batchContentType)
        {
            this.currentBatchContentType = batchContentType;

            try
            {
                const int N = 1<<10;
                foreach (var iteration in Benchmark.Iterations)
                {
                    using (iteration.StartMeasurement())
                    {
                        // Create a batch and exercise round trip.
                        // Repeat N times.
                        // Memory usage should stay relatively flat.
                        for (int i = 0; i < N; i++)
                        {
                            PrintProgress(i);
                            DoOneRoundTrip();
                        }
                    }
                }
            }
            finally
            {
                this.currentBatchContentType = null;
            }
        }

        private void DoOneRoundTrip()
        {
            byte[] requestPayload = this.CreateBatchRequest(this.currentBatchContentType, NumOfUnitsPerBatch);
            byte[] responsePayload = this.ServiceReadReferenceUriBatchRequestAndWriteResponse(
                this.currentBatchContentType, requestPayload);
            this.ClientReadBatchResponse(this.currentBatchContentType, responsePayload);
        }

        private void PrintProgress(int i)
        {
            if (i%100 == 0)
            {
                this.output.WriteLine("iteration: " + i);
            }
        }

        private byte[] CreateBatchRequest(string batchContentType, int unitRepeatCount)
        {
            Debug.Assert(!string.IsNullOrEmpty(batchContentType), "!string.IsNullOrEmpty(batchContentType)");

            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                // Each iteration generates a change set with two operations, followed by one top level operation.
                // Operations count in each iteration is three.
                // Use API available in OData 7.2
                for (int idx = 0; idx < unitRepeatCount * NumOfRequestPerUnit; idx += NumOfRequestPerUnit)
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
                                new ODataProperty() { Name = "Name", Value = this.aLongString }
                            }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    // A PATCH operation.
                    ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                        "PATCH",
                        new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                        (idx + 1).ToString(),
                        BatchPayloadUriOption.AbsoluteUri);

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[] { new ODataProperty() { Name = "WebId", Value = "webid_" + (idx + 1) } }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    batchWriter.WriteEndChangeset();

                    ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                        "GET",
                        new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "MySingleton")),
                        (idx + 2).ToString());
                    queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                }

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadReferenceUriBatchRequestAndWriteResponse(string batchContentType, byte[] requestPayload)
        {
            Debug.Assert(!string.IsNullOrEmpty(batchContentType), "!string.IsNullOrEmpty(batchContentType)");

            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentType);
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);

            byte[] responseBytes = null;

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, settings, this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentType);

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
                                                new ODataProperty() {Name = "Name", Value = aLongString}
                                            }
                                        };
                                        entryWriter.WriteStart(entry);
                                        entryWriter.WriteEnd();
                                    }
                                }

                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                // Set the group Id on the writer side to correlate with request.
                                // Use API available in OData 7.2. Don't need to have same group Id for this test.
                                batchWriter.WriteStartChangeset();
                                break;
                            case ODataBatchReaderState.ChangesetEnd:
                                batchWriter.WriteEndChangeset();
                                break;
                        }
                    }

                    batchWriter.WriteEndBatch();

                    responseStream.Position = 0;
                    responseBytes = responseStream.ToArray();
                }

                return responseBytes;
            }
        }

        private void ClientReadBatchResponse(string batchContentType, byte[] responsePayload)
        {
            Debug.Assert(!string.IsNullOrEmpty(batchContentType), "!string.IsNullOrEmpty(batchContentType)");

            IODataResponseMessage responseMessage = new InMemoryMessage() { Stream = new MemoryStream(responsePayload) };
            responseMessage.SetHeader("Content-Type", batchContentType);
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

                                    while (reader.Read()) { }
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
