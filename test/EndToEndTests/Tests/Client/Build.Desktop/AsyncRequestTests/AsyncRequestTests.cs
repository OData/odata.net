//---------------------------------------------------------------------
// <copyright file="AsyncRequestTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AsyncRequestTests
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

    public class AsyncRequestTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public AsyncRequestTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [Fact]
        public void AsyncQueryRequestTest()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(202, responseMessage.StatusCode);
                string monitorResource = responseMessage.GetHeader("Location");
                Assert.False(string.IsNullOrWhiteSpace(monitorResource));

                var statusCheckRequest1 = new HttpWebRequestMessage(new Uri(monitorResource));
                var statusCheckResponse1 = statusCheckRequest1.GetResponse();
                Assert.Equal(202, statusCheckResponse1.StatusCode);
                monitorResource = statusCheckResponse1.GetHeader("Location");
                Assert.False(string.IsNullOrWhiteSpace(monitorResource));

                //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
                Thread.Sleep(6000); 
                var statusCheckRequest2 = new HttpWebRequestMessage(new Uri(monitorResource));
                var statusCheckResponse2 = statusCheckRequest2.GetResponse();
                Assert.Equal(200, statusCheckResponse2.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, Model))
                    {
                        var asyncReader = messageReader.CreateODataAsynchronousReader();
                        var innerMessage = asyncReader.CreateResponseMessage();

                        Assert.Equal(200, innerMessage.StatusCode);
                        using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();
                            var feedReader = innerMessageReader.CreateODataResourceSetReader();

                            while (feedReader.Read())
                            {
                                if (feedReader.State == ODataReaderState.ResourceEnd)
                                {
                                    ODataResource entry = feedReader.Item as ODataResource;
                                    if (entry != null && entry.Id != null)
                                    {
                                        entries.Add(entry);
                                    }
                                }
                                else if (feedReader.State == ODataReaderState.ResourceSetEnd)
                                {
                                    Assert.NotNull(feedReader.Item as ODataResourceSet);
                                }
                            }

                            Assert.Equal(ODataReaderState.Completed, feedReader.State);
                            Assert.Equal(5, entries.Count);

                            Assert.Equal("Bob", entries[0].Properties.Single(p => p.Name == "FirstName").Value);
                            Assert.Equal("Peter", entries[4].Properties.Single(p => p.Name == "FirstName").Value);
                        }

                    }
                }
            }
        }

        [Fact]
        public void AsyncCreateRequestTest()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                #region send the Create request with respond-async preference

                var accountEntry = new ODataResourceWrapper()
                {
                    Resource = new ODataResource
                    {
                        TypeName = NameSpacePrefix + "Account",
                        Properties = new[]
                        {
                            new ODataProperty { Name = "AccountID", Value = 110 },
                            new ODataProperty { Name = "CountryRegion", Value = "CN" }
                        }
                    },
                    NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                    {
                        new ODataNestedResourceInfoWrapper()
                        {
                            NestedResourceInfo = new ODataNestedResourceInfo
                            {
                                Name = "AccountInfo",
                                IsCollection = false
                            },
                            NestedResourceOrResourceSet = new ODataResourceWrapper()
                            {
                                Resource = new ODataResource()
                                {
                                    TypeName = NameSpacePrefix + "AccountInfo",
                                    Properties = new []
                                    {
                                        new ODataProperty
                                        {
                                            Name = "FirstName",
                                            Value = "FN"
                                        },
                                        new ODataProperty
                                        {
                                            Name = "LastName",
                                            Value = "LN"
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                var settings = new ODataMessageWriterSettings();
                settings.BaseUri = ServiceBaseUri;

                var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
                var accountSet = Model.EntityContainer.FindEntitySet("Accounts");

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts"));
                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(accountSet, accountType);
                    ODataWriterHelper.WriteResource(odataWriter, accountEntry);
                }

                var responseMessage = requestMessage.GetResponse();

                #endregion

                Assert.Equal(202, responseMessage.StatusCode);
                string monitorResource = responseMessage.GetHeader("Location");
                Assert.False(string.IsNullOrWhiteSpace(monitorResource));

                var statusCheckRequest1 = new HttpWebRequestMessage(new Uri(monitorResource));
                var statusCheckResponse1 = statusCheckRequest1.GetResponse();
                Assert.Equal(202, statusCheckResponse1.StatusCode);
                monitorResource = statusCheckResponse1.GetHeader("Location");
                Assert.False(string.IsNullOrWhiteSpace(monitorResource));

                //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
                Thread.Sleep(6000);
                var statusCheckRequest2 = new HttpWebRequestMessage(new Uri(monitorResource));
                var statusCheckResponse2 = statusCheckRequest2.GetResponse();
                Assert.Equal(200, statusCheckResponse2.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, Model))
                    {
                        var asyncReader = messageReader.CreateODataAsynchronousReader();
                        var innerMessage = asyncReader.CreateResponseMessage();

                        Assert.Equal(201, innerMessage.StatusCode);
                        using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();
                            var entryReader = innerMessageReader.CreateODataResourceReader();

                            while (entryReader.Read())
                            {
                                if (entryReader.State == ODataReaderState.ResourceEnd)
                                {
                                    ODataResource entry = entryReader.Item as ODataResource;
                                    Assert.NotNull(entry);
                                    entries.Add(entry);
                                }
                            }

                            Assert.Equal(ODataReaderState.Completed, entryReader.State);
                            Assert.Equal(2, entries.Count);
                            Assert.Equal(110, entries[1].Properties.Single(p => p.Name == "AccountID").Value);
                        }

                    }
                }
            }
        }

        [Fact]
        public void AsyncBatchRequestTest()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.BaseUri = ServiceBaseUri;
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            #region send a batch request with respond-async preference

            var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = Model.FindDeclaredType(NameSpacePrefix + "PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "$batch"));
            requestMessage.SetHeader("Content-Type", "multipart/mixed;boundary=batch_01AD6766-4A45-47CC-9463-94D4591D8DA9");
            requestMessage.SetHeader("OData-Version", "4.0");
            requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
            requestMessage.Method = "POST";

            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                //Batch start.
                batchWriter.WriteStartBatch();

                //A Get request.

                var batchOperation1 = batchWriter.CreateOperationRequestMessage("GET", new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments"), null);
                batchOperation1.SetHeader("Accept", "application/json;odata.metadata=full");
                //Get request ends. 

                //Changeset start.
                batchWriter.WriteStartChangeset();

                //The first operation in changeset is a Create request.

                ODataBatchOperationRequestMessage batchChangesetOperation1 = batchWriter.CreateOperationRequestMessage("POST", new Uri(ServiceBaseUri + "Accounts(102)/MyPaymentInstruments"), "1");
                batchChangesetOperation1.SetHeader("Content-Type", "application/json;odata.metadata=full");
                batchChangesetOperation1.SetHeader("Accept", "application/json;odata.metadata=full");
                var paymentInstrumentEntry = new ODataResource() {TypeName = NameSpacePrefix + "PaymentInstrument"};
                var paymentInstrumentEntryP1 = new ODataProperty {Name = "PaymentInstrumentID", Value = 102910};
                var paymentInstrumentEntryP2 = new ODataProperty {Name = "FriendlyName", Value = "102 batch new PI"};
                var paymentInstrumentEntryP3 = new ODataProperty {Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57))};
                paymentInstrumentEntry.Properties = new[] {paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3};

                using (var entryMessageWriter = new ODataMessageWriter(batchChangesetOperation1, writerSettings, Model))
                {
                    var odataEntryWriter = entryMessageWriter.CreateODataResourceWriter(myPaymentInstrumentSet, paymentInstrumentType);
                    odataEntryWriter.WriteStart(paymentInstrumentEntry);
                    odataEntryWriter.WriteEnd();
                }

                //Changeset end.
                batchWriter.WriteEndChangeset();

                //Another Get request.

                var batchOperation2 = batchWriter.CreateOperationRequestMessage("GET", new Uri(ServiceBaseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"), null);
                batchOperation2.SetHeader("Accept", "application/json;odata.metadata=full");

                //Batch end.
                batchWriter.WriteEndBatch();
            }

            var responseMessage = requestMessage.GetResponse();

            #endregion

            #region request the status monitor resource

            Assert.Equal(202, responseMessage.StatusCode);
            string monitorResource = responseMessage.GetHeader("Location");
            Assert.False(string.IsNullOrWhiteSpace(monitorResource));

            var statusCheckRequest1 = new HttpWebRequestMessage(new Uri(monitorResource));
            var statusCheckResponse1 = statusCheckRequest1.GetResponse();
            Assert.Equal(202, statusCheckResponse1.StatusCode);
            monitorResource = statusCheckResponse1.GetHeader("Location");
            Assert.False(string.IsNullOrWhiteSpace(monitorResource));

            //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
            Thread.Sleep(6000);
            var statusCheckRequest2 = new HttpWebRequestMessage(new Uri(monitorResource));
            var statusCheckResponse2 = statusCheckRequest2.GetResponse();
            Assert.Equal(200, statusCheckResponse2.StatusCode);

            #endregion

            #region read and verify the response

            using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, Model))
            {
                var asyncReader = messageReader.CreateODataAsynchronousReader();
                var innerMessage = asyncReader.CreateResponseMessage();

                Assert.Equal(200, innerMessage.StatusCode);
                using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, Model))
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
                                        Assert.Single(pis);
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
                                        Assert.Single(statements);
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

            #endregion
        }
    }
}
