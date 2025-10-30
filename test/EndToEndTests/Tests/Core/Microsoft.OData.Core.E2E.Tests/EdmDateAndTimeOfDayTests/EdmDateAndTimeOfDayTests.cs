//-----------------------------------------------------------------------------
// <copyright file="EdmDateAndTimeOfDayTests.cs" company=".NET Foundation">
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
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EdmDateAndTimeOfDay;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.EdmDateAndTimeOfDayTests
{
    public class EdmDateAndTimeOfDayTests : EndToEndTestBase<EdmDateAndTimeOfDayTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(EdmDateAndTimeOfDayTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public EdmDateAndTimeOfDayTests(TestWebApplicationFactory<TestsStartup> fixture)
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

        #region Query/Action/Function
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAnEntityThatContainsDateAndTimeOfDay_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                {
                    var reader = await messageReader.CreateODataResourceReaderAsync();
                    ODataResource entry = null;
                    while (await reader.ReadAsync())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            entry = reader.Item as ODataResource;
                        }
                    }

                    // Verify Date Property
                    Assert.Equal(new DateOnly(2014, 8, 31), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipDate").Value);
                    Assert.Equal(new TimeOnly(12, 40, 5, 50), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipTime").Value);
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingTopLevelDateProperies_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/ShipDate", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                ODataProperty property = await messageReader.ReadPropertyAsync();
                Assert.Equal(new DateOnly(2014, 8, 31), property.Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingTopLevelTimeOfDayProperies_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/ShipTime", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                ODataProperty property = await messageReader.ReadPropertyAsync();
                Assert.Equal(new TimeOnly(12, 40, 5, 50), property.Value);
            }
        }

        [Fact]
        public async Task QueryingRawDateAndTimeOfDayValues_WorkCorrectly()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/ShipDate/$value", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
            {
                var date = await messageReader.ReadValueAsync(EdmCoreModel.Instance.GetDate(false));
                Assert.Equal(new DateOnly(2014, 8, 31), date);
            }

            var todRequestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/ShipTime/$value", UriKind.Absolute);

            var requestMessage2 = new TestHttpClientRequestMessage(todRequestUrl, Client);
            var responseMessage2 = await requestMessage2.GetResponseAsync();

            Assert.Equal(200, responseMessage2.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage2, readerSettings, _model))
            {
                var date = await messageReader.ReadValueAsync(EdmCoreModel.Instance.GetTimeOfDay(false));
                Assert.Equal(new TimeOnly(12, 40, 5, 50), date);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingWithDateFilter_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders?$filter=ShipDate eq 2014-08-31", UriKind.Absolute);

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
                        if (entry != null && entry.TypeName.EndsWith("Order"))
                        {
                            // Verify Date Property
                            Assert.Equal(new DateOnly(2014, 8, 31), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipDate").Value);
                            Assert.Equal(new TimeOnly(12, 40, 5, 50), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipTime").Value);
                        }
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingWithTimeFilter_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders?$filter=ShipTime eq 12:40:5.05", UriKind.Absolute);

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

                        if (entry != null)
                        {
                            // Verify Date Property
                            Assert.Equal(new DateOnly(2014, 8, 31), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipDate").Value);
                            Assert.Equal(new TimeOnly(12, 40, 5, 50), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipTime").Value);
                        }
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Fact]
        public async Task InvokingAFunctionCorrectly_ReturnDates()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.GetShipDate()", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var date = (await messageReader.ReadPropertyAsync()).Value;
            Assert.Equal(new DateOnly(2014, 8, 31), date);
        }

        [Fact]
        public async Task InvokingAFunctionWithDateParameter_WorksCorrectly()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.CheckShipDate(date = 2014-08-31)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var result = (await messageReader.ReadPropertyAsync()).Value;
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task InvokingAFunctionWithTimeReturnType_CorrectlyWorks()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.GetShipTime", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var time = (await messageReader.ReadPropertyAsync()).Value;

            Assert.Equal(new TimeOnly(12, 40, 5, 50), time);
        }

        [Fact]
        public async Task InvokingAFunctionWithATimeParameter_WorksCorrectly()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Orders(7)/Default.CheckShipTime(time = 12:40:5.5)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var result = (await messageReader.ReadPropertyAsync()).Value;

            Assert.Equal(false, result);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingByDateKey_WorksCorrectly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "Calendars(2015-11-11)", UriKind.Absolute);

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
                        // Verify Date Property
                        Assert.Equal(new DateOnly(2015, 11, 11), entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "Day").Value);
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task InvokingAnActionWithDateAndTimeParameters_WorksCorrectly(string mimeType)
        {
            var writerSettings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var readerSettings = new ODataMessageReaderSettings
            {
                BaseUri = _baseUri
            };

            var requestUrl = new Uri(_baseUri + "Orders(7)/Default.ChangeShipTimeAndDate");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            DateOnly newDate = DateOnly.MinValue;
            TimeOnly newTime = TimeOnly.MinValue;

            await using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
            {
                var odataWriter = await messageWriter.CreateODataParameterWriterAsync(null);
                await odataWriter.WriteStartAsync();
                await odataWriter.WriteValueAsync("date", newDate);
                await odataWriter.WriteValueAsync("time", newTime);
                await odataWriter.WriteEndAsync();
            }

            // send the http request
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

                        if (entry != null)
                        {
                            Assert.Equal(DateOnly.MinValue, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipDate").Value);
                            Assert.Equal(TimeOnly.MinValue, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShipTime").Value);
                        }
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        #endregion

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "edmdateandtimeofday/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
