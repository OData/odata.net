//-----------------------------------------------------------------------------
// <copyright file="ContainmentTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Containment;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.ContainmentTests
{
    public class ContainmentTests : EndToEndTestBase<ContainmentTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;
        private const string TestModelNameSpace = "Microsoft.OData.E2E.TestCommon.Common.Server.Default";

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
                var odataWriter = await messageWriter.CreateODataParameterWriterAsync(null);

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
            ODataResource entry = await QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101904)") as ODataResource;
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
            ODataResource deletedEntry = await QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101904)", 404) as ODataResource;
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
            ODataResource entry = await QueryEntityItemAsync("Accounts(104)/MyGiftCard") as ODataResource;
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
            ODataResource entry = await QueryEntityItemAsync("Accounts(101)/MyPaymentInstruments(101903)") as ODataResource;
            Assert.Equal(101903, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PaymentInstrumentID").Value);
            Assert.Equal(mimeType, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FriendlyName").Value);
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
