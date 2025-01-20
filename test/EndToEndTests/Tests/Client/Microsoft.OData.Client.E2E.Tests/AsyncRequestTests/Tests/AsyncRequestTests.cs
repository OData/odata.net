//-----------------------------------------------------------------------------
// <copyright file="AsyncRequestTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRequestTests.Tests;

public class AsyncRequestTests : EndToEndTestBase<AsyncRequestTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(AsyncRequestTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), batchHandler: new DefaultODataBatchHandler()));
        }
    }

    public AsyncRequestTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    // Constants
    private const string IncludeAnnotation = "odata.include-annotations";
    private const string NameSpacePrefix = "Microsoft.OData.Client.E2E.Tests.Common.Server.Default";
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task AsyncQueryRequestTest(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People", UriKind.Absolute);

        // Act & Assert
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
        requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
        var responseMessage = await requestMessage.GetResponseAsync();
        Assert.Equal(202, responseMessage.StatusCode);
        string monitorResource = responseMessage.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new TestHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync();
        Assert.Equal(202, statusCheckResponse1.StatusCode);
        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new TestHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync();
        Assert.Equal(200, statusCheckResponse2.StatusCode);

        using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
        {
            var asyncReader = messageReader.CreateODataAsynchronousReader();
            var innerMessage = asyncReader.CreateResponseMessage();

            Assert.Equal(200, innerMessage.StatusCode);
            using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, _model))
            {
                var entries = new List<ODataResource>();
                var feedReader = innerMessageReader.CreateODataResourceSetReader();

                while (feedReader.Read())
                {
                    if (feedReader.State == ODataReaderState.ResourceEnd)
                    {
                        var entry = feedReader.Item as ODataResource;
                        Assert.NotNull(entry);
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

                var entry0FirstName = entries[0].Properties.Single(p => p.Name == "FirstName") as ODataProperty;
                Assert.NotNull(entry0FirstName);
                Assert.Equal("Bob", entry0FirstName.Value);

                var entry4FirstName = entries[4].Properties.Single(p => p.Name == "FirstName") as ODataProperty;
                Assert.NotNull(entry4FirstName);
                Assert.Equal("Peter", entry4FirstName.Value);
            }
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task AsyncCreateRequestTest(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() 
        { 
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };

        #region send the Create request with respond-async preference

        var accountEntry = new ODataResourceWrapper()
        {
            Resource = new ODataResource
            {
                TypeName = $"{NameSpacePrefix}.Account",
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
                                    TypeName = $"{NameSpacePrefix}.AccountInfo",
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

        var settings = new ODataMessageWriterSettings()
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };

        var accountType = _model.FindDeclaredType($"{NameSpacePrefix}.Account") as IEdmEntityType;
        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

        // Act & Assert
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Accounts", UriKind.Absolute), Client);
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
        requestMessage.Method = "POST";
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
        {
            var odataWriter = messageWriter.CreateODataResourceWriter(accountSet, accountType);
            ODataWriterHelper.WriteResource(odataWriter, accountEntry);
        }

        var responseMessage = await requestMessage.GetResponseAsync();

        #endregion

        Assert.Equal(202, responseMessage.StatusCode);
        string monitorResource = responseMessage.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new TestHttpClientRequestMessage(new Uri(monitorResource, UriKind.Absolute), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync();
        Assert.Equal(202, statusCheckResponse1.StatusCode);
        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new TestHttpClientRequestMessage(new Uri(monitorResource), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync();
        Assert.Equal(200, statusCheckResponse2.StatusCode);

        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
            {
                var asyncReader = messageReader.CreateODataAsynchronousReader();
                var innerMessage = asyncReader.CreateResponseMessage();

                Assert.Equal(201, innerMessage.StatusCode);
                using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, _model))
                {
                    List<ODataResource> entries = new List<ODataResource>();
                    var entryReader = innerMessageReader.CreateODataResourceReader();

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

                    var entry1AccountID = entries[1].Properties.Single(p => p.Name == "AccountID") as ODataProperty;
                    Assert.NotNull(entry1AccountID);
                    Assert.Equal(110, entry1AccountID.Value);
                }

            }
        }
    }

    [Fact]
    public async Task AsyncBatchRequestTest()
    {
        // Arrange
        var writerSettings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

        #region send a batch request with respond-async preference

        var accountType = _model.FindDeclaredType($"{NameSpacePrefix}.Account") as IEdmEntityType;
        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
        var paymentInstrumentType = _model.FindDeclaredType($"{NameSpacePrefix}.PaymentInstrument") as IEdmEntityType;
        var navProp = accountType?.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
        var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

        // Act & Assert
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "$batch", UriKind.Absolute), Client);
        requestMessage.SetHeader("Content-Type", "multipart/mixed;boundary=batch_01AD6766-4A45-47CC-9463-94D4591D8DA9");
        requestMessage.SetHeader("OData-Version", "4.0");
        requestMessage.PreferHeader().RespondAsync = true; //Request the service to process asynchronously.
        requestMessage.Method = "POST";

        await using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var batchWriter = await messageWriter.CreateODataBatchWriterAsync();

            //Batch start.
            await batchWriter.WriteStartBatchAsync();

            //A Get request.
            var batchOperation1 = await batchWriter.CreateOperationRequestMessageAsync("GET", new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments"), null);
            batchOperation1.SetHeader("Accept", MimeTypeODataParameterFullMetadata);
            //Get request ends. 

            // Changeset start.
            await batchWriter.WriteStartChangesetAsync();

            //The first operation in changeset is a Create request.

            var batchChangesetOperation1 = await batchWriter.CreateOperationRequestMessageAsync("POST", new Uri(_baseUri + "Accounts(102)/MyPaymentInstruments", UriKind.Absolute), "1");
            batchChangesetOperation1.SetHeader("Content-Type", MimeTypeODataParameterFullMetadata);
            batchChangesetOperation1.SetHeader("Accept", MimeTypeODataParameterFullMetadata);

            var paymentInstrumentEntry = new ODataResource() { TypeName = $"{NameSpacePrefix}.PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 102910 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "102 batch new PI" };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57)) };
            paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

            await using (var entryMessageWriter = new ODataMessageWriter(batchChangesetOperation1))
            {
                var odataEntryWriter = await entryMessageWriter.CreateODataResourceWriterAsync(myPaymentInstrumentSet, paymentInstrumentType);
                await odataEntryWriter.WriteStartAsync(paymentInstrumentEntry);
                await odataEntryWriter.WriteEndAsync();
            }

            // Changeset end.
            await batchWriter.WriteEndChangesetAsync();

            //Another Get request.

            var batchOperation2 = await batchWriter.CreateOperationRequestMessageAsync("GET", new Uri(_baseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)", UriKind.Absolute), null);
            batchOperation2.SetHeader("Accept", MimeTypeODataParameterFullMetadata);

            // Batch end.
            await batchWriter.WriteEndBatchAsync();
        }

        var responseMessage = await requestMessage.GetResponseAsync();

        #endregion

        #region request the status monitor resource

        Assert.Equal(202, responseMessage.StatusCode);
        string monitorResource = responseMessage.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        var statusCheckRequest1 = new TestHttpClientRequestMessage(new Uri(monitorResource, UriKind.Absolute), Client);
        var statusCheckResponse1 = await statusCheckRequest1.GetResponseAsync();
        Assert.Equal(202, statusCheckResponse1.StatusCode);
        monitorResource = statusCheckResponse1.GetHeader("Location");
        Assert.False(string.IsNullOrWhiteSpace(monitorResource));

        //The request takes 5 seconds to finish in service, so we wait for 6 seconds. 
        Thread.Sleep(6000);
        var statusCheckRequest2 = new TestHttpClientRequestMessage(new Uri(monitorResource, UriKind.Absolute), Client);
        var statusCheckResponse2 = await statusCheckRequest2.GetResponseAsync();
        Assert.Equal(200, statusCheckResponse2.StatusCode);

        #endregion

        #region read and verify the response

        using (var messageReader = new ODataMessageReader(statusCheckResponse2, readerSettings, _model))
        {
            var asyncReader = messageReader.CreateODataAsynchronousReader();
            var innerMessage = asyncReader.CreateResponseMessage();

            Assert.Equal(200, innerMessage.StatusCode);
            using (var innerMessageReader = new ODataMessageReader(innerMessage, readerSettings, _model))
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

                                    var paymentInstrumentIDProp = pis[0].Properties.Single(p => p.Name == "PaymentInstrumentID") as ODataProperty;
                                    Assert.Equal(102910, paymentInstrumentIDProp?.Value);
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

                                    var statementIDProp = statements[0].Properties.Single(p => p.Name == "StatementID") as ODataProperty;
                                    Assert.Equal(103901001, statementIDProp?.Value);
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

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "asyncrequesttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
