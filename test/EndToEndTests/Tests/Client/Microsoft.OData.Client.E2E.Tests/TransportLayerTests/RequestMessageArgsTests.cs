//-----------------------------------------------------------------------------
// <copyright file="RequestMessageArgsTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.TransportLayerTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using ClientEndToEndModel = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.TransportLayerTests
{
    public class RequestMessageArgsTests : EndToEndTestBase<RequestMessageArgsTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(RequestMessageArgsTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public RequestMessageArgsTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDataSource();
        }

        [Fact]
        public async Task AddingCustomHeaderToRequest_ShouldSucceed()
        {
            string headerName = "MyNewHeader";
            string headerValue = "MyNewHeaderValue";
            IODataRequestMessage lastRequestMessage = null;

            Func<DataServiceClientRequestMessageArgs, DataServiceClientRequestMessage> configureRequest =
                args =>
                {
                    args.Headers.Add(headerName, headerValue);
                    return new HttpClientRequestMessage(args);
                };

            _context.SendingRequest2 += (obj, args) => lastRequestMessage = args.RequestMessage;
            _context.Configurations.RequestPipeline.OnMessageCreating = configureRequest;

            DataServiceQuery<ClientEndToEndModel.Product> query = _context.Products;
            await query.ExecuteAsync();

            Assert.NotNull(lastRequestMessage);
            var header = lastRequestMessage.Headers.SingleOrDefault(h => h.Key == headerName);
            Assert.Equal(headerValue, header.Value);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "requestmessageargs/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
