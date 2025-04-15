//---------------------------------------------------------------------
// <copyright file="AsyncRequestTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.AsyncRequestTests;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.AsyncRequestTests;

/// <summary>
/// Contains end-to-end tests for validating asynchronous OData requests.
/// These tests ensure that the OData service correctly handles asynchronous operations
/// such as querying, creating, and batching resources.
/// </summary>
public class AsyncRequestTests : EndToEndTestBase<AsyncRequestTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(MetadataController), typeof(AsyncRequestTestsController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
        }
    }

    public AsyncRequestTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    // Constants
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default.";

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task AsyncQueryRequest_ReturnsAcceptedStatusAndValidatesResponse(string mimeType)
    {
        // Arrange
        var requestMessage = new CustomHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "People", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", "respond-async"); //Request the service to process asynchronously.

        // Act
        var responseMessage = await requestMessage.GetResponseAsync(mimeType);

        // Assert
        Assert.Equal(202, responseMessage.StatusCode);
        var monitorResource = responseMessage.GetHeader("Location");

        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync(mimeType);

        Assert.Equal(202, statusCheckResponse1.StatusCode);
        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync(mimeType);

        Assert.Equal(200, statusCheckResponse2.StatusCode);

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
        {
            var entries = new List<ODataResource>();
            var feedReader = messageReader.CreateODataResourceSetReader();

            while (feedReader.Read())
            {
                if (feedReader.State == ODataReaderState.ResourceEnd)
                {
                    if (feedReader.Item is ODataResource entry && entry.Id != null)
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
            Assert.Equal("Bob", (entries[0].Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value);
            Assert.Equal("Peter", (entries[4].Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value);

        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task AsyncCreateRequest_ReturnsAcceptedStatusAndValidatesResponse(string mimeType)
    {
        // Arrange
        var accountEntry = new ODataResourceWrapper()
        {
            Resource = new ODataResource
            {
                TypeName = NameSpacePrefix + "Account",
                Properties =
                [
                    new ODataProperty { Name = "AccountID", Value = 110 },
                    new ODataProperty { Name = "CountryRegion", Value = "CN" }
                ]
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

        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false,
        };

        var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

        var requestMessage = new CustomHttpClientRequestMessage(new Uri(_baseUri + "Accounts"), Client);
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", "respond-async"); //Request the service to process asynchronously.
        requestMessage.Method = "POST";
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
        {
            var odataWriter = messageWriter.CreateODataResourceWriter(accountSet, accountType);
            ODataWriterHelper.WriteResource(odataWriter, accountEntry);
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync(mimeType, requestMessage);

        // Assert
        Assert.Equal(202, responseMessage.StatusCode);
        string monitorResource = responseMessage.GetHeader("Location");

        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync(mimeType);

        Assert.Equal(202, statusCheckResponse1.StatusCode);

        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync(mimeType);

        Assert.Equal(201, statusCheckResponse2.StatusCode);

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
        {
            List<ODataResource> entries = new List<ODataResource>();
            var entryReader = messageReader.CreateODataResourceReader();

            while (entryReader.Read())
            {
                if (entryReader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = entryReader.Item as ODataResource;
                    Assert.NotNull(entry);
                    entries.Add(entry);
                }
            }

            Assert.Equal(ODataReaderState.Completed, entryReader.State);
            Assert.Equal(2, entries.Count);
            Assert.Equal(110, (entries[1].Properties.Single(p => p.Name == "AccountID") as ODataProperty)?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task AsyncBatchRequest_ReturnsAcceptedStatusAndValidatesResponse(string mimeType)
    {
        // Arrange
        var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
        var paymentInstrumentType = _model.FindDeclaredType(NameSpacePrefix + "PaymentInstrument") as IEdmEntityType;
        var navProp = accountType?.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
        var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

        // Act
        var requestMessage = new CustomHttpClientRequestMessage(new Uri(_baseUri + "$batch"), Client);
        requestMessage.SetHeader("Content-Type", "multipart/mixed;boundary=batch_01AD6766-4A45-47CC-9463-94D4591D8DA9");
        requestMessage.SetHeader("OData-Version", "4.0");
        requestMessage.SetHeader("Prefer", "respond-async"); //Request the service to process asynchronously.
        requestMessage.Method = "POST";

        var writerSettings = new ODataMessageWriterSettings { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var batchWriter = messageWriter.CreateODataBatchWriter();

            //Batch start.
            batchWriter.WriteStartBatch();

            //A Get request.

            var batchOperation1 = batchWriter.CreateOperationRequestMessage("GET", new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments"), null);
            batchOperation1.SetHeader("Accept", mimeType);
            //Get request ends. 

            //Changeset start.
            batchWriter.WriteStartChangeset();

            //The first operation in changeset is a Create request.

            ODataBatchOperationRequestMessage batchChangesetOperation1 = batchWriter.CreateOperationRequestMessage("POST", new Uri(_baseUri + "Accounts(102)/MyPaymentInstruments"), "1");
            batchChangesetOperation1.SetHeader("Content-Type", mimeType);
            batchChangesetOperation1.SetHeader("Accept", mimeType);
            var paymentInstrumentEntry = new ODataResource() { TypeName = NameSpacePrefix + "PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 102910 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "102 batch new PI" };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57)) };
            paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

            using (var entryMessageWriter = new ODataMessageWriter(batchChangesetOperation1))
            {
                var odataEntryWriter = entryMessageWriter.CreateODataResourceWriter(myPaymentInstrumentSet, paymentInstrumentType);
                odataEntryWriter.WriteStart(paymentInstrumentEntry);
                odataEntryWriter.WriteEnd();
            }

            //Changeset end.
            batchWriter.WriteEndChangeset();

            //Another Get request.

            var batchOperation2 = batchWriter.CreateOperationRequestMessage("GET", new Uri(_baseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"), null);
            batchOperation2.SetHeader("Accept", mimeType);

            //Batch end.
            batchWriter.WriteEndBatch();
        }

        var responseMessage = await requestMessage.GetResponseAsync(mimeType, requestMessage);

        // Assert
        Assert.Equal(202, responseMessage.StatusCode);
        string monitorResource = responseMessage.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync(mimeType);
        Assert.Equal(202, statusCheckResponse1.StatusCode);
        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new CustomHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync(mimeType);
        Assert.Equal(200, statusCheckResponse2.StatusCode);

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };
        using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
        {
            var batchReader = messageReader.CreateODataBatchReader();

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

                        using (var operationResponseReader = new ODataMessageReader(operationResponse, readerSettings, _model))
                        {
                            if (batchOperationId == 0)
                            {
                                // the first response message is a feed
                                var feedReader = operationResponseReader.CreateODataResourceSetReader();

                                Assert.Equal(200, operationResponse.StatusCode);

                                var pis = new List<ODataResource>();
                                while (feedReader.Read())
                                {
                                    switch (feedReader.State)
                                    {
                                        case ODataReaderState.ResourceEnd:
                                            var entry = feedReader.Item as ODataResource;
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

                                var pis = new List<ODataResource>();
                                while (entryReader.Read())
                                {
                                    switch (entryReader.State)
                                    {
                                        case ODataReaderState.ResourceEnd:
                                            var entry = entryReader.Item as ODataResource;
                                            Assert.NotNull(entry);
                                            pis.Add(entry);
                                            break;
                                    }
                                }
                                Assert.Equal(ODataReaderState.Completed, entryReader.State);
                                Assert.Single(pis);
                                Assert.Equal(102910, (pis[0].Properties.Single(p => p.Name == "PaymentInstrumentID") as ODataProperty)?.Value);
                            }
                            else if (batchOperationId == 2)
                            {
                                // the third response message is an entry
                                var entryReader = operationResponseReader.CreateODataResourceReader();

                                Assert.Equal(200, operationResponse.StatusCode);

                                var statements = new List<ODataResource>();
                                while (entryReader.Read())
                                {
                                    switch (entryReader.State)
                                    {
                                        case ODataReaderState.ResourceEnd:
                                            var entry = entryReader.Item as ODataResource;
                                            Assert.NotNull(entry);
                                            statements.Add(entry);
                                            break;
                                    }
                                }
                                Assert.Equal(ODataReaderState.Completed, entryReader.State);
                                Assert.Single(statements);
                                Assert.Equal(103901001, (statements[0].Properties.Single(p => p.Name == "StatementID") as ODataProperty)?.Value);
                            }
                        }
                        batchOperationId++;
                        break;
                }
            }
            Assert.Equal(ODataBatchReaderState.Completed, batchReader.State);

        }
    }

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "asyncrequetstests/Default.ResetDefaultDataSource", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(actionUri, Client);
        requestMessage.Method = "POST";

        var responseMessage = requestMessage.GetResponseAsync().Result;

        Assert.Equal(200, responseMessage.StatusCode);
    }

    #endregion
}
