//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchAtomicityGroupTests.cs" company="Microsoft">
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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Tests;
    using Microsoft.OData.Tests.JsonLight;

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
        private readonly string textualSampleString;
        private readonly byte[] binarySampleBytes;


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

            textualSampleString = "azAZ09~!@#$%^&*()_+{}|:\"';?/.,\\etc\r\n\b\t\f";

            // Generate array of bytes representing all byte values.
            int binaryBytesLength = 256;
            this.binarySampleBytes = new byte[binaryBytesLength];

            for (int i = 0; i < binaryBytesLength; i++)
            {
                this.binarySampleBytes[i] = (byte)(i & 0xff);
            }

            this.readerSettingsV401 = new ODataMessageReaderSettings();
            readerSettingsV401.MaxProtocolVersion = ODataVersion.V401;

            this.writerSettingsV401 = new ODataMessageWriterSettings();
            writerSettingsV401.Version = ODataVersion.V401;
        }

        [Fact]
        public void JsonBatchJsonContentTypeTest()
        {
            ODataJsonBatchPayloadTestCase testCase = new ODataJsonBatchPayloadTestCase
            {
                Description = "Batch request of Json body and headers in different order.",
                RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""id"": ""r1"",
                              ""body"": {""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""},
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              }
                            },
                            {
                              ""id"": ""r2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal""
                              },
                              ""body"": {""userPrincipalName"": ""mu6@odata.org""}
                            }
                          ]
                        }",
                RequestMessageDependsOnIdVerifier = null,
                ContentTypeVerifier =
                    (message, offset) => VerifyOperationRequestMessage(message, offset, new string[]{ "r1", "r2" }, new int[]{ 2, 1 })
            };

            ServiceProcessBatchRequest(testCase, ODataVersion.V4);
        }

        [Fact]
        public void JsonBatchTextualContentTypeTest()
        {
            ODataJsonBatchPayloadTestCase testCase = new ODataJsonBatchPayloadTestCase
            {
                Description = "Batch request of Textual body and headers in different order.",
                RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""body"": ""__ENCODED_TEXTUAL_CONTENT__"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""text/plain"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""r1""
                            },
                            {
                              ""id"": ""r2"",
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""text/*; odata.metadata=minimal""
                              },
                              ""body"": ""__ENCODED_TEXTUAL_CONTENT__""
                            }
                          ]
                        }",
                RequestMessageDependsOnIdVerifier = null,
                ContentTypeVerifier =
                    (message, offset) => VerifyOperationRequestMessage(message, offset, new string[] { "r1", "r2" }, new int[] { 2, 1 })
            };

            testCase.PopulateEncodedContent("__ENCODED_TEXTUAL_CONTENT__", JsonLightUtils.GetJsonEncodedString(textualSampleString));

            ServiceProcessBatchRequest(testCase, ODataVersion.V4);
        }

        [Fact]
        public void JsonBatchBinaryContentTypeTest()
        {
            ODataJsonBatchPayloadTestCase testCase = new ODataJsonBatchPayloadTestCase
            {
                Description = "Batch request of binary body and headers in different order, or content-type header is an empty string.",
                RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""body"": ""__ENCODED_BINARY_CONTENT__"",
                              ""headers"": {
                                ""Content-Type"": ""my-binary/*"",
                                ""OData-Version"": ""4.0""
                              },
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",

                              ""id"": ""r1""
                            },
                            {
                              ""id"": ""r2"",
                              ""headers"": {
                                ""Content-Type"": ""my-other-binary/*""
                              },
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""body"": ""__ENCODED_BINARY_CONTENT__""
                            },
                            {
                              ""id"": ""r3"",
                              ""headers"": {
                                ""Content-Type"": """"
                              },
                              ""method"": ""POST"",
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""body"": ""__ENCODED_BINARY_CONTENT__""
                            }
                          ]
                        }",
                RequestMessageDependsOnIdVerifier = null,
                ContentTypeVerifier = (message, offset) =>
                    VerifyOperationRequestMessage(
                        message,
                        offset,
                        new string[] { "r1", "r2", "r3" },
                        new int[] { 2, 1, 1 })
            };

            testCase.PopulateEncodedContent("__ENCODED_BINARY_CONTENT__", JsonLightUtils.GetBase64UrlEncodedString(this.binarySampleBytes));

            ServiceProcessBatchRequest(testCase, ODataVersion.V4);
        }

        [Fact]
        public void JsonBatchAtomicityGroupTestCases()
        {
            ODataJsonBatchPayloadTestCase[] testCases =
            {
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Request depends on id of another request.",
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
                        new List<string>(),
                        new List<string>(){"r1"}
                    }
                },
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Request depends on groupId of another atomic group and another requestId.",
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
                        new List<string>(),
                        new List<string>(),
                        new List<string>(){"g1r1", "g1r2"},
                        new List<string>{"g1r1", "g1r2", "r2"},
                        new List<string>()
                    }
                },
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: Request depends on requestId of request in another atomic group.",
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
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: Request depends on invalid requestId.",
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
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: duplicate request Id.",
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
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: duplicate request Id and invalid dependsOn requestId.",
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
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: Request depends on id forward reference is not allowed.",
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
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: Request depends on its own groupId.",
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
                              ""dependsOn"": [""g1"", ""g1r1""],
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r2"",
                              ""body"": {""userPrincipalName"": ""mu2@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    ListOfDependsOnIds = new IList<string>[]
                    {
                        new List<string>(),
                        new List<string>(){ "g1", "g1r1" }
                    },
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed("g1", "g1")
                },
                new ODataJsonBatchPayloadTestCase
                {
                    Description =  "Bad Request: Request depends on itself.",
                    RequestPayload = @"
                        {
                          ""requests"": [
                            {
                              ""method"": ""POST"",
                              ""atomicityGroup"": ""g1"",
                              ""dependsOn"": [""g1r1""],
                              ""url"": ""http://odata.org/test/Users HTTP/1.1"",
                              ""headers"": {
                                ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                                ""OData-Version"": ""4.0""
                              },
                              ""id"": ""g1r1"",
                              ""body"": {""userPrincipalName"": ""mu1@odata.org"", ""givenName"": ""Jon1"", ""surname"": ""Doe""}
                            }
                          ]
                        }",
                    ListOfDependsOnIds = new IList<string>[]
                    {
                        new List<string>(){ "g1r1" }
                    },
                    ExceptionType = typeof(ODataException),
                    TokenInExceptionMessage = Strings.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed("g1r1", "g1r1")
                },
            };

            foreach (ODataJsonBatchPayloadTestCase testCase in testCases)
            {
                try
                {
                    byte[] response = ServiceReadSingletonBatchRequestAndWriterBatchResponse(testCase, ODataVersion.V401);
                    Assert.True(testCase.ExceptionType == null,
                        "testCase should have been succeeded: " + testCase.Description);
                    Assert.True(response != null && response.Length > 0);
                }
                catch (Exception e)
                {
                    Assert.IsType(testCase.ExceptionType, e);
                    Assert.True(e.Message.Contains(testCase.TokenInExceptionMessage));
                }
            }
        }

        private void ServiceProcessBatchRequest(ODataJsonBatchPayloadTestCase testCase, ODataVersion version)
        {
            string requestPayload = testCase.RequestPayload;
            ODataJsonBatchPayloadTestCase.ValidateContentType requestMessageContentTypeVerifier = testCase.ContentTypeVerifier;

            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(Encoding.ASCII.GetBytes(requestPayload)) };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (ODataMessageReader messageReader =
                new ODataMessageReader(requestMessage, new ODataMessageReaderSettings() { MaxProtocolVersion = version }, this.edmModel))
            {
                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();

                int operationIdx = 0;
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            ODataBatchOperationRequestMessage operationMessage = batchReader.CreateOperationRequestMessage();

                            // Verify operation message content type processing.
                            requestMessageContentTypeVerifier.Invoke(operationMessage, operationIdx);

                            operationIdx++;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Validation for the operation request message by checking some sample data points.
        /// </summary>
        /// <param name="requestMessage">The request operation message to be verified.</param>
        /// <param name="offset">The offset of the expected data used for verification.</param>
        /// <param name="requestIds">Expected values of the request Ids for request messages.</param>
        /// <param name="headersCounts">Expected counts of headers for the request messages.</param>
        private static void VerifyOperationRequestMessage(
            ODataBatchOperationRequestMessage requestMessage, int offset, string[] requestIds, int[] headersCounts)
        {
            Assert.Equal(requestIds[offset], requestMessage.ContentId);
            Assert.Equal(headersCounts[offset], requestMessage.Headers.Count());

            using (Stream contentStream = requestMessage.GetStream())
            {
                Assert.True(contentStream.Length > 0);
            }
        }

        private byte[] ServiceReadSingletonBatchRequestAndWriterBatchResponse(ODataJsonBatchPayloadTestCase testCase, ODataVersion version)
        {
            string requestPayload = testCase.RequestPayload;
            Action<ODataBatchOperationRequestMessage, IList<string>> requestOpMessageVerifier = testCase.RequestMessageDependsOnIdVerifier;

            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(Encoding.ASCII.GetBytes(requestPayload)) };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            MemoryStream responseStream = new MemoryStream();

            using (ODataMessageReader messageReader =
                new ODataMessageReader(requestMessage, new ODataMessageReaderSettings() {MaxProtocolVersion = version}, this.edmModel))
            {

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

                ODataMessageWriterSettings settings = new ODataMessageWriterSettings()
                {
                    Version = version
                };
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