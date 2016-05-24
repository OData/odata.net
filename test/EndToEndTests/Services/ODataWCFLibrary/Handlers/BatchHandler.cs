//---------------------------------------------------------------------
// <copyright file="BatchHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System.Net;
    using Microsoft.OData;

    public class BatchHandler : RequestHandler
    {
        public BatchHandler(RequestHandler other) :
            base(other, HttpMethod.POST, other.RequestUri, null)
        {
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            responseMessage.SetStatusCode(HttpStatusCode.OK);

            using (var batchRequestMessageWriter = this.CreateMessageWriter(responseMessage))
            {
                var batchWriter = batchRequestMessageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                using (var batchRequestMessageReader = this.CreateBatchMessageReader(requestMessage))
                {
                    var batchReader = batchRequestMessageReader.CreateODataBatchReader();

                    while (batchReader.Read())
                    {
                        switch (batchReader.State)
                        {
                            case ODataBatchReaderState.Initial:
                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                batchWriter.WriteStartChangeset();
                                break;
                            case ODataBatchReaderState.ChangesetEnd:
                                batchWriter.WriteEndChangeset();
                                break;
                            case ODataBatchReaderState.Operation:
                                ODataBatchOperationRequestMessage operationRequestMessage =
                                    batchReader.CreateOperationRequestMessage();

                                ODataBatchOperationResponseMessage operationResponseMessage =
                                    batchWriter.CreateOperationResponseMessage(operationRequestMessage.ContentId);

                                HttpMethod method = Utility.CreateHttpMethod(operationRequestMessage.Method);

                                switch (method)
                                {
                                    case HttpMethod.GET:
                                        new QueryHandler(this, operationRequestMessage.Url, operationRequestMessage.Headers).Process(operationRequestMessage, operationResponseMessage);
                                        break;
                                    case HttpMethod.POST:
                                        new CreateHandler(this, operationRequestMessage.Url, operationRequestMessage.Headers).Process(operationRequestMessage, operationResponseMessage);
                                        break;
                                    case HttpMethod.DELETE:
                                        new DeleteHandler(this, operationRequestMessage.Url, operationRequestMessage.Headers).Process(operationRequestMessage, operationResponseMessage);
                                        break;
                                    case HttpMethod.PATCH:
                                    case HttpMethod.PUT:
                                        new UpdateHandler(this, method, operationRequestMessage.Url, operationRequestMessage.Headers).Process(operationRequestMessage, operationResponseMessage);
                                        break;
                                    default:
                                        new ErrorHandler(this, Utility.BuildException(HttpStatusCode.MethodNotAllowed)).Process(operationRequestMessage, operationResponseMessage);
                                        break;
                                }

                                break;
                        }
                    }
                }

                batchWriter.WriteEndBatch();
            }

        }

        private ODataMessageReader CreateBatchMessageReader(IODataRequestMessage message)
        {
            return new ODataMessageReader(
                message,
                new ODataMessageReaderSettings
                {
                    BaseUri = ServiceConstants.ServiceBaseUri
                });
        }

    }
}