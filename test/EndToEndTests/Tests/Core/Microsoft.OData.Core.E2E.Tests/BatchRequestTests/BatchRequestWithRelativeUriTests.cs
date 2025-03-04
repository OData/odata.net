//---------------------------------------------------------------------
// <copyright file="BatchRequestWithRelativeUriTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.BatchRequest;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.BatchRequestTests;

public class BatchRequestWithRelativeUriTests : EndToEndTestBase<BatchRequestWithRelativeUriTests.TestsStartup>
{
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default";

    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(BatchRequestTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
        }
    }

    public BatchRequestWithRelativeUriTests(TestWebApplicationFactory<TestsStartup> fixture) 
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    [Theory]
    [InlineData(BatchPayloadUriOption.AbsoluteUri)]
    [InlineData(BatchPayloadUriOption.RelativeUri)]
    [InlineData(BatchPayloadUriOption.AbsoluteUriUsingHostHeader)]
    public async Task BatchRequestWithResourcePathTest(BatchPayloadUriOption option)
    {
        // Arrange
        var writerSettings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };

        var accountType = _model.FindDeclaredType($"{NameSpacePrefix}.Account") as IEdmEntityType;
        Assert.NotNull(accountType);

        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
        Assert.NotNull(accountSet);

        var paymentInstrumentType = _model.FindDeclaredType($"{NameSpacePrefix}.PaymentInstrument") as IEdmEntityType;
        Assert.NotNull(paymentInstrumentType);

        var navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
        Assert.NotNull(navProp);

        var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);
        Assert.NotNull(myPaymentInstrumentSet);

        var requestUrl = new Uri(_baseUri.AbsoluteUri + "$batch", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Content-Type", "multipart/mixed;boundary=batch_01AD6766-4A45-47CC-9463-94D4591D8DA9");
        requestMessage.SetHeader("OData-Version", "4.0");
        requestMessage.Method = "POST";

        await using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var batchWriter = await messageWriter.CreateODataBatchWriterAsync();

            // Batch start.
            await batchWriter.WriteStartBatchAsync();

            // A Get request.
            requestUrl = new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments");
            var batchOperation1 = await batchWriter.CreateOperationRequestMessageAsync("GET", requestUrl, null, option);
            batchOperation1.SetHeader("Accept", "application/json;odata.metadata=full");
            // Get request ends. 

            // Changeset start.
            await batchWriter.WriteStartChangesetAsync();

            // The first operation in changeset is a Create request.
            requestUrl = new Uri(_baseUri + "Accounts(102)/MyPaymentInstruments");
            var batchChangesetOperation1 = await batchWriter.CreateOperationRequestMessageAsync("POST", requestUrl, "1", option);
            batchChangesetOperation1.SetHeader("Content-Type", "application/json;odata.metadata=full");
            batchChangesetOperation1.SetHeader("Accept", "application/json;odata.metadata=full");

            var paymentInstrumentEntry = new ODataResource() { TypeName = $"{NameSpacePrefix}.PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 102910 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "102 batch new PI" };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57)) };
            paymentInstrumentEntry.Properties = [paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3];

            await using (var entryMessageWriter = new ODataMessageWriter(batchChangesetOperation1))
            {
                var odataEntryWriter = await entryMessageWriter.CreateODataResourceWriterAsync(myPaymentInstrumentSet, paymentInstrumentType);
                await odataEntryWriter.WriteStartAsync(paymentInstrumentEntry);
                await odataEntryWriter.WriteEndAsync();
            }

            // Changeset end.
            await batchWriter.WriteEndChangesetAsync();

            // Another Get request.
            requestUrl = new Uri(_baseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)");
            var batchOperation2 = await batchWriter.CreateOperationRequestMessageAsync("GET", requestUrl, null, option);
            batchOperation2.SetHeader("Accept", "application/json;odata.metadata=full");

            // Batch end.
            await batchWriter.WriteEndBatchAsync();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        await ProcessBatchResponseAsync(responseMessage);
    }

    #region Private methods

    private async Task ProcessBatchResponseAsync(IODataResponseMessageAsync responseMessage)
    {
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        using (var innerMessageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var batchReader = await innerMessageReader.CreateODataBatchReaderAsync();
            int batchOperationId = 0;

            while (await batchReader.ReadAsync())
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
                        var operationResponse = await batchReader.CreateOperationResponseMessageAsync();

                        using (var operationResponseReader = new ODataMessageReader(operationResponse, readerSettings, _model))
                        {
                            if (batchOperationId == 0)
                            {
                                // the first response message is a feed
                                var feedReader = await operationResponseReader.CreateODataResourceSetReaderAsync();

                                Assert.Equal(200, operationResponse.StatusCode);

                                var pis = new List<ODataResource>();
                                while (await feedReader.ReadAsync())
                                {
                                    if (feedReader.State == ODataReaderState.ResourceEnd)
                                    {
                                        var entry = feedReader.Item as ODataResource;
                                        Assert.NotNull(entry);
                                        pis.Add(entry);
                                    }
                                }

                                Assert.Equal(ODataReaderState.Completed, feedReader.State);

                                Assert.Equal(3, pis.Count);
                            }
                            else if (batchOperationId == 1)
                            {
                                // the second response message is a creation response
                                var entryReader = await operationResponseReader.CreateODataResourceReaderAsync();

                                Assert.Equal(201, operationResponse.StatusCode);
                                
                                var pis = new List<ODataResource>();
                                while (await entryReader.ReadAsync())
                                {
                                    if (entryReader.State == ODataReaderState.ResourceEnd)
                                    {
                                        var entry = entryReader.Item as ODataResource;
                                        Assert.NotNull(entry);
                                        pis.Add(entry);
                                    }
                                }

                                Assert.Equal(ODataReaderState.Completed, entryReader.State);

                                Assert.Single(pis);
                                var paymentInstrumentIDProperty = pis[0].Properties.Single(p => p.Name == "PaymentInstrumentID") as ODataProperty;
                                Assert.NotNull(paymentInstrumentIDProperty);
                                Assert.Equal(102910, paymentInstrumentIDProperty.Value);
                            }
                            else if (batchOperationId == 2)
                            {
                                // the third response message is an entry
                                var entryReader = await operationResponseReader.CreateODataResourceReaderAsync();

                                Assert.Equal(200, operationResponse.StatusCode);

                                var statements = new List<ODataResource>();
                                while (await entryReader.ReadAsync())
                                {
                                    
                                    if (entryReader.State == ODataReaderState.ResourceEnd)
                                    {
                                        var entry = entryReader.Item as ODataResource;
                                        Assert.NotNull(entry);
                                        statements.Add(entry);
                                    }
                                }

                                Assert.Equal(ODataReaderState.Completed, entryReader.State);

                                Assert.Single(statements);
                                var statementIDProperty = statements[0].Properties.Single(p => p.Name == "StatementID") as ODataProperty;
                                Assert.NotNull(statementIDProperty);
                                Assert.Equal(103901001, statementIDProperty.Value);
                            }
                        }

                        batchOperationId++;
                        break;
                }
            }
            Assert.Equal(ODataBatchReaderState.Completed, batchReader.State);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "batchrequests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
