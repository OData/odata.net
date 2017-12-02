﻿//---------------------------------------------------------------------
// <copyright file="SingletonBatchRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class BatchRoundtripJsonLightTests
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentType = "multipart/mixed; boundary=batch_cb48b61f-511b-48e6-b00a-77c847badfb9";
        private const string selfReferenceUriNotAllowed = "contains self-reference of Content-ID value";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        // GET and DELETE should containt extra empty line as content is empty, see RFC 2046 5.1.1
        private const string ExpectedRequestPayload = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fd

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10,""Name"":""SingletonWeb""}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton/WebId HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fe

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fe
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

DELETE http://odata.org/test/MySingleton HTTP/1.1


--changeset_702fbcf5-653b-4217-bf4b-563aae4971fe--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayload = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea77
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 500 Internal Server Error


--changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        public BatchRoundtripJsonLightTests()
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
        public void BatchJsonLightTest()
        {
            var requestPayload = this.ClientWriteSingletonBatchRequest();
            VerifyPayload(ExpectedRequestPayload, requestPayload);
            var responsePayload = this.ServiceReadSingletonBatchRequestAndWriterBatchResponse(requestPayload);
            VerifyPayload(ExpectedResponsePayload, responsePayload);
            this.ClientReadSingletonBatchResponse(responsePayload);
        }

        [Fact]
        public void BatchJsonLightSelfReferenceUriTest()
        {
            var requestPayload = this.CreateSelfReferenceBatchRequest();
            bool expectedExceptionThrown = false;
            try
            {
                this.ServiceReadSingletonBatchRequestAndWriterBatchResponse(requestPayload);
            }
            catch (Exception e)
            {
                expectedExceptionThrown = e.Message.Contains(selfReferenceUriNotAllowed);
            }
            Assert.True(expectedExceptionThrown, "Uri self-referencing with its Content-ID is not allowed.");
        }

        private void VerifyPayload(string expectedPayload, byte[] payloadBytes)
        {
            Stream stream = null;
            try
            {
                stream = new MemoryStream(payloadBytes);
                using (var sr = new StreamReader(stream))
                {
                    string payload = sr.ReadToEnd();

                    expectedPayload = Regex.Replace(expectedPayload, "changeset.*$", "changeset_GUID", RegexOptions.Multiline);
                    payload = Regex.Replace(payload, "changeset.*$", "changeset_GUID", RegexOptions.Multiline);

                    Assert.Equal(expectedPayload, payload);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        private byte[] ClientWriteSingletonBatchRequest()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageWriter = new ODataMessageWriter(requestMessage))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a query operation.
                var queryOperationMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/MySingleton"), /*contentId*/ null);

                // Header modification on inner payload.
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                // Write a changeset with multi update operation.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the changeset.
                var updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "1");

                // Use a new message writer to write the body of this operation.
                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "NS.Web", Properties = new[] { new ODataProperty() { Name = "WebId", Value = 10 }, new ODataProperty() { Name = "Name", Value = "SingletonWeb" } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "2");

                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "NS.Web", Properties = new[] { new ODataProperty() { Name = "WebId", Value = 111 } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Write a query operation.
                queryOperationMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/MySingleton/WebId"), /*contentId*/ null);

                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                // DELETE singleton, invalid
                batchWriter.WriteStartChangeset();
                batchWriter.CreateOperationRequestMessage("DELETE", new Uri(serviceDocumentUri + "MySingleton"), "1");
                batchWriter.WriteEndChangeset();

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadSingletonBatchRequestAndWriterBatchResponse(byte[] requestPayload)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), this.userModel))
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
                            if (operationMessage.Url.ToString()
                                .Contains(String.Format("${0}", operationMessage.ContentId)))
                            {
                                throw new ODataException(String.Format("Uri {0} {2} {1}",
                                    operationMessage.Url.ToString(),
                                    operationMessage.ContentId,
                                    selfReferenceUriNotAllowed));
                            }

                            if (operationMessage.Method == "PATCH")
                            {
                                var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 204;
                                response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                var response = batchWriter.CreateOperationResponseMessage(null);
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", "application/json;");
                                var settings = new ODataMessageWriterSettings();
                                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (var operationMessageWriter = new ODataMessageWriter(response, settings, this.userModel))
                                {
                                    var entryWriter = operationMessageWriter.CreateODataResourceWriter(this.singleton, this.webType);
                                    var entry = new ODataResource() { TypeName = "NS.Web", Properties = new[] { new ODataProperty() { Name = "WebId", Value = 10 }, new ODataProperty() { Name = "Name", Value = "WebSingleton" } } };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }
                            else if (operationMessage.Method == "DELETE")
                            {
                                var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 500;
                            }

                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            batchWriter.WriteStartChangeset();
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

        private void ClientReadSingletonBatchResponse(byte[] responsePayload)
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
                                            Assert.Equal(10, entry.Properties.Single(p => p.Name == "WebId").Value);
                                            Assert.Equal("WebSingleton", entry.Properties.Single(p => p.Name == "Name").Value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Assert.True(204 == operationMessage.StatusCode || 500 == operationMessage.StatusCode);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Create an invalid batch request with operation containing self-referencing uri.
        /// </summary>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateSelfReferenceBatchRequest()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageWriter = new ODataMessageWriter(requestMessage))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a change set with an operation with self-referencing uri.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set with uri referencing itself.
                string resourceSegment = "MySingleton";
                string contentId = "1";
                var updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(String.Format("{0}/{1}/${2}", serviceDocumentUri, resourceSegment, contentId)),
                    contentId);

                // Use a new message writer to write the body of this operation.
                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();
                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
