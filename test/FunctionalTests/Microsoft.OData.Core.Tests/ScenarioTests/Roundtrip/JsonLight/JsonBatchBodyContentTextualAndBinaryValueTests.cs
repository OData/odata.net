//---------------------------------------------------------------------
// <copyright file="JsonBatchBodyContentTextualAndBinaryValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    using System;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.OData.Tests.JsonLight;
    using Xunit;

    public class JsonBatchBodyContentTextualAndBinaryValueTests
    {
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private const string serviceDocumentUri = "http://odata.org/test/";

        // Textual sample string for PUT request.
        private readonly string textualSampleStringA;

        // Textual sample string for GET response.
        private readonly string textualSampleStringB;

        // Binary sample bytes for PUT request.
        private readonly byte[] binarySampleBytesA;

        // Binary sample bytes for GET response.
        private readonly byte[] binarySampleBytesB;

        private const string ExpectedRequestTextualPayload = @"
{
    ""requests"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PUT"",
            ""url"": ""http://odata.org/test/MyBlob"",
            ""headers"": {
                ""content-type"": ""text/plain; charset=utf-8"",
                ""odata-version"": ""4.0""
            },
            ""body"": ""__ENCODED_TOKEN_A__""
        }, {
            ""id"": ""a05368c8-479d-4409-a2ee-9b54b133ec38"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test/MyBlob"",
            ""headers"": {
                ""accept"": ""text/plain; charset=utf-8""
            }
        }
    ]
}";

        private const string ExpectedResponseTextualPayload = @"
{
    ""responses"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 201,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""cbcdb345-afdc-4125-8832-fcd7e14a013f"",
            ""status"": 200,
            ""headers"": {
                ""content-type"": ""text/plain; charset=utf-8"",
                ""odata-version"": ""4.0""
            },
            ""body"": ""__ENCODED_TOKEN_B__""
        }
    ]
}";

        private const string ExpectedRequestBinaryPayload = @"
{
    ""requests"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PUT"",
            ""url"": ""http://odata.org/test/MyBlob"",
            ""headers"": {
                ""content-type"": ""application/octet-stream;charset=utf-8"",
                ""odata-version"": ""4.0""
            },
            ""body"": ""__ENCODED_TOKEN_A__""
        }, {
            ""id"": ""a05368c8-479d-4409-a2ee-9b54b133ec38"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test/MyBlob"",
            ""headers"": {
                ""accept"": ""application/octet-stream;charset=utf-8""
            }
        }
    ]
}";

        private const string ExpectedResponseBinaryPayload = @"
{
    ""responses"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 201,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""cbcdb345-afdc-4125-8832-fcd7e14a013f"",
            ""status"": 200,
            ""headers"": {
                ""content-type"": ""application/octet-stream;charset=utf-8"",
                ""odata-version"": ""4.0""
            },
            ""body"": ""__ENCODED_TOKEN_B__""
        }
    ]
}";

        public JsonBatchBodyContentTextualAndBinaryValueTests()
        {
            // Generate array of bytes representing all byte values.
            int binaryBytesLength = 256;
            this.binarySampleBytesA = new byte[binaryBytesLength];

            this.textualSampleStringA = "azAZ09~!@#$%^&*()_+{}|:\"';?/.,\\etc\r\n\b\t\f";
            this.textualSampleStringB = "\r\n\b\t\f,<.>?/'\":;\\|]}[{=+-_)(*&^%$#@!~09ZAza";

            for (int i = 0; i < binaryBytesLength; i++)
            {
                this.binarySampleBytesA[i] = (byte)(i & 0xff);
            }

            this.binarySampleBytesB = new byte[binaryBytesLength];
            for (int i = 0; i < binaryBytesLength; i++)
            {
                this.binarySampleBytesB[i] = (byte)(0xff - (i & 0xff));
            }
        }

        private enum BodyContentType
        {
            Textual,
            Binary
        }

        [Fact]
        public void RequestBodyContentTextualValueTest()
        {
            BodyContentTest(BodyContentType.Textual);
        }

        [Fact]
        public void RequestBodyContentBinaryValueTest()
        {
            BodyContentTest(BodyContentType.Binary);
        }

        private void BodyContentTest(BodyContentType bodyContentType)
        {
            byte[] requestPayload = ClientWriteBatchRequest(bodyContentType);

            VerifyPayload(requestPayload, bodyContentType,  /*forRequest*/true);
            byte[] responsePayload = this.ServiceReadBatchRequestAndWriterBatchResponse(requestPayload, bodyContentType);
            VerifyPayload(responsePayload, bodyContentType, /*forRequest*/false);
            this.ClientReadBatchResponse(responsePayload, bodyContentType);
        }

        private void VerifyPayload(byte[] payloadBytes, BodyContentType bodyContentType, bool forRequest)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {

                string payload = sr.ReadToEnd();

                string expectedPayload = null;
                if (forRequest)
                {
                    expectedPayload = Regex.Replace(
                        bodyContentType == BodyContentType.Textual
                        ? ExpectedRequestTextualPayload
                        : ExpectedRequestBinaryPayload,
                        "\"body\": \"__ENCODED_TOKEN_A__\"",
                        "\"body\":" + GetEncodedStringContent(bodyContentType, true),
                        RegexOptions.Multiline);
                }
                else
                {
                    expectedPayload = Regex.Replace(
                        bodyContentType == BodyContentType.Textual
                        ? ExpectedResponseTextualPayload
                        : ExpectedResponseBinaryPayload,
                        "\"body\": \"__ENCODED_TOKEN_B__\"",
                        "\"body\":" + GetEncodedStringContent(bodyContentType, false),
                        RegexOptions.Multiline);
                }

                string expectedNormalizedPayload = GetNormalizedJsonMessage(expectedPayload);
                string normalizedPayload = GetNormalizedJsonMessage(payload);
                Assert.True(expectedNormalizedPayload.Equals(normalizedPayload));
            }
        }

        private byte[] ClientWriteBatchRequest(BodyContentType bodyContentType)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader(ODataConstants.ContentTypeHeader, batchContentTypeApplicationJson);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a change set with update operation.
                batchWriter.WriteStartChangeset();

                // Create an update operation in the change set.
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PUT", new Uri(serviceDocumentUri + "MyBlob"), "1");

                // Set the content type with explicit character set so that the content string
                // is flushed into the operation message body stream without byte-order-mark.
                updateOperationMessage.SetHeader("CoNtEnt-TYPE", GetContentType(bodyContentType));

                // Use the message writer to write encoded string content.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    operationMessageWriter.WriteValue(GetEncodedContentObject(bodyContentType, /*forRequest*/ true));
                }

                batchWriter.WriteEndChangeset();

                // Write a query operation.
                ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(serviceDocumentUri + "MyBlob"), /*contentId*/ null);

                // Header modification on inner payload.
                queryOperationMessage.SetHeader("AcCePt", GetContentType(bodyContentType));

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private string GetEncodedStringContent(BodyContentType bodyContentType, bool forRequest)
        {
            string result = null;
            switch (bodyContentType)
            {
                case BodyContentType.Textual:
                    string text = JsonLightUtils.GetJsonEncodedString(forRequest ? this.textualSampleStringA : this.textualSampleStringB);
                    result = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", text);
                    break;

                case BodyContentType.Binary:
                    byte[] bytes = forRequest ? this.binarySampleBytesA : this.binarySampleBytesB;
                    // Beginning double quote and ending double quote are needed for Json string representation of
                    // base64url-encoded data.
                    result = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", JsonLightUtils.GetBase64UrlEncodedString(bytes));
                    break;

                default:
                    break;
            }

            return result;
        }


        private object GetEncodedContentObject(BodyContentType bodyContentType, bool forRequest)
        {
            object result = null;
            switch (bodyContentType)
            {
                case BodyContentType.Textual:
                    result = GetEncodedStringContent(bodyContentType, forRequest);
                    break;

                case BodyContentType.Binary:
                    // Create binary value object representing a string consisting of base64url-encoding characters
                    result = Encoding.UTF8.GetBytes(GetEncodedStringContent(bodyContentType, forRequest));
                    break;

                default:
                    break;
            }

            return result;
        }

        private byte[] ServiceReadBatchRequestAndWriterBatchResponse(byte[] requestPayload, BodyContentType bodyContentType)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader(ODataConstants.ContentTypeHeader, batchContentTypeApplicationJson);

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), null))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader(ODataConstants.ContentTypeHeader, batchContentTypeApplicationJson);
                ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage);
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:

                            ODataBatchOperationRequestMessage operationMessage = batchReader.CreateOperationRequestMessage();

                            if (operationMessage.Method == "PUT")
                            {
                                using (Stream operationMessageBodyStream = operationMessage.GetStream())
                                {
                                    // Verify the bytes in the request operation stream.
                                    byte[] sampleBytes = bodyContentType == BodyContentType.Textual
                                        ? Encoding.UTF8.GetBytes("\"" + this.textualSampleStringA + "\"")
                                        : binarySampleBytesA;
                                    Assert.Equal(operationMessageBodyStream.Length, sampleBytes.Length);
                                    foreach (byte samplebyte in sampleBytes)
                                    {
                                        Assert.Equal(samplebyte, operationMessageBodyStream.ReadByte());
                                    }
                                }

                                // Create the response.
                                ODataBatchOperationResponseMessage operationResponse = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                operationResponse.StatusCode = 201;
                                operationResponse.SetHeader("CoNtEnT-TyPe", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                ODataBatchOperationResponseMessage operationResponse = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                operationResponse.StatusCode = 200;
                                operationResponse.SetHeader("CoNtEnT-TyPe", GetContentType(bodyContentType));
                                ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
                                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(operationResponse, settings, null))
                                {
                                    operationMessageWriter.WriteValue(GetEncodedContentObject(bodyContentType, false));
                                }
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

        private void ClientReadBatchResponse(byte[] responsePayload, BodyContentType bodyContentType)
        {
            IODataResponseMessage responseMessage = new InMemoryMessage() { Stream = new MemoryStream(responsePayload) };
            responseMessage.SetHeader(ODataConstants.ContentTypeHeader, batchContentTypeApplicationJson);
            using (ODataMessageReader messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), null))
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
                                using (Stream operationMessageBody = operationMessage.GetStream())
                                {
                                    // Verify the bytes in the response body.
                                    byte[] sampleBytes = bodyContentType == BodyContentType.Textual
                                        ? Encoding.UTF8.GetBytes("\"" + this.textualSampleStringB + "\"")
                                        : this.binarySampleBytesB;

                                    Assert.Equal(operationMessageBody.Length, sampleBytes.Length);
                                    foreach (byte samplebyte in sampleBytes)
                                    {
                                        Assert.Equal(samplebyte, operationMessageBody.ReadByte());
                                    }
                                }
                            }
                            else
                            {
                                Assert.True(201 == operationMessage.StatusCode);
                            }
                            break;
                    }
                }
            }
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
            const string myODataVersionProperty = @"""odata-version"":""myODataVer""";

            string result = Regex.Replace(jsonMessage, @"\s*", "", RegexOptions.Multiline);
            result = Regex.Replace(result, "\"id\":\"[^\"]*\"", myIdProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"atomicityGroup\":\"[^\"]*\"", myAtomicGroupProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"odata-version\":\"[^\"]*\"", myODataVersionProperty, RegexOptions.Multiline);

            return result;
        }

        private string GetContentType(BodyContentType bodyContentType)
        {
            string result;
            switch (bodyContentType)
            {
                case BodyContentType.Textual:
                    result = "text/plain; charset=utf-8";
                    break;

                case BodyContentType.Binary:
                    result = MimeConstants.MimeApplicationOctetStream + "; charset=utf-8";
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}

