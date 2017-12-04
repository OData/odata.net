//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchAtomicityGroupTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Tests;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Xunit;

    public class ODataJsonBatchAtomicityGroupTests
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentTypeApplicationJson = "application/json";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel edmModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType userType;
        private readonly ODataMessageReaderSettings readerSettingsV401;
        private readonly ODataMessageWriterSettings writerSettingsV401;

        public ODataJsonBatchAtomicityGroupTests()
        {
            this.edmModel = new EdmModel();

            this.userType = new EdmEntityType("NS", "User");
            this.userType.AddStructuralProperty("UserPrincipalName", EdmPrimitiveTypeKind.String);
            this.userType.AddStructuralProperty("Surname", EdmPrimitiveTypeKind.String);
            this.userType.AddStructuralProperty("GivenName", EdmPrimitiveTypeKind.String);
            this.edmModel.AddElement(this.userType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.edmModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.userType);
            this.defaultContainer.AddElement(this.singleton);

            this.readerSettingsV401 = new ODataMessageReaderSettings();
            readerSettingsV401.MaxProtocolVersion = ODataVersion.V401;

            this.writerSettingsV401 = new ODataMessageWriterSettings();
            writerSettingsV401.Version = ODataVersion.V401;
        }

        [Fact]
        public void JsonBatchAtomicityGroupTestCases()
        {
            ODataJsonBatchPayloadAtomicGroupTestCase[] testCases =
            {
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Request depends on id of another request.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""GET"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""r1""
                            },{
                              ""id"": ""r2"",
                              ""dependsOn"": [""r1""],
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    ListOfDependsOnIds = new IList<string>[]
                    {
                        null,
                        new List<string>(){"r1"}
                    }
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Request depends on groupId of another atomic group and another requestId.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g1"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r1"",
                              ""body"": {""userPrincipalName"": ""mu1@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            },{
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g1"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r2"",
                              ""body"": {""userPrincipalName"": ""mu2@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            },{
                              ""id"": ""r2"",
                              ""dependsOn"": [""g1""],
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu5@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            },{
                              ""id"": ""g2r6"",
                              ""dependsOn"": [""g1"", ""r2""],
                              ""atomicityGroup"": ""g2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            },{
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g3"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=none; odata.streaming=false"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g3r1"",
                              ""body"": {""userPrincipalName"": ""mu7@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    ListOfDependsOnIds = new IList<string>[]
                    {
                        null,
                        null,
                        new List<string>(){"g1r1", "g1r2"},
                        new List<string>{"g1r1", "g1r2", "r2"},
                        null
                    }
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Bad Request: Request depends on requestId of request in another atomic group.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g1"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r1"",
                              ""body"": {""userPrincipalName"": ""mu1@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            },{
                              ""id"": ""g2r6"",
                              ""dependsOn"": [""g1r1""],
                              ""atomicityGroup"": ""g2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed("g1r1", "g1")
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Bad Request: Request depends on invalid requestId.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g1"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r1"",
                              ""body"": {""userPrincipalName"": ""mu1@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            },{
                              ""id"": ""g2r6"",
                              ""dependsOn"": [""invalidId""],
                              ""atomicityGroup"": ""g2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_DependsOnIdNotFound("invalidId", "g2r6")
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Bad Request: duplicate request Id.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""GET"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""duplicate""
                            },{
                              ""id"": ""duplicate"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_DuplicateContentIDsNotAllowed("duplicate")
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Bad Request: duplicate request Id and invalid dependsOn requestId.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""GET"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""duplicate""
                            },{
                              ""id"": ""duplicate"",
                              ""dependsOn"": [""invalidId""],
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_DependsOnIdNotFound("invalidId", "duplicate")
                },
                new ODataJsonBatchPayloadAtomicGroupTestCase
                {
                    Desciption =  "Bad Request: Request depends on id forward reference is not allowed.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""GET"",
                              ""dependsOn"": [""r2""],
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""r1""
                            },{
                              ""id"": ""r2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_DependsOnIdNotFound("r2", "r1")
                },
            };

            foreach (ODataJsonBatchPayloadAtomicGroupTestCase testCase in testCases)
            {
                try
                {
                    byte[] response = ServiceReadSingletonBatchRequestAndWriterBatchResponse(testCase);
                    Assert.True(testCase.ExceptionType == null,
                        "testCase should have been succeeded: " + testCase.Desciption);
                    Assert.True(response != null && response.Length > 0);
                }
                catch (Exception e)
                {
                    Assert.IsType(testCase.ExceptionType, e);
                    Assert.True(e.Message.Contains(testCase.TokenInExceptionMessage));
                }
            }
        }

        private byte[] ServiceReadSingletonBatchRequestAndWriterBatchResponse(ODataJsonBatchPayloadAtomicGroupTestCase testCase)
        {
            string requestPayload = testCase.RequestPayload;
            Action<ODataBatchOperationRequestMessage, IList<string>> requestOpMessageVerifier = testCase.RequestMessageDependsOnIdVerifier;

            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(Encoding.ASCII.GetBytes(requestPayload)) };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            MemoryStream responseStream = new MemoryStream();

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, readerSettingsV401.Clone(), this.edmModel))
            {

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

                ODataMessageWriterSettings settings = writerSettingsV401.Clone();
                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage, settings, null);
                int operationIdx = 0;

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

                            // Verify operation message if applicable.
                            requestOpMessageVerifier?.Invoke(operationMessage, testCase.ListOfDependsOnIds.ElementAt(operationIdx));

                            if (operationMessage.Method == "POST")
                            {
                                ODataBatchOperationResponseMessage response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 201;
                                response.SetHeader("Content-Type", batchContentTypeApplicationJson);
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                ODataBatchOperationResponseMessage response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", batchContentTypeApplicationJson);

                                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(response, settings, this.edmModel))
                                {
                                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter(this.singleton,
                                        this.userType);
                                    ODataResource entry = new ODataResource()
                                    {
                                        TypeName = "NS.User",
                                        Properties =
                                            new[]
                                            {
                                                new ODataProperty() {Name = "UserPrincipalName", Value = "foo@bar.com"},
                                                new ODataProperty() {Name = "GivenName", Value = "Jon"}
                                            }
                                    };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }

                            operationIdx++;
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
            }

            responseStream.Position = 0;
            return responseStream.ToArray();
        }
    }
}