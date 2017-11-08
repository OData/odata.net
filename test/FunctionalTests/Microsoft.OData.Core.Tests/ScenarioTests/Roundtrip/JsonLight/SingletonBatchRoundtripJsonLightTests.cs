//---------------------------------------------------------------------
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
using Xunit.Sdk;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class BatchRoundtripJsonLightTests
    {
        private enum BatchFormat
        {
            MultipartMIME,
            ApplicationJson
        };

        private const string batchContentTypeMultipartMime = "multipart/mixed; boundary=batch_cb48b61f-511b-48e6-b00a-77c847badfb9";
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string selfReferenceUriNotAllowed = "contains self-reference of Content-ID value";
        private const string changesetContainingQueryNotAllowed =
            "was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', and 'PATCH'.";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        // GET and DELETE should containt extra empty line as content is empty, see RFC 2046 5.1.1
        private const string ExpectedRequestPayload1 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
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
Content-ID: 3

DELETE http://odata.org/test/MySingleton HTTP/1.1


--changeset_702fbcf5-653b-4217-bf4b-563aae4971fe--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedRequestPayload2 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9

--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PATCH http://odata.org/test//MySingleton1 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10,""Name"":""SingletonWeb""}
--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PATCH http://odata.org/test//MySingleton2 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50

--changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

PATCH http://odata.org/test//MySingleton3 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":30,""Name"":""SingletonWeb3""}
--changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton3/WebId HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayload1 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
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
Content-ID: 3

HTTP/1.1 500 Internal Server Error


--changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayload2 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238

--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1

--changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedRequestPayloadUsingJson1 = @"
{
    ""requests"": [{
            ""id"": ""a05368c8-479d-4409-a2ee-9b54b133ec38"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""Accept"": ""application/json;odata.metadata=full""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 10,
                ""Name"": ""SingletonWeb""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 111
            }
        }, {
            ""id"": ""adec0e52-7647-4d9d-baac-9316f9dc6927"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton/WebId"",
            ""headers"": {
                ""Accept"": ""application/json;odata.metadata=full""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""29873bf8-2d2c-478d-a7df-1e7b236faebf"",
            ""method"": ""DELETE"",
            ""url"": ""http://odata.org/test/MySingleton"",
            ""headers"": {}
        }
    ]
}";

        private const string ExpectedRequestPayloadUsingJson2 = @"
{
    ""requests"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""2ffeac98-237b-46a7-b97c-6a360d622aaa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton1"",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 10,
                ""Name"": ""SingletonWeb""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""2ffeac98-237b-46a7-b97c-6a360d622aaa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton2"",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 111
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""2ffabc98-257b-46a7-b17c-97650d6220aa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton3"",
            ""headers"": {
                ""OData-Version"": ""4.0"",
                ""Content-Type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 30,
                ""Name"": ""SingletonWeb3""
            }
        }, {
            ""id"": ""0d24a607-3fb0-4a83-a8fb-ef33f8c7e4ba"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton3/WebId"",
            ""headers"": {
                ""Accept"": ""application/json;odata.metadata=full""
            }
        }
    ]
}";

        private const string ExpectedResponsePayloadUsingJson1 = @"
{
    ""responses"": [{
            ""id"": ""cbcdb345-afdc-4125-8832-fcd7e14a013f"",
            ""status"": 200,
            ""headers"": {
                ""Content-Type"": ""application/json;"",
                ""OData-Version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 204,
            ""headers"": {
                ""Content-Type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 204,
            ""headers"": {
                ""Content-Type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""fd811ff7-4c67-4a2f-bbe1-60b606093f14"",
            ""status"": 200,
            ""headers"": {
                ""Content-Type"": ""application/json;"",
                ""OData-Version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""a2f8de08-ae49-4717-92c5-9dbc198acc71"",
            ""status"": 500,
            ""headers"": {}
        }
    ]
}";

        private const string ExpectedResponsePayloadUsingJson2 = @"
{
    ""responses"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""b749ce30-c81d-4a04-af0f-342da47218ec"",
            ""status"": 204,
            ""headers"": {
                ""Content-Type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""b749ce30-c81d-4a04-af0f-342da47218ec"",
            ""status"": 204,
            ""headers"": {
                ""Content-Type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""630855a9-c931-478f-a6d7-860bba586e5e"",
            ""status"": 204,
            ""headers"": {
                ""Content-Type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""81133a6f-c773-4add-963f-54b167adb747"",
            ""status"": 200,
            ""headers"": {
                ""Content-Type"": ""application/json;"",
                ""OData-Version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }
    ]
}";
        private readonly string[] ExpectedRequestPayloads =
        {
            ExpectedRequestPayload1,
            ExpectedRequestPayload2
        };

        private readonly string[] ExpectedRequestPayloadsJson =
        {
            ExpectedRequestPayloadUsingJson1,
            ExpectedRequestPayloadUsingJson2
        };

        private readonly string[] ExpectedResponsePayloads =
        {
            ExpectedResponsePayload1,
            ExpectedResponsePayload2
        };

        private readonly string[] ExpectedResponsePayloadsJson =
        {
            ExpectedResponsePayloadUsingJson1,
            ExpectedResponsePayloadUsingJson2
        };

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
            BatchJsonLightTestUsingBatchFormat(BatchFormat.MultipartMIME, 0);
        }

        [Fact]
        public void BatchJsonLightTestUsingJson()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.ApplicationJson, 0);
        }

        [Fact]
        public void BatchJsonLightTestChangesetsFollowedByQuery()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.MultipartMIME, 1);
        }

        [Fact]
        public void BatchJsonLightTestChangesetsFollowedByQueryUsingJson()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.ApplicationJson, 1);
        }

        [Fact]
        public void BatchJsonLightSelfReferenceUriTest()
        {
            BatchFormat[] formats = new BatchFormat[] {BatchFormat.MultipartMIME, BatchFormat.ApplicationJson};
            foreach (BatchFormat format in formats)
            {
                var requestPayload = this.CreateSelfReferenceBatchRequest(format);
                bool expectedExceptionThrown = false;
                try
                {
                    this.ServiceReadSingletonBatchRequestAndWriterBatchResponse(requestPayload,
                        GetContentTypeHeader(format));
                }
                catch (Exception e)
                {
                    expectedExceptionThrown = e.Message.Contains(selfReferenceUriNotAllowed);
                }
                Assert.True(expectedExceptionThrown, "Uri self-referencing with its Content-ID is not allowed.");
            }
        }

        [Fact]
        public void BatchJsonLightTestUsingJsonCanGenerateRequest_xyz()
        {
            byte[] requestPayload =
                CreateBatchRequestWithChangesetFirstAndQueryLast(GetContentTypeHeader(BatchFormat.ApplicationJson));
            VerifyPayload(requestPayload, BatchFormat.ApplicationJson, true, 1);
        }

        [Fact]
        public void BatchJsonLightQueryInsideChangesetTest()
        {
            BatchFormat[] formats = new BatchFormat[] { BatchFormat.MultipartMIME, BatchFormat.ApplicationJson };
            foreach (BatchFormat format in formats)
            {
                bool expectedExceptionThrown = false;
                try
                {
                    this.CreateQueryInsideChangesetBatchRequest(format);
                }
                catch (Exception e)
                {
                    expectedExceptionThrown = e.Message.Contains(changesetContainingQueryNotAllowed);
                }
                Assert.True(expectedExceptionThrown, "change set containing query operation is not allowed.");
            }
        }

        private void BatchJsonLightTestUsingBatchFormat(BatchFormat batchFormat, int idx)
        {
            byte[] requestPayload = null;
            switch (idx)
            {
                case 0:
                    requestPayload = ClientWriteSingletonBatchRequest(
                        GetContentTypeHeader(batchFormat));
                    break;

                case 1:
                    requestPayload = CreateBatchRequestWithChangesetFirstAndQueryLast(
                        GetContentTypeHeader(batchFormat));
                    break;

                default:
                    throw new ArgumentException("Unknown batch request index value", "idx");
            }

            VerifyPayload(requestPayload, batchFormat, true /*for request*/, idx);
            var responsePayload = this.ServiceReadSingletonBatchRequestAndWriterBatchResponse(requestPayload, GetContentTypeHeader(batchFormat));
            VerifyPayload(responsePayload, batchFormat, false /*for response*/, idx);
            this.ClientReadSingletonBatchResponse(responsePayload, GetContentTypeHeader(batchFormat));
        }

        private void VerifyPayload(byte[] payloadBytes, BatchFormat batchFormat, bool forRequest, int idx)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {

                string payload = sr.ReadToEnd();
                string expectedPayload = null;

                switch (batchFormat)
                {
                    case BatchFormat.MultipartMIME:
                        {
                            expectedPayload = Regex.Replace(
                                forRequest ? ExpectedRequestPayloads[idx] : ExpectedResponsePayloads[idx],
                                "changeset.*$", "changeset_GUID", RegexOptions.Multiline);

                            payload = Regex.Replace(payload, "changeset.*$", "changeset_GUID", RegexOptions.Multiline);
                        }
                        break;

                    case BatchFormat.ApplicationJson:
                        {
                            expectedPayload = GetNormalizedJsonMessage(forRequest ?
                                ExpectedRequestPayloadsJson[idx] : ExpectedResponsePayloadsJson[idx]);

                            payload = GetNormalizedJsonMessage(payload);
                        }
                        break;

                    default:
                        {
                            Assert.True(false, "Unknown BatchFormat value");
                        }
                        break;
                }
                Assert.Equal(expectedPayload, payload);
            }
        }

        private byte[] ClientWriteSingletonBatchRequest(string batchContentType)
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

                // Write a change set with multi update operation.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                var updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "1");

                // Use a new message writer to write the body of this operation.
                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 10 }, new ODataProperty() { Name = "Name", Value = "SingletonWeb" } }
                    };
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
                batchWriter.CreateOperationRequestMessage("DELETE", new Uri(serviceDocumentUri + "MySingleton"), "3");
                batchWriter.WriteEndChangeset();

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadSingletonBatchRequestAndWriterBatchResponse(byte[] requestPayload, string batchContentType)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), this.userModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
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
                            // Encountered an operation (either top-level or in a change set)
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
                                    var entry = new ODataResource()
                                    {
                                        TypeName = "NS.Web",
                                        Properties = new[]
                                        {
                                            new ODataProperty() { Name = "WebId", Value = 10 },
                                            new ODataProperty() { Name = "Name", Value = "WebSingleton" }
                                        }
                                    };
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

        private void ClientReadSingletonBatchResponse(byte[] responsePayload, string batchContentType)
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
                            // Encountered an operation (either top-level or in a change set)
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
        private byte[] CreateSelfReferenceBatchRequest(BatchFormat format)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(format));

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a change set with an operation with self-referencing uri.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set with uri referencing itself.
                string resourceSegment = "MySingleton";
                string contentId = "1";
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(String.Format("{0}/{1}/${2}", serviceDocumentUri, resourceSegment, contentId)),
                    contentId);

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource() { TypeName = "NS.Web" };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();
                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Create a batch request with two batches and one query operation.
        /// </summary>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateBatchRequestWithChangesetFirstAndQueryLast(string batchContentType)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // First item is part of a change set.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton1"), "1");

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 10 },
                            new ODataProperty() { Name = "Name",  Value = "SingletonWeb" }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton2"), "2");

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 111 } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Second change set starts.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton3"), "3");

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 30 },
                            new ODataProperty() { Name = "Name",  Value = "SingletonWeb3" }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }
                batchWriter.WriteEndChangeset();

                // Last item is a query operation.
                ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(serviceDocumentUri + "/MySingleton3/WebId"), /*contentId*/ null);

                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Create an invalid batch request with query operation inside a change set.
        /// </summary>
        /// <returns>Thrown exception.</returns>
        private byte[] CreateQueryInsideChangesetBatchRequest(BatchFormat format)
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(format));

            using (var messageWriter = new ODataMessageWriter(requestMessage))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                // An invalid batch that has a change set containing query request
                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                // Create a query operation in the change set with uri referencing itself.
                ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(serviceDocumentUri + "/MySingletonInvalidRequest/WebId"), /*contentId*/ null);
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                batchWriter.WriteEndChangeset();
                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private static string GetContentTypeHeader(BatchFormat batchFormat)
        {
            return batchFormat == BatchFormat.MultipartMIME
                ? batchContentTypeMultipartMime
                : batchContentTypeApplicationJson;
        }

        /// <summary>
        /// Normalize the json message by replacing GUIDs and removing white spaces from formatted input.
        /// </summary>
        /// <param name="jsonMessage">The json message to be normalized.</param>
        /// <returns>The normalized message.</returns>
        private string GetNormalizedJsonMessage(string jsonMessage)
        {
            const string myIdProperty = @"""id"":""my_id_guid""";
            const string myAtomicGroupProperty = @"""atomicityGroup"":""my_groupid_guid""";

            string result = Regex.Replace(jsonMessage, @"\s*", "", RegexOptions.Multiline);
            result = Regex.Replace(result, "\"id\":\"[^\"]*\"", myIdProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"atomicityGroup\":\"[^\"]*\"", myAtomicGroupProperty, RegexOptions.Multiline);

            return result;
        }
    }
}
