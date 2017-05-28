//---------------------------------------------------------------------
// <copyright file="JsonBatchAtomicityGroupTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Xunit;

    public class ODataJsonBatchAtomicityGroupTests
    {
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string batchContentTypeApplicationJson = "application/json";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel edmModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType userType;

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
                              ""id"": ""r2"",
                              ""dependsOn"": [""g1""],
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
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
                            }
                          ]
                        }",
                    ListOfDependsOnIds = new IList<string>[]
                    {
                        null,
                        new List<string>(){"g1r1"},
                        new List<string>{"g1r1", "r2"}
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
                    //TODO: Would be best to get the string from resource file. For now, partial token serves the purpose.
                    TokenInExceptionMessage = "Therefore dependsOn property should refer to atomic group Id"
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
                    //TODO: Would be best to get the string from resource file. For now, partial token serves the purpose.
                    TokenInExceptionMessage = "is not matching any of the request Id and atomic group Id seen so far. Forward reference is not allowed."
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
                    RequestMessageDependsOnIdVerifier = null,
                    ExceptionType = typeof(ODataException),
                    //TODO: Would be best to get the string from resource file. For now, partial token serves the purpose.
                    TokenInExceptionMessage = "Content IDs have to be unique across all operations of a change set."
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
                    //TODO: Would be best to get the string from resource file. For now, partial token serves the purpose.
                    TokenInExceptionMessage = "is not matching any of the request Id and atomic group Id seen so far. Forward reference is not allowed."
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

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), this.edmModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

                var messageWriter = new ODataMessageWriter(responseMessage);
                int operationIdx = 0;

                try
                {
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

                                // Verify operation mesasge if applicable.
                                if (requestOpMessageVerifier != null)
                                {
                                    requestOpMessageVerifier(operationMessage, testCase.ListOfDependsOnIds.ElementAt(operationIdx));
                                }

                                if (operationMessage.Method == "POST")
                                {
                                    var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                    response.StatusCode = 201;
                                    response.SetHeader("Content-Type", batchContentTypeApplicationJson);
                                }
                                else if (operationMessage.Method == "GET")
                                {
                                    var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                    response.StatusCode = 200;
                                    response.SetHeader("Content-Type", batchContentTypeApplicationJson);
                                    var settings = new ODataMessageWriterSettings();
                                    settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                    using (
                                        var operationMessageWriter = new ODataMessageWriter(response, settings,
                                            this.edmModel))
                                    {
                                        var entryWriter = operationMessageWriter.CreateODataEntryWriter(this.singleton,
                                            this.userType);
                                        var entry = new ODataEntry()
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
                    responseStream.Position = 0;
                    return responseStream.ToArray();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }
}
