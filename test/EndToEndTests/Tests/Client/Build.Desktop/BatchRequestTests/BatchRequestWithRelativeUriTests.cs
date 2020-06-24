//---------------------------------------------------------------------
// <copyright file="BatchRequestWithRelativeUriTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.BatchRequestTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class BatchRequestWithRelativeUriTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public BatchRequestWithRelativeUriTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [Fact]
        public void BatchRequestWithAbsoluteUriTest()
        {
            BatchRequestWithPayloadUriWritingOption(BatchPayloadUriOption.AbsoluteUri);
        }

        [Fact]
        public void BatchRequestWithAbsoluteResourcePathAndHostTest()
        {
            BatchRequestWithPayloadUriWritingOption(BatchPayloadUriOption.AbsoluteUriUsingHostHeader);
        }

        [Fact]
        public void BatchRequestWithRelativeResourcePathTest()
        {
            BatchRequestWithPayloadUriWritingOption(BatchPayloadUriOption.RelativeUri);
        }

        private void BatchRequestWithPayloadUriWritingOption(BatchPayloadUriOption option)
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.BaseUri = ServiceBaseUri;
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = Model.FindDeclaredType(NameSpacePrefix + "PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "$batch"));
            requestMessage.SetHeader("Content-Type", "multipart/mixed;boundary=batch_01AD6766-4A45-47CC-9463-94D4591D8DA9");
            requestMessage.SetHeader("OData-Version", "4.0");
            requestMessage.Method = "POST";

            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                //Batch start.
                batchWriter.WriteStartBatch();

                //A Get request.

                var batchOperation1 = batchWriter.CreateOperationRequestMessage("GET", new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments"), null, option);
                batchOperation1.SetHeader("Accept", "application/json;odata.metadata=full");
                //Get request ends. 

                //Changeset start.
                batchWriter.WriteStartChangeset();

                //The first operation in changeset is a Create request.

                ODataBatchOperationRequestMessage batchChangesetOperation1 = batchWriter.CreateOperationRequestMessage("POST", new Uri(ServiceBaseUri + "Accounts(102)/MyPaymentInstruments"), "1", option);
                batchChangesetOperation1.SetHeader("Content-Type", "application/json;odata.metadata=full");
                batchChangesetOperation1.SetHeader("Accept", "application/json;odata.metadata=full");
                var paymentInstrumentEntry = new ODataResource() { TypeName = NameSpacePrefix + "PaymentInstrument" };
                var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 102910 };
                var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "102 batch new PI" };
                var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57)) };
                paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

                using (var entryMessageWriter = new ODataMessageWriter(batchChangesetOperation1, writerSettings, Model))
                {
                    var odataEntryWriter = entryMessageWriter.CreateODataResourceWriter(myPaymentInstrumentSet, paymentInstrumentType);
                    odataEntryWriter.WriteStart(paymentInstrumentEntry);
                    odataEntryWriter.WriteEnd();
                }

                //Changeset end.
                batchWriter.WriteEndChangeset();

                //Another Get request.

                var batchOperation2 = batchWriter.CreateOperationRequestMessage("GET", new Uri(ServiceBaseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"), null, option);
                batchOperation2.SetHeader("Accept", "application/json;odata.metadata=full");

                //Batch end.
                batchWriter.WriteEndBatch();
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            using (var innerMessageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var batchReader = innerMessageReader.CreateODataBatchReader();
                int batchOperationId = 0;

                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Initial:
                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            break;
                        case ODataBatchReaderState.ChangesetEnd:
                            break;
                        case ODataBatchReaderState.Operation:
                            ODataBatchOperationResponseMessage operationResponse = batchReader.CreateOperationResponseMessage();

                            using (var operationResponseReader = new ODataMessageReader(operationResponse, readerSettings, Model))
                            {
                                if (batchOperationId == 0)
                                {
                                    // the first response message is a feed
                                    var feedReader = operationResponseReader.CreateODataResourceSetReader();

                                    Assert.Equal(200, operationResponse.StatusCode);
                                    List<ODataResource> pis = new List<ODataResource>();
                                    while (feedReader.Read())
                                    {
                                        switch (feedReader.State)
                                        {
                                            case ODataReaderState.ResourceEnd:
                                                ODataResource entry = feedReader.Item as ODataResource;
                                                Assert.NotNull(entry);
                                                pis.Add(entry);
                                                break;
                                        }
                                    }
                                    Assert.Equal(ODataReaderState.Completed, feedReader.State);
                                    Assert.Equal(3, pis.Count);
                                }
                                else if (batchOperationId == 1)
                                {
                                    // the second response message is a creation response
                                    var entryReader = operationResponseReader.CreateODataResourceReader();

                                    Assert.Equal(201, operationResponse.StatusCode);
                                    List<ODataResource> pis = new List<ODataResource>();
                                    while (entryReader.Read())
                                    {
                                        switch (entryReader.State)
                                        {
                                            case ODataReaderState.ResourceEnd:
                                                ODataResource entry = entryReader.Item as ODataResource;
                                                Assert.NotNull(entry);
                                                pis.Add(entry);
                                                break;
                                        }
                                    }
                                    Assert.Equal(ODataReaderState.Completed, entryReader.State);
                                    Assert.Equal(1, pis.Count);
                                    Assert.Equal(102910, pis[0].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                                }
                                else if (batchOperationId == 2)
                                {
                                    // the third response message is an entry
                                    var entryReader = operationResponseReader.CreateODataResourceReader();

                                    Assert.Equal(200, operationResponse.StatusCode);
                                    List<ODataResource> statements = new List<ODataResource>();
                                    while (entryReader.Read())
                                    {
                                        switch (entryReader.State)
                                        {
                                            case ODataReaderState.ResourceEnd:
                                                ODataResource entry = entryReader.Item as ODataResource;
                                                Assert.NotNull(entry);
                                                statements.Add(entry);
                                                break;
                                        }
                                    }
                                    Assert.Equal(ODataReaderState.Completed, entryReader.State);
                                    Assert.Equal(1, statements.Count);
                                    Assert.Equal(103901001, statements[0].Properties.Single(p => p.Name == "StatementID").Value);
                                }
                            }
                            batchOperationId++;
                            break;
                    }
                }
                Assert.Equal(ODataBatchReaderState.Completed, batchReader.State);
            }
        }
    }
}