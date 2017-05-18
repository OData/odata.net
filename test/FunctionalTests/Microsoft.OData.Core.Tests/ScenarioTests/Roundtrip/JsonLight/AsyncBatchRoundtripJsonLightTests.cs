//---------------------------------------------------------------------
// <copyright file="AsyncBatchRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class AsyncBatchRoundtripJsonLightTests
    {
        private const string serviceDocumentUri = "http://service";
        private const string batchContentType = "multipart/mixed; boundary=batch_36522ad7-fc75-4b56-8c71-56071383e77b";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmEntityType customerType;
        private readonly EdmEntitySet customers;

        public AsyncBatchRoundtripJsonLightTests()
        {
            this.userModel = new EdmModel();

            this.customerType = new EdmEntityType("MyNS", "Customer");
            this.customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String);
            this.customerType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.userModel.AddElement(this.customerType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.customers = this.defaultContainer.AddEntitySet("Customers", customerType);
        }

        [Fact]
        public void AsyncBatchJsonLightTestFromSpecExample85()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUri);
            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload);
            this.ClientReadAsyncBatchResponse(responsePayload);
        }

        [Fact]
        public void AsyncBatchJsonLightWrtingAbsoluteResourcePathAndHostTest()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

#if NETCOREAPP1_0
            var payloadString = System.Text.Encoding.GetEncoding(0).GetString(requestPayload);
#else
            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);
#endif
            Assert.True(payloadString.Contains("GET /Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("POST /Customers HTTP/1.1") &&
                payloadString.Contains("PATCH /Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("GET /Products HTTP/1.1"));

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload);
            this.ClientReadAsyncBatchResponse(responsePayload);
        }

        [Fact]
        public void AsyncBatchJsonLightWrtingRelativeResourcePathTest()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.RelativeUri);

#if NETCOREAPP1_0
            var payloadString = System.Text.Encoding.GetEncoding(0).GetString(requestPayload);
#else
            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);
#endif
            Assert.True(payloadString.Contains("GET Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("POST Customers HTTP/1.1") &&
                payloadString.Contains("PATCH Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("GET Products HTTP/1.1"));

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload);
            this.ClientReadAsyncBatchResponse(responsePayload);
        }

        private byte[] ClientWriteAsyncBatchRequest(BatchPayloadUriOption payloadUriOption)
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a query operation.
                var queryOperationMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/Customers('ALFKI')"), /*contentId*/ null, payloadUriOption);

                // Write a changeset with multi update operation.
                batchWriter.WriteStartChangeset();

                // Create a creation operation in the changeset.
                var updateOperationMessage = batchWriter.CreateOperationRequestMessage("POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);

                // Use a new message writer to write the body of this operation.
                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Id", Value = "AFKIL" }, new ODataProperty() { Name = "Name", Value = "Bob" } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/Customers('ALFKI')"), "2", payloadUriOption);

                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Jack" } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Write a query operation.
                batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/Products"), /*contentId*/ null, payloadUriOption);

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadAsyncBatchRequestAndWriteAsyncResponse(byte[] requestPayload)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings { BaseUri = new Uri(serviceDocumentUri) }, this.userModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };
                responseMessage.SetHeader("Content-Type", batchContentType);
                var messageWriter = new ODataMessageWriter(responseMessage);
                var batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                var batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a changeset)
                            var operationMessage = batchReader.CreateOperationRequestMessage();
                            if (operationMessage.Method == "GET" && operationMessage.Url.AbsolutePath.Contains("ALFKI"))
                            {
                                var response = batchWriter.CreateOperationResponseMessage(null);
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", "application/json;");
                                var settings = new ODataMessageWriterSettings();
                                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (var operationMessageWriter = new ODataMessageWriter(response, settings, this.userModel))
                                {
                                    var entryWriter = operationMessageWriter.CreateODataResourceWriter(this.customers, this.customerType);
                                    var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Id", Value = "ALFKI" }, new ODataProperty() { Name = "Name", Value = "John" } } };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }
                            break;
                    }
                }

                var asyncResponse = batchWriter.CreateOperationResponseMessage(null);
                asyncResponse.StatusCode = 202;
                asyncResponse.SetHeader("Location", "http://service/async-monitor");
                asyncResponse.SetHeader("Retry-After", "10");

                batchWriter.WriteEndBatch();

                responseStream.Position = 0;
                return responseStream.ToArray();
            }
        }

        private void ClientReadAsyncBatchResponse(byte[] responsePayload)
        {
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
                            // Encountered an operation (either top-level or in a changeset)
                            var operationMessage = batchReader.CreateOperationResponseMessage();
                            if (operationMessage.StatusCode == 200)
                            {
                                using (ODataMessageReader innerMessageReader = new ODataMessageReader(operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                {
                                    var reader = innerMessageReader.CreateODataResourceReader();

                                    while (reader.Read())
                                    {
                                        if (reader.State == ODataReaderState.ResourceEnd)
                                        {
                                            ODataResource entry = reader.Item as ODataResource;
                                            Assert.Equal("ALFKI", entry.Properties.Single(p => p.Name == "Id").Value);
                                            Assert.Equal("John", entry.Properties.Single(p => p.Name == "Name").Value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Assert.Equal(202, operationMessage.StatusCode);
                            }
                            break;
                    }
                }
            }
        }
    }
}
