//-----------------------------------------------------------------------------
// <copyright file="TransportLayerErrorTests.cs" company=".NET Foundation">
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
    public class TransportLayerErrorTests : EndToEndTestBase<TransportLayerErrorTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(TransportLayerErrorController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public TransportLayerErrorTests(TestWebApplicationFactory<TestsStartup> fixture)
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
        public async Task QueryWithInvalidUri_ShouldThrowADataServiException()
        {
            var exception = await Assert.ThrowsAsync<DataServiceQueryException>(async () =>
                await _context.ExecuteAsync<ClientEndToEndModel.Product>(new Uri("http://var1.svc/Products"))
            );

            Assert.Equal(404, exception.Response.StatusCode);
        }

        [Fact]
        public void QueryForEntryWithInvalidKey_ShouldReturnEmptyResult()
        {
            var query = _context.MessageAttachments.Where(ma => ma.AttachmentId == Guid.NewGuid()).ToList();
            Assert.Empty(query);
        }

        [Fact]
        public async Task JsonQueryWithInvalidDataServiceVersion_DoesNotThrowException_ItReturnsTheExpectedResults()
        {
            _context.SendingRequest2 += (sender, e) =>
            {
                e.RequestMessage.SetHeader("DataServiceVersion", "99.99;NetFx");
            };

            _context.Format.UseJson();

            var exception = await Record.ExceptionAsync(async () =>
            {
                var result = await _context.Customers.ExecuteAsync();
                Assert.Equal(10, result.Count());
            });

            Assert.Null(exception);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "transportlayererror/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
