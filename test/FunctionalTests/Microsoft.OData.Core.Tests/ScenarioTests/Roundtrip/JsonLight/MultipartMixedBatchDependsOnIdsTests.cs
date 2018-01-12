//---------------------------------------------------------------------
// <copyright file="MultipartMixedBatchDependsOnIdsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.MultipartMixed;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class MultipartMixedBatchDependsOnIdsTests
    {
        private const string batchContentTypeMultipartMime = "multipart/mixed; boundary=batch_cb48b61f-511b-48e6-b00a-77c847badfb9";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        private const string RequestPayloadVerifyDependsOnIdsTemplate = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PUT http://odata.org/test/MySingleton HTTP/1.1
Content-Type: application/json;odata.metadata=minimal;IEEE754Compatible=false;charset=utf-8
Accept: application/json;odata.metadata=full

{""@odata.type"":""#NS.Web"",""WebId"":1}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fd

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":9}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

PATCH __REF_URI_1__ HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":11}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

PATCH __REF_URI_2__ HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":3}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayloadVerifyDependsOnIds = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 201 Created
Content-Type: application/json;


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        public MultipartMixedBatchDependsOnIdsTests()
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
        public void MultipartBatchTestWithTopLevelDependsOnIdsV401()
        {
            string customizedValidRequest = Regex.Replace(RequestPayloadVerifyDependsOnIdsTemplate,
                "__REF_URI_1__",
                "$2B",
                RegexOptions.Multiline);

            customizedValidRequest = Regex.Replace(customizedValidRequest,
                "__REF_URI_2__",
                "$1",
                RegexOptions.Multiline);

            byte[] responsePayload = this.ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(
                customizedValidRequest, ODataVersion.V401);
            VerifyPayloadForMultipartBatch(responsePayload, ExpectedResponsePayloadVerifyDependsOnIds);

            ClientReadSingletonBatchResponse(responsePayload, batchContentTypeMultipartMime);
        }

        [Fact]
        public void MultipartBatchTestWithTopLevelDependsOnIdsV4()
        {
            string customizedValidRequest = Regex.Replace(RequestPayloadVerifyDependsOnIdsTemplate,
                "__REF_URI_1__",
                "$2B",
                RegexOptions.Multiline);

            customizedValidRequest = Regex.Replace(customizedValidRequest,
                "__REF_URI_2__",
                "$1",
                RegexOptions.Multiline);

            // For V4 MultipartMixed batch, top-level request "3"'s url "/$1"(referencing a preceding top-level request "1")
            // should trigger an exception because request reference scope is limited to change set in V4 implementation.
            ODataException ode = Assert.Throws<ODataException>(
                 () => this.ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(customizedValidRequest, ODataVersion.V4));
            Assert.True(ode.Message.Contains(
                 "When the relative URI is a reference to a content ID, the content ID does not exist in the current change set."));
        }

        private void VerifyPayloadForMultipartBatch(byte[] payloadBytes, string expectedPayload)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {
                string normalizedPayload = GetNormalizedMultipartMimeMessage(sr.ReadToEnd());
                string normalizedExpectedPayload = GetNormalizedMultipartMimeMessage(expectedPayload);

                Assert.Equal(normalizedExpectedPayload, normalizedPayload);
            }
        }

        private string GetNormalizedMultipartMimeMessage(string message)
        {
            string normalizedMessage = Regex.Replace(message, "changeset.*$", "changeset_GUID", RegexOptions.Multiline);
            normalizedMessage = Regex.Replace(normalizedMessage, "OData-Version: .*$", "OData-version: myODataVer",
                RegexOptions.Multiline);
            return normalizedMessage;
        }

        private byte[] ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(string requestPayload, ODataVersion maxVersion)
        {
            byte[] responseBytes = null;

            IODataRequestMessage requestMessage = new InMemoryMessage()
            {
                Stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestPayload))
            };

            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings { MaxProtocolVersion = maxVersion };

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, settings, this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);
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

                                // Verify DependsOnIds are set correctly
                                IEnumerable<string> dependsOnIds = operationMessage.DependsOnIds;
                                switch (operationMessage.ContentId)
                                {
                                    case "1":
                                    case "2A":
                                        Assert.True(dependsOnIds.Count() == 0);
                                        break;

                                    case "2B":
                                        Assert.True(dependsOnIds.SequenceEqual(new List<string> {"2A"}));
                                        break;

                                    case "2C":
                                        Assert.True(dependsOnIds.SequenceEqual(new List<string> {"2A", "2B"}));
                                        break;

                                    case "3":
                                        Assert.True(dependsOnIds.SequenceEqual(new List<string> {"1"}));
                                        break;

                                    default:
                                        break;
                                }

                                ODataBatchOperationResponseMessage response =
                                    batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                if (operationMessage.Method == "PATCH")
                                {
                                    response.StatusCode = 204;
                                    response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                                }
                                else if (operationMessage.Method == "PUT")
                                {
                                    response.StatusCode = 201;
                                    response.SetHeader("Content-Type", "application/json;");
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
                    responseBytes = responseStream.ToArray();
                }

                return responseBytes;
            }
        }

        private void ClientReadSingletonBatchResponse(byte[] responsePayload, string batchContentType)
        {
            IODataResponseMessage responseMessage = new InMemoryMessage() { Stream = new MemoryStream(responsePayload) };
            responseMessage.SetHeader("Content-Type", batchContentType);
            using (ODataMessageReader messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), this.userModel))
            {
                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            ODataBatchOperationResponseMessage operationMessage = batchReader.CreateOperationResponseMessage();
                            if (operationMessage.StatusCode == 200)
                            {
                                using (ODataMessageReader innerMessageReader = new ODataMessageReader(operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                {
                                    ODataReader reader = innerMessageReader.CreateODataResourceReader();

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

                                // The only two messages with HTTP-200 response codes are the two GET requests with content id value of null.
                                // Verify that: for multipart batch the content id of the response is matching that of the request;
                                // for Json batch the content id of the response is not null.
                                Assert.True(
                                        (batchReader is ODataJsonLightBatchReader && operationMessage.ContentId != null)
                                    || (batchReader is ODataMultipartMixedBatchReader && operationMessage.ContentId == null));
                            }
                            break;
                    }
                }
            }
        }
    }
}
