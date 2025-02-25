//-----------------------------------------------------------------------------
// <copyright file="PayloadValueConverterTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Client.Default;
using Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Converters;
using Microsoft.OData.Edm;
using Xunit;
using ClientPayloadConverterModel = Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Client;

namespace Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests
{
    public class PayloadValueConverterTests : EndToEndTestBase<PayloadValueConverterTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(PayloadValueConverterController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents(
                    "odata",
                    PayloadValueConverterEdmModel.GetEdmModel(),
                    a => a.AddSingleton<ODataPayloadValueConverter, BinaryPayloadConverter>()));
            }
        }

        public PayloadValueConverterTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDataSource();
        }

        [Fact]
        public async Task PostingAndQueryingABinaryProperty_UsingCustomBinaryToHyphenStringConverter_ShouldReturnHyphenSeparatedString()
        {
            var person = new ClientPayloadConverterModel.Person
            {
                Picture = new byte[] { 3, 1, 4 },
                BusinessCard = new ClientPayloadConverterModel.ContactInfo()
                {
                    N = "T"
                }
            };

            _context.AddToPeople(person);
            await _context.SaveChangesAsync();

            var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(" + person.Id + ")/Picture", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "GET"
            };

            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            var dat = new StreamReader(await responseMessage.GetStreamAsync()).ReadToEnd();

            Assert.Contains("\"value\":\"3-1-4\"", dat);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "payloadvalueconverter/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
