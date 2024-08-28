//-----------------------------------------------------------------------------
// <copyright file="ContainmentTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.ContainmentTest.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ContainmentTest.Tests
{
    public class ContainmentTests : EndToEndTestBase<ContainmentTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;
        private const string TestModelNameSpace = "Microsoft.OData.Client.E2E.Tests.Common.Server.Default";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ContainmentTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public ContainmentTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        public static IEnumerable<object[]> MimeTypesData
        {
            get
            {
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
            }
        }
        #region Query

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAContainedEntity_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyGiftCard", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await  requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.Equal(301, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "GiftCardID").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Fact]
        public async Task CallingAFunctionBoundToAContainedEntity_WorksCorrectly()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri +
                "Accounts(101)/MyGiftCard/Default.GetActualAmount(bonusRate=0.2)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var amount = (await messageReader.ReadPropertyAsync()).Value;

            Assert.Equal(23.88, amount);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task CallingAFunctionThatReturnsAContainedEntity_WorksCorrecty(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(101)/Default.GetDefaultPI()", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                List<ODataResource> entries = [];
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry);
                        entries.Add(entry);
                    }
                }

                Assert.Equal(101901, entries.Single().Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task InvokeActionReturnsContainedEntity(string mimeType)
        {
            var writerSettings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var readerSettings = new ODataMessageReaderSettings
            {
                BaseUri = _baseUri,
            };

            var requestUrl = new Uri(_baseUri + "Accounts(101)/Default.RefreshDefaultPI");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            DateTimeOffset newDate = new(DateTime.Now);

            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
            {
                var odataWriter = await messageWriter.CreateODataParameterWriterAsync((IEdmOperation)null);

                await odataWriter.WriteStartAsync();
                await odataWriter.WriteValueAsync("newDate", newDate);
                await odataWriter.WriteEndAsync();
            }

            // send the http request
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();
                List<ODataResource> entries = [];

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        entries.Add(entry);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Equal(101901, entries.Single().Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal(newDate, entries.Single().Properties.OfType<ODataProperty>().Single(p => p.Name == "CreatedDate").Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAContainedEntitySet_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            Uri requestUrl = new(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var entries = new List<ODataResource>();
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;

                        Assert.NotNull(entry);
                        entries.Add(entry);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Equal(4, entries.Count);

                Assert.Equal(103905, entries[2].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal("103 new PI", entries[2].Properties.OfType<ODataProperty>().Single(p => p.Name == "FriendlyName").Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingASpecificEntityInAContainedEntitySet_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103902)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = messageReader.CreateODataResourceReader();

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.Equal("103 second PI", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FriendlyName").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingALevel2NestedContainedEntity_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.Equal("Digital goods: App", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "TransactionDescription").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingALevel2NestedContainedEntitySet_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "StatementID").Value);
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "TransactionType").Value);
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "TransactionDescription").Value);
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "Amount").Value);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingALeve2NestedNonContainedEntity_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/TheStoredPI", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.Equal(802, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "StoredPIID").Value);
                        Assert.Equal("AliPay", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PIType").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingALevel2NestedSingleton_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)/BackupStoredPI", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                List<ODataResource> entries = new List<ODataResource>();
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry);
                        entries.Add(entry);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Single(entries);

                Assert.Equal(800, entries[0].Properties.OfType<ODataProperty>().Single(p => p.Name == "StoredPIID").Value);
                Assert.Equal("The Default Stored PI", entries[0].Properties.OfType<ODataProperty>().Single(p => p.Name == "PIName").Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryContainedEntityWithDerivedTypeCast(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments(101902)/{0}.CreditCardPI", TestModelNameSpace), UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.Equal(101902, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAContainedEntitySetWithDerivedTypeCast_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments/{0}.CreditCardPI", TestModelNameSpace), UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingContainedEntitiesInADerivedTypeEntity_WorkCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments(101902)/{0}.CreditCardPI/CreditRecords", TestModelNameSpace), UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "CreditRecordID").Value);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAnEntityThatContainsAContainmentSet_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(101)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();
                ODataResource entry = null;

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = reader.Item as ODataResource;
                    }
                }

                Assert.Equal(101, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "AccountID").Value);
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAFeedContainingAContainmentSet_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();
                ODataResourceSet feed = null;

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        if (entry != null && entry.TypeName.EndsWith("Account"))
                        {
                            Assert.NotNull(entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "AccountID").Value);
                        }
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        feed = reader.Item as ODataResourceSet;
                    }
                }

                Assert.NotNull(feed);
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryIndividualPropertyOfContainedEntity(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101902)/PaymentInstrumentID", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                ODataProperty paymentInstrumentId = await messageReader.ReadPropertyAsync();
                Assert.Equal(101902, paymentInstrumentId.Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingContainedEntitiesWithAFilter_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments?$filter=PaymentInstrumentID gt 103901", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                List<ODataResource> entries = [];
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;
                        Assert.NotNull(entry);
                        entries.Add(entry);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Equal(2, entries.Count);
                Assert.Equal(103902, entries[0].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal(103905, entries[1].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingContainedEntitiesWithOrderby_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments?$orderby=CreatedDate", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                List<ODataResource> entries = [];
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource entry = reader.Item as ODataResource;

                        Assert.NotNull(entry);
                        entries.Add(entry);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        Assert.NotNull(reader.Item as ODataResourceSet);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Equal(4, entries.Count);
                Assert.Equal(103902, entries[0].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal(101910, entries[1].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal(103901, entries[2].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.Equal(103905, entries[3].Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
            }
        }

        [Theory]
        [MemberData(nameof(SelectQueryTestData))]
        public async Task QueryingAContainedEntityWithASelectOption_WorksCorrectly(string query, int expectedPropertyCount, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + query, UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                List<ODataResource> entries = [];
                List<ODataNestedResourceInfo> navigationLinks = [];

                var reader = await messageReader.CreateODataResourceReaderAsync();
                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entries.Add(reader.Item as ODataResource);
                    }
                    else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        navigationLinks.Add(reader.Item as ODataNestedResourceInfo);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
                Assert.Single(entries);
                Assert.Empty(navigationLinks);
                Assert.Equal(expectedPropertyCount, entries[0].Properties.Count());
            }
        }
        #endregion

        #region Create/Update/Delete
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task CreatingAndDeletingAContainmentEntity_WorksCorrectly(string mimeType)
        {
            // create entry and insert
            var paymentInstrumentEntry = new ODataResource() { TypeName = TestModelNameSpace + ".PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 101904 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "101 new PI" };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
            paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var accountType = _model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = _model.FindDeclaredType(TestModelNameSpace + ".PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            var requestUrl = new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(myPaymentInstrumentSet, paymentInstrumentType);
                await odataWriter.WriteStartAsync(paymentInstrumentEntry);
                await odataWriter.WriteEndAsync();
            }

            // send the http request
            var responseMessage = await requestMessage.GetResponseAsync();

            // verify the create
            Assert.Equal(201, responseMessage.StatusCode);
            ODataResource entry = await this.QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101904)") as ODataResource;
            Assert.Equal(101904, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);

            // delete the entry
            var deleteRequestUrl = new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments(101904)");

            var deleteRequestMessage = new TestHttpClientRequestMessage(deleteRequestUrl, Client)
            {
                Method = "DELETE"
            };

            var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);
            ODataResource deletedEntry = await this.QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101904)", 404) as ODataResource;
            Assert.Null(deletedEntry);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task CreatingASingleValuedContainedEntity_WorksCorrectly(string mimeType)
        {
            // create entry and insert
            var giftCardEntry = new ODataResource() { TypeName = TestModelNameSpace + ".GiftCard" };
            var giftCardEntryP1 = new ODataProperty { Name = "GiftCardID", Value = 304 };
            var giftCardEntryP2 = new ODataProperty { Name = "GiftCardNO", Value = "AAGS993A" };
            var giftCardEntryP3 = new ODataProperty { Name = "ExperationDate", Value = new DateTimeOffset(new DateTime(2013, 12, 30)) };
            var giftCardEntryP4 = new ODataProperty { Name = "Amount", Value = 37.0 };
            giftCardEntry.Properties = [giftCardEntryP1, giftCardEntryP2, giftCardEntryP3, giftCardEntryP4];

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var accountType = _model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
            var giftCardType = _model.FindDeclaredType(TestModelNameSpace + ".GiftCard") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyGiftCard") as IEdmNavigationProperty;
            var myGiftCardSet = accountSet.FindNavigationTarget(navProp);

            var requestUrl = new Uri(_baseUri + "Accounts(104)/MyGiftCard");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client)
            {
                // Use PATCH to upsert
                Method = "PATCH"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);
            
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(myGiftCardSet, giftCardType);
                await odataWriter.WriteStartAsync(giftCardEntry);
                await odataWriter.WriteEndAsync();
            }

            // send the http request
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(204, responseMessage.StatusCode);
            ODataResource entry = await this.QueryEntityItemAsync("Accounts(104)/MyGiftCard") as ODataResource;
            Assert.Equal(304, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "GiftCardID").Value);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task UpdatingAContainedEntity_WorksCorrectly(string mimeType)
        {
            // create entry and insert
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var accountType = _model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = _model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = _model.FindDeclaredType(TestModelNameSpace + ".PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            var paymentInstrumentEntry = new ODataResource() { TypeName = TestModelNameSpace + ".PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 101903 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = mimeType };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
            paymentInstrumentEntry.Properties = [paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3];

            var requestUrl = new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments(101903)");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client)
            {
                Method = "PATCH"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(myPaymentInstrumentSet, paymentInstrumentType);
                await odataWriter.WriteStartAsync(paymentInstrumentEntry);
                await odataWriter.WriteEndAsync();
            }

            // send the http request
            var responseMessage = await requestMessage.GetResponseAsync();

            // verify the create
            Assert.Equal(204, responseMessage.StatusCode);
            ODataResource entry = await this.QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101903)") as ODataResource;
            Assert.Equal(101903, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
            Assert.Equal(mimeType, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FriendlyName").Value);
        }

        #endregion

        #region OData Client test cases

        [Fact]
        public async Task QueryingAContainedEntityFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<Common.Client.Default.GiftCard>("Accounts(101)/MyGiftCard");
            Assert.EndsWith("Accounts(101)/MyGiftCard", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<Common.Client.Default.GiftCard> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(301, result[0].GiftCardID);
            Assert.Equal("AAA123A", result[0].GiftCardNO);
        }

        [Fact]
        public async Task QueryingAContainedEntitySetFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<Common.Client.Default.PaymentInstrument> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Equal(4, result.Count);
            Assert.Equal(103902, result[1].PaymentInstrumentID);
            Assert.Equal("103 second PI", result[1].FriendlyName);
        }

        [Fact]
        public async Task QueryingASpecificEntityInAContainedEntitySetFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments(103902)");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103902)", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<Common.Client.Default.PaymentInstrument> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(103902, result[0].PaymentInstrumentID);
            Assert.Equal("103 second PI", result[0].FriendlyName);
        }

        [Fact]
        public async Task QueryingAnIndividualPropertyOfAContainedEntityFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            var queryable = _context.CreateQuery<int>("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<int> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(103902, result[0]);
        }

        [Fact]
        public async Task LinqUriTranslationTest_WorkCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            //translate to key
            var q1 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901);
            Common.Client.Default.PaymentInstrument q1Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q1).ExecuteAsync()).Single();
            Assert.Equal(103901, q1Result.PaymentInstrumentID);

            //$filter
            var q2 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.CreatedDate > new DateTimeOffset(new DateTime(2013, 10, 1)));
            Common.Client.Default.PaymentInstrument q2Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q2).ExecuteAsync()).Single();
            Assert.Equal(103905, q2Result.PaymentInstrumentID);

            //$orderby
            var q3 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").OrderBy(pi => pi.CreatedDate).ThenByDescending(pi => pi.FriendlyName);
            List<Common.Client.Default.PaymentInstrument> q3Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q3).ExecuteAsync()).ToList();
            Assert.Equal(103902, q3Result[0].PaymentInstrumentID);

            //$expand
            var q4 = _context.Accounts.Expand(account => account.MyPaymentInstruments).Where(account => account.AccountID == 103);
            Common.Client.Default.Account q4Result = (await ((DataServiceQuery<Common.Client.Default.Account>)q4).ExecuteAsync()).Single();
            Assert.NotNull(q4Result.MyPaymentInstruments);

            var q5 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Expand(pi => pi.BillingStatements).Where(pi => pi.PaymentInstrumentID == 103901);
            Common.Client.Default.PaymentInstrument q5Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q5).ExecuteAsync()).Single();
            Assert.NotNull(q5Result.BillingStatements);

            //$top
            var q6 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Take(1);
            var q6Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q6).ExecuteAsync()).ToList();

            //$count
            var q7 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Count();

            //$count=true
            var q8 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").IncludeCount();
            var q8Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q8).ExecuteAsync()).ToList();

            //projection
            var q9 = _context.Accounts.Where(a => a.AccountID == 103).Select(a => new Common.Client.Default.Account() { AccountID = a.AccountID, MyGiftCard = a.MyGiftCard });
            var q9Result = (await ((DataServiceQuery<Common.Client.Default.Account>)q9).ExecuteAsync()).Single();
            Assert.NotNull(q9Result.MyGiftCard);

            var q10 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901).Select(p => new Common.Client.Default.PaymentInstrument()
            {
                PaymentInstrumentID = p.PaymentInstrumentID,
                BillingStatements = p.BillingStatements
            });
            var q10Result = (await ((DataServiceQuery<Common.Client.Default.PaymentInstrument>)q10).ExecuteAsync()).ToList();
        }

        [Fact]
        public async Task CallingFunctionFromODataClientThatReturnsAContainedEntity_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            Common.Client.Default.PaymentInstrument result = (await _context.ExecuteAsync<Common.Client.Default.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri +
                "Accounts(101)/Default.GetDefaultPI()", UriKind.Absolute), "GET", true)).Single();
            Assert.Equal(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            _context.UpdateObject(result);
            await _context.SaveChangesAsync();

            result = (await _context.ExecuteAsync<Common.Client.Default.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET", true)).Single();
            Assert.Equal("Random Name", result.FriendlyName);
        }

        [Fact]
        public async Task InvokingAnActionFromODataClientThatReturnsAContainedEntity_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            Common.Client.Default.PaymentInstrument result =(await _context.ExecuteAsync<Common.Client.Default.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri +
                "Accounts(101)/Default.RefreshDefaultPI", UriKind.Absolute), "POST", true,
                new BodyOperationParameter("newDate", new DateTimeOffset(DateTime.Now)))).Single();
            Assert.Equal(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            _context.UpdateObject(result);
            await _context.SaveChangesAsync();

            result = (await _context.ExecuteAsync<Common.Client.Default.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET")).Single();
            Assert.Equal("Random Name", result.FriendlyName);
        }

        [Fact]
        public async Task CreatingAContainedEntityFromODataClientUsingAddRelatedObject_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            Common.Client.Default.Account newAccount = new Common.Client.Default.Account()
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new Common.Client.Default.AccountInfo()
                {
                    FirstName = "New",
                    LastName = "Guy"
                }
            };
            Common.Client.Default.PaymentInstrument newPI = new Common.Client.Default.PaymentInstrument()
            {
                PaymentInstrumentID = 110901,
                FriendlyName = "110's first PI",
                CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 10))
            };

            _context.AddToAccounts(newAccount);
            _context.AddRelatedObject(newAccount, "MyPaymentInstruments", newPI);
            await _context.SaveChangesAsync();

            var queryable0 = _context.Accounts.ByKey(110);
            Common.Client.Default.Account accountResult = await queryable0.GetValueAsync();
            Assert.Equal("Guy", accountResult.AccountInfo.LastName);

            var queryable1 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(110)/MyPaymentInstruments").ByKey(110901);
            Common.Client.Default.PaymentInstrument piResult = await queryable1.GetValueAsync();
            Assert.Equal("110's first PI", piResult.FriendlyName);
        }

        [Fact]
        public async Task UpdatingAContainedEntityFromODataClientUsingUpdateObject_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            // Get a contained PI entity
            var queryable1 = _context.CreateQuery<Common.Client.Default.PaymentInstrument>("Accounts(101)/MyPaymentInstruments").ByKey(101901);
            Common.Client.Default.PaymentInstrument piResult = await queryable1.GetValueAsync();

            piResult.FriendlyName = "Michael's first PI";
            _context.UpdateObject(piResult);
            await _context.SaveChangesAsync();

            piResult = await queryable1.GetValueAsync();
            Assert.Equal("Michael's first PI", piResult.FriendlyName);
        }

        [Fact]
        public async Task CreateContainedNonCollectionEntityFromODataClientUsingUpdateRelatedObject()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            Common.Client.Default.Account newAccount = new Common.Client.Default.Account()
            {
                AccountID = 120,
                CountryRegion = "GB",
                AccountInfo = new Common.Client.Default.AccountInfo()
                {
                    FirstName = "Diana",
                    LastName = "Spencer"
                }
            };

            Common.Client.Default.GiftCard giftCard = new Common.Client.Default.GiftCard()
            {
                GiftCardID = 320,
                GiftCardNO = "XX120ABCDE",
                Amount = 76,
                ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
            };

            _context.AddToAccounts(newAccount);
            _context.UpdateRelatedObject(newAccount, "MyGiftCard", giftCard);
            await _context.SaveChangesAsync();

            var queryable1 = _context.CreateQuery<Common.Client.Default.GiftCard>("Accounts(120)/MyGiftCard");
            List<Common.Client.Default.GiftCard> giftCardResult = (await ((DataServiceQuery<Common.Client.Default.GiftCard>)queryable1).ExecuteAsync()).ToList();

            Assert.Single(giftCardResult);
            Assert.Equal(76, giftCardResult[0].Amount);
        }
        #endregion

        public static IEnumerable<object[]> SelectQueryTestData()
        {
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=full" };
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=minimal" };
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=none" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=full" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=minimal" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=none" };
        }

        private async Task<ODataItem> QueryEntityItemAsync(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new()
            { 
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute);

            var queryRequestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = await queryRequestMessage.GetResponseAsync();

            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        item = reader.Item;
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }

            return item;
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "containmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
