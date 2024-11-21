//---------------------------------------------------------------------
// <copyright file="AsyncBatchRoundtripJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.Json
{
    public class AsyncBatchRoundtripJsonTests
    {
        private const string serviceDocumentUri = "http://service";
        private const string batchContentTypeMultipartMixed = "multipart/mixed; boundary=batch_36522ad7-fc75-4b56-8c71-56071383e77b";
        private const string batchContentTypeApplicationJson = "application/json";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmEntityType customerType;
        private readonly EdmEntitySet customers;

        private enum SkipBatchWriterStep
        {
            None,
            BatchStarted,
            ChangesetStarted,
            OperationCreated,
            OperationStreamRequested,
            OperationStreamDisposed,
            ChangesetCompleted,
            BatchCompleted
        }

        public AsyncBatchRoundtripJsonTests()
        {
            this.userModel = new EdmModel();

            this.customerType = new EdmEntityType("MyNS", "Customer");
            this.customerType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String);
            this.customerType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.customerType.AddStructuralProperty("LastName", EdmPrimitiveTypeKind.String);
            this.userModel.AddElement(this.customerType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.customers = this.defaultContainer.AddEntitySet("Customers", customerType);
        }

        #region Client Write Request Tests
        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithMissingTopLevelPropertyThrowsException()
        {
            string payload = @"{
                ""invalid"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        },
                        ""body"":{""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.Throws<ODataException>(Strings.ODataBatchReader_JsonBatchTopLevelPropertyMissing);
        }

        [Fact]
        public void AsyncBatchJsonTestWithInvalidBatchContentThrowsException()
        {
            Action test = () => AsyncBatchJsonTestFromSpecExample85(null);
            test.Throws<ODataException>(Strings.JsonReader_InvalidNumberFormat("--"));
        }

        [Fact]
        public void AsyncBatchJsonTestWithoutBatchStartedStatusThrowsException()
        {
            Action test = () => this.ClientWriteAsyncBatchRequest(
                BatchPayloadUriOption.AbsoluteUri,
                batchContentTypeApplicationJson,
                SkipBatchWriterStep.BatchStarted);
            test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromStart);
        }

        [Fact]
        public void AsyncBatchJsonTestCreateOperationAfterCreateBatchWriterThrowsException()
        {
            BatchPayloadUriOption payloadUriOption = BatchPayloadUriOption.AbsoluteUri;
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                Action test = () => batchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);
                test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromStart);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWithoutOperationChangesetCompletedStatusThrowsException()
        {
            Action test = () => this.ClientWriteAsyncBatchRequest(
                BatchPayloadUriOption.AbsoluteUri,
                batchContentTypeApplicationJson,
                SkipBatchWriterStep.ChangesetCompleted);

            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.StartsWith("An invalid HTTP method", exception.Message);
        }

        [Fact]
        public void AsyncBatchJsonTestWithoutOperationBatchCompletedStatusThrowsException()
        {
            BatchPayloadUriOption payloadUriOption = BatchPayloadUriOption.AbsoluteUri;
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                var updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);
                updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/Customers('ALFKI')"), "2", payloadUriOption);

                // Attempt to start another change set before ending the current one.
                Action test = () => batchWriter.WriteStartChangeset();
                test.Throws<ODataException>(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWriteEndBatchBeforeWriteEndChangesetThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                Action test = () => batchWriter.WriteEndBatch();
                test.Throws<ODataException>(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWriteEndChangesetBeforeWriteStartChangesetThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                Action test = () => batchWriter.WriteEndChangeset();
                test.Throws<ODataException>(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWriteStartBatchAfterWriteStartBatchThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                Action test = () => batchWriter.WriteStartBatch();
                test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestNestedWriteStartChangeSetThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                Action test = () => batchWriter.WriteStartChangeset();
                test.Throws<ODataException>(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestInvalidOperationAfterWriteStartChangeSetThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                Action test = () => batchWriter.WriteEndBatch();
                ODataException exception = Assert.Throws<ODataException>(test);
                Assert.StartsWith("An invalid method call on ODataBatchWriter was detected. You cannot call", exception.Message);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestInvalidOperationAfterOperationContentStreamDisposedThrowsException()
        {
            BatchPayloadUriOption payloadUriOption = BatchPayloadUriOption.AbsoluteUri;
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                var updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);

                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Id", Value = "AFKIL" }, new ODataProperty() { Name = "Name", Value = "Bob" } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                Action test = () => batchWriter.WriteStartBatch();
                test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestInvalidOperationAfterWriteEndChangeSetThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();
                batchWriter.WriteEndChangeset();

                Action test = () => batchWriter.WriteStartBatch();
                test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestInvalidOperationAfterWriteEndBatchThrowsException()
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(serviceDocumentUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();
                batchWriter.WriteEndChangeset();
                batchWriter.WriteEndBatch();

                Action test = () => batchWriter.WriteEndBatch();
                test.Throws<ODataException>(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWriteRequestExceedMaxPartsPerBatchThrowsException()
        {
            const int maxPartsPerBatch = 1;
            BatchPayloadUriOption payloadUriOption = BatchPayloadUriOption.AbsoluteUri;
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings
            {
                BaseUri = new Uri(serviceDocumentUri),
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = maxPartsPerBatch
                }
            }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                var updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);

                Action test = () => batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/Customers('ALFKI')"), "2", payloadUriOption);
                test.Throws<ODataException>(Strings.ODataBatchWriter_MaxBatchSizeExceeded(maxPartsPerBatch));
            }
        }

        [Fact]
        public void AsyncBatchJsonTestWriteRequestExceedMaxOperationsPerChangesetThrowsException()
        {
            const int maxOperationsPerChangeset = 1;
            BatchPayloadUriOption payloadUriOption = BatchPayloadUriOption.AbsoluteUri;
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings
            {
                BaseUri = new Uri(serviceDocumentUri),
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxOperationsPerChangeset = maxOperationsPerChangeset
                }
            }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                var updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "POST", new Uri(serviceDocumentUri + "/Customers"), "1", payloadUriOption);

                Action test = () => batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/Customers('ALFKI')"), "2", payloadUriOption);
                test.Throws<ODataException>(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(maxOperationsPerChangeset));
            }
        }
        #endregion

        #region Server Read Request Tests
        #region Multipart/Mixed
        [Fact]
        public void AsyncBatchJsonTestMultipartMixedBatchWithInvalidHeaderThrowsException()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            test.Throws<ODataException>(Strings.ODataBatchReaderStream_InvalidHeaderSpecified("--changeset_4faeec78-01a5-40c4-863e-9915be75db31--"));
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithBadFormatHttpMethodOperationThrowsException()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "POSThttp://service/CustomersHTTP/1.1\r\n" +
                "OData-Version: 4.0\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            test.Throws<ODataException>(Strings.ODataBatchReaderStream_InvalidRequestLine("POSThttp://service/CustomersHTTP/1.1"));
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithMissingHttpMethodOperationPayloadThrowsException()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "OData-Version: 4.0\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            test.Throws<ODataException>(Strings.ODataBatchReaderStream_InvalidRequestLine("OData-Version: 4.0"));
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithBadFormatHttpVersionThrowsException()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "POST http://service/Customers HTTPLOL/1.1\r\n" +
                "OData-Version: 4.0\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            test.Throws<ODataException>(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified("HTTPLOL/1.1", ODataConstants.HttpVersionInBatching));
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithGetRequestThrowsException()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "GET http://service/Customers HTTP/1.1\r\n" +
                "OData-Version: 4.0\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            test.Throws<ODataException>(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest("GET"));
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithEmptyOperationPasses()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "PUT http://service/Customers HTTP/1.1\r\n" +
                "OData-Version: 4.0\r\n" +
                "Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31--\r\n" +
                "--batch_36522ad7-fc75-4b56-8c71-56071383e77b--\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            Action test = () => this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeMultipartMixed);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestMultipartMixedWithoutEndBoundariesPasses()
        {
            string payload = "--batch_36522ad7-fc75-4b56-8c71-56071383e77b\r\n" +
                "Content-Type: multipart/mixed; boundary=changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n\r\n" +
                "--changeset_4faeec78-01a5-40c4-863e-9915be75db31\r\n" +
                "Content-Type: application/http\r\n" +
                "Content-Transfer-Encoding: binary\r\n" +
                "Content-ID: 1\r\n\r\n" +
                "PUT http://service/Customers HTTP/1.1\r\n" +
                "OData-Version: 4.0\r\n" +
                "Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\r\n\r\n";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            Action test = () => this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeMultipartMixed);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestReadRequestNoMessageCreatedForRequestThrowsException()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUri, batchContentTypeMultipartMixed);
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMixed);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings { BaseUri = new Uri(serviceDocumentUri) }, this.userModel))
            {
                var batchReader = messageReader.CreateODataBatchReader();

                Action test = () =>
                {
                    while (batchReader.Read())
                    {
                    }
                };
                test.Throws<ODataException>(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
            }
        }

        [Fact]
        public void AsyncBatchJsonTestReadRequestExceedMaxPartsPerBatchThrowsException()
        {
            const int maxPartsPerBatch = 1;
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUri, batchContentTypeMultipartMixed);
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMixed);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings
            {
                BaseUri = new Uri(serviceDocumentUri),
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = maxPartsPerBatch
                }
            }, this.userModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };
                var messageWriter = new ODataMessageWriter(responseMessage);
                var batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                var batchReader = messageReader.CreateODataBatchReader();

                Action test = () =>
                {
                    while (batchReader.Read())
                    {
                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                // Encountered an operation (either top-level or in a change set)
                                // Having a message for each operation is required
                                var operationMessage = batchReader.CreateOperationRequestMessage();
                                break;
                        }
                    }
                };
                test.Throws<ODataException>(Strings.ODataBatchReader_MaxBatchSizeExceeded(maxPartsPerBatch));
            }
        }

        [Fact]
        public void AsyncBatchJsonTestReadRequestExceedMaxOperationsPerChangesetThrowsException()
        {
            const int maxOperationsPerChangeset = 1;
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUri, batchContentTypeMultipartMixed);
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMixed);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings
            {
                BaseUri = new Uri(serviceDocumentUri),
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxOperationsPerChangeset = maxOperationsPerChangeset
                }
            }, this.userModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };
                var messageWriter = new ODataMessageWriter(responseMessage);
                var batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                var batchReader = messageReader.CreateODataBatchReader();

                Action test = () =>
                {
                    while (batchReader.Read())
                    {
                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                // Encountered an operation (either top-level or in a change set)
                                // Having a message for each operation is required
                                var operationMessage = batchReader.CreateOperationRequestMessage();
                                break;
                        }
                    }
                };
                test.Throws<ODataException>(Strings.ODataBatchReader_MaxChangeSetSizeExceeded(maxOperationsPerChangeset));
            }
        }
        #endregion

        #region JSON
        // OData 4.01 spec: http://docs.oasis-open.org/odata/odata-json-format/v4.01/csprd04/odata-json-format-v4.01-csprd04.html#sec_BatchRequest
        // See ODataJsonBatchAtomicityGroupTests.cs for a variety of atomicity group, request ID, and dependsOn scenarios
        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithMissingMethodThrowsException()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        },
                        ""body"":{""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.Throws<ODataException>(Strings.ODataBatchReader_RequestPropertyMissing("METHOD"));
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithMissingUrlThrowsException()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""POST"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        },
                        ""body"":{""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.Throws<ODataException>(Strings.ODataBatchReader_RequestPropertyMissing("URL"));
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithMissingHeadersPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""POST"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""body"":{""userPrincipalName"": ""mu6@odata.org"", ""givenName"": ""Jon6"", ""surname"": ""Doe""}
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithMissingHeaderBodyPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""POST"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        }
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchWithValidCaseInsensitiveRequestMethodPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""get_All_Caps"",
                        ""method"":""GET"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        }
                    },{
                        ""id"":""get_case_insensitive"",
                        ""method"":""gEt"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        }
                    },{
                        ""id"":""post_All_Caps"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""POST"",
                        ""url"":""http://service/Customers"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Id"":""AFKIL"",
                            ""Name"":""Bob"",
                            ""LastName"":""Dole""
                        }
                    },{
                        ""id"":""post_case_insensitive"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""pOsT"",
                        ""url"":""http://service/Customers"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Id"":""AFKIL2"",
                            ""Name"":""Bob2"",
                            ""LastName"":""Dole2""
                        }
                    },{
                        ""id"":""patch_All_Caps"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""PATCH"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai"",
                            ""LastName"":""Jack""
                        }
                    },{
                        ""id"":""put_All_Caps"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""PUT"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai4""
                        }
                    },{
                        ""id"":""put_case_insensitive"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""pUt"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai3""
                        }
                    },{
                        ""id"":""patch_case_insensitive"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""pATcH"",
                        ""url"":""http://service/Customers('ALFKI2')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai2"",
                            ""LastName"":""Jack2""
                        }
                    },{
                        ""id"":""delete_All_Caps"",
                        ""method"":""DELETE"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        }
                    },{
                        ""id"":""delete_case_insensitive"",
                        ""method"":""deLeTe"",
                        ""url"":""http://service/Customers('ALFKI2')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        }
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchAbsoluteUrlWithoutForwardSlashPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""GET"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        }
                    },{
                        ""id"":""g1r1"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""POST"",
                        ""url"":""http://service/Customers"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Id"":""AFKIL2"",
                            ""Name"":""Bob2"",
                            ""LastName"":""Dole2""
                        }
                    },{
                        ""id"":""g1r2"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""PATCH"",
                        ""url"":""http://service/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai"",
                            ""LastName"":""Jack""
                        }
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchAbsoluteUrlWithForwardSlashPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""GET"",
                        ""url"":""/Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        }
                    },{
                        ""id"":""g1r1"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""POST"",
                        ""url"":""/Customers"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Id"":""AFKIL2"",
                            ""Name"":""Bob2"",
                            ""LastName"":""Dole2""
                        }
                    },{
                        ""id"":""g1r2"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""PATCH"",
                        ""url"":""/Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai"",
                            ""LastName"":""Jack""
                        }
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }

        [Fact]
        public void AsyncBatchJsonTestJsonBatchRelativeUrlPasses()
        {
            string payload = @"{
                ""requests"":[{
                        ""id"":""77934a5b-e7cb-4959-a799-20351246d0b5"",
                        ""method"":""GET"",
                        ""url"":""Customers('ALFKI')"",
                        ""headers"":{
                            ""Content-Type"": ""application/json; odata.metadata=minimal; odata.streaming=true"",
                            ""OData-Version"": ""4.0""
                        }
                    },{
                        ""id"":""g1r1"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""POST"",
                        ""url"":""Customers"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Id"":""AFKIL2"",
                            ""Name"":""Bob2"",
                            ""LastName"":""Dole2""
                        }
                    },{
                        ""id"":""g1r2"",
                        ""atomicityGroup"":""b67707ba-fab4-47e8-9f5e-7271536fb98e"",
                        ""method"":""PATCH"",
                        ""url"":""Customers('ALFKI')"",
                        ""headers"":{
                            ""OData-Version"":""4.0"",
                            ""Content-Type"":""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
                        },
                        ""body"" :{
                            ""@odata.type"":""#MyNS.Customer"",
                            ""Name"":""Samurai"",
                            ""LastName"":""Jack""
                        }
                    }
                ]
            }";

            byte[] requestPayload = ConvertStringToByteArray(payload);

            Action test = () => this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            test.DoesNotThrow();
        }
        #endregion
        #endregion

        #region Round Trip Tests
        [Fact]
        public void AsyncBatchJsonTestFromSpecExample85MultipartMime()
        {
            AsyncBatchJsonTestFromSpecExample85(batchContentTypeMultipartMixed);
        }

        [Fact]
        public void AsyncBatchJsonTestFromSpecExample85Json()
        {
            AsyncBatchJsonTestFromSpecExample85(batchContentTypeApplicationJson);
        }

        private void AsyncBatchJsonTestFromSpecExample85(string batchContentType)
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUri, batchContentType);
            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentType);
            this.ClientReadAsyncBatchResponse(responsePayload, batchContentType);
        }

        [Fact]
        public void AsyncBatchMultipartMixedWritingAbsoluteResourcePathAndHostTest()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUriUsingHostHeader, batchContentTypeMultipartMixed);

            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);
            Assert.True(payloadString.Contains("GET /Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("POST /Customers HTTP/1.1") &&
                payloadString.Contains("PATCH /Customers('ALFKI') HTTP/1.1") &&
                payloadString.Contains("GET /Products HTTP/1.1"));

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeMultipartMixed);
        }

        [Fact]
        public void AsyncBatchMultipartMixedWritingRelativeResourcePathTest()
        {
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.RelativeUri, batchContentTypeMultipartMixed);

            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);
            Assert.Contains("GET Customers('ALFKI') HTTP/1.1", payloadString);
            Assert.Contains("POST Customers HTTP/1.1", payloadString);
            Assert.Contains("PATCH Customers('ALFKI') HTTP/1.1", payloadString);
            Assert.Contains("GET Products HTTP/1.1", payloadString);

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeMultipartMixed);
            this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeMultipartMixed);
        }


        [Fact]
        public void AsyncBatchJsonWritingAbsoluteResourcePathAndHostTest()
        {
            var baseUri = "http://example.com/root/service";
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.AbsoluteUriUsingHostHeader, batchContentTypeApplicationJson,
                SkipBatchWriterStep.None, baseUri);

            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);

            Assert.Contains("\"url\":\"/root/service/Customers('ALFKI')\"", payloadString);
            Assert.Contains("\"url\":\"/root/service/Customers\"", payloadString);
            Assert.Contains("\"url\":\"/root/service/Products\"", payloadString);
            Assert.Contains("\"headers\":{\"host\":\"example.com:80\"}}", payloadString);

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeApplicationJson);
        }

        [Fact]
        public void AsyncBatchJsonWritingRelativeResourcePathTest()
        {
            var baseUri = "http://example.com/root/service";
            var requestPayload = this.ClientWriteAsyncBatchRequest(BatchPayloadUriOption.RelativeUri, batchContentTypeApplicationJson,
                SkipBatchWriterStep.None, baseUri);

            var payloadString = System.Text.Encoding.Default.GetString(requestPayload);

            Assert.Contains("\"url\":\"Customers('ALFKI')\"", payloadString);
            Assert.Contains("\"url\":\"Customers\"", payloadString);
            Assert.Contains("\"url\":\"Products\"", payloadString);

            var responsePayload = this.ServiceReadAsyncBatchRequestAndWriteAsyncResponse(requestPayload, batchContentTypeApplicationJson);
            this.ClientReadAsyncBatchResponse(responsePayload, batchContentTypeApplicationJson);
        }
        #endregion

        #region Helper Functions
        private byte[] ClientWriteAsyncBatchRequest(
            BatchPayloadUriOption payloadUriOption,
            string batchContentType,
            SkipBatchWriterStep skipBatchWriterStep = SkipBatchWriterStep.None,
            string baseUri = null)
        {
            var stream = new MemoryStream();
            baseUri = baseUri ?? serviceDocumentUri;

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, new ODataMessageWriterSettings { BaseUri = new Uri(baseUri) }))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                if (skipBatchWriterStep != SkipBatchWriterStep.BatchStarted)
                {
                    batchWriter.WriteStartBatch();
                }

                // Write a query operation.
                if (skipBatchWriterStep != SkipBatchWriterStep.OperationCreated)
                {
                    batchWriter.CreateOperationRequestMessage("GET", new Uri(baseUri + "/Customers('ALFKI')"), /*contentId*/ null, payloadUriOption);
                }

                // Write a change set with multi update operation.
                if (skipBatchWriterStep != SkipBatchWriterStep.ChangesetStarted)
                {
                    batchWriter.WriteStartChangeset();
                }

                // Create a creation operation in the change set.
                if (skipBatchWriterStep != SkipBatchWriterStep.OperationCreated)
                {
                    var updateOperationMessage = batchWriter.CreateOperationRequestMessage("POST", new Uri(baseUri + "/Customers"), "1", payloadUriOption);

                    // Use a new message writer to write the body of this operation.
                    using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Id", Value = "AFKIL" }, new ODataProperty() { Name = "Name", Value = "Bob" } } };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(baseUri + "/Customers('ALFKI')"), "2", payloadUriOption);

                    using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource() { TypeName = "MyNS.Customer", Properties = new[] { new ODataProperty() { Name = "Name", Value = "Jack" } } };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }
                }

                if (skipBatchWriterStep != SkipBatchWriterStep.ChangesetCompleted)
                {
                    batchWriter.WriteEndChangeset();
                }

                // Write a query operation.
                if (skipBatchWriterStep != SkipBatchWriterStep.OperationCreated)
                {
                    batchWriter.CreateOperationRequestMessage("GET", new Uri(baseUri + "/Products"), /*contentId*/ null, payloadUriOption);
                }

                if (skipBatchWriterStep != SkipBatchWriterStep.BatchCompleted)
                {
                    batchWriter.WriteEndBatch();
                }

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadAsyncBatchRequestAndWriteAsyncResponse(byte[] requestPayload, string batchContentType)
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
                            // Encountered an operation (either top-level or in a change set)
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

        private void ClientReadAsyncBatchResponse(byte[] responsePayload, string batchContentType)
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
                                            Assert.Equal("ALFKI", Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Id")).Value);
                                            Assert.Equal("John", Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Name")).Value);
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

        private byte[] ConvertStringToByteArray(string content)
        {
            var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(content));
            stream.Position = 0;
            return stream.ToArray();
        }
        #endregion
    }
}
