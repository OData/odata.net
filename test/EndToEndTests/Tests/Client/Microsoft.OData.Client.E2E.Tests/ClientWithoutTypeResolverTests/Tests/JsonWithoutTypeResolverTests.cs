//-----------------------------------------------------------------------------
// <copyright file="JsonWithoutTypeResolverTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientWithoutTypeResolverTests.Tests
{
    public class JsonWithoutTypeResolverTests : EndToEndTestBase<JsonWithoutTypeResolverTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(JsonWithoutTypeResolverTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.EnableQueryFeatures()
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public JsonWithoutTypeResolverTests(TestWebApplicationFactory<TestsStartup> fixture)
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
        public async Task DerivedTypeFeedQuery_Executes_Successfully()
        {
            var productPageViews = (await _context.ExecuteAsync<Common.Clients.EndToEnd.PageView>(new Uri(_baseUri.OriginalString + "PageViews/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.ProductPageView"))).ToArray();
            var discontinuedProducts = (await _context.ExecuteAsync<Common.Clients.EndToEnd.DiscontinuedProduct>(new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct"))).ToArray();

            Assert.Equal(5, productPageViews.Length);

            foreach (var productPageView in productPageViews)
            {
                Assert.IsAssignableFrom<Common.Clients.EndToEnd.PageView>(productPageView);
            }

            Assert.Equal(5, discontinuedProducts.Length);

            foreach (var discontinuedProduct in discontinuedProducts)
            {
                Assert.IsAssignableFrom<Common.Clients.EndToEnd.Product>(discontinuedProduct);
            }
        }

        [Fact]
        public async Task SelectQuery_Executes_Successfully()
        {
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$select=Name,PrimaryContactInfo"))).Single();

            Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", queryResults.Name);
            Assert.NotNull(queryResults.PrimaryContactInfo);
        }

        [Fact]
        public async Task ExpandEntryQuery_ExecutesSuccessfully()
        {
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info"))).Single();

            Assert.Equal(-9, queryResults.CustomerId);
            Assert.NotNull(queryResults.Info);
        }

        [Fact]
        public async Task ExpandEntryQueryWithNestedSelect_WorksCorrectly()
        {
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info($select=Information)"))).Single();

            Assert.Equal(-9, queryResults.CustomerId);
            Assert.NotNull(queryResults.Info);
            
            Assert.Equal("frubhbngipuuveyneosslslbtrßqjujnssgcxuuzdbeußeaductgqbvhpussktbzzfuqvkxajzckmkzluthcjsku", queryResults.Info.Information);
        }

        [Fact]
        public async Task DerivedTypeExpandWithProjectionFeedQuery_ExecutesSuccessfully()
        {
            var queryUri = new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct?$expand=RelatedProducts($select=*),Detail($select=*),Reviews($select=*),Photos($select=*)");
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.DiscontinuedProduct>(queryUri)).ToArray();

            //Get one of the products and check the expanded navigation props are not null. 
            var productWithId9 = queryResults.Single(a => a.ProductId == -9);

            Assert.NotNull(productWithId9.RelatedProducts);
            Assert.NotNull(productWithId9.Detail);
            Assert.NotNull(productWithId9.Reviews);
            Assert.NotNull(productWithId9.Photos);
        }

        [Fact]
        public async Task QueryDerivedTypeProductId_ReturnsExpectedProductId()
        {
            var queryResults = (await _context.ExecuteAsync<int>(new Uri(_baseUri.OriginalString + "Products(-9)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/ProductId"))).Single();

            // Assert that the queryResults matches the expected ProductId
            Assert.Equal(-9, queryResults);
        }

        [Fact]
        public async Task ComplexPropertyQuery_ReturnsExpectedContactDetails()
        {
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.ContactDetails>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo"))).Single();
            
            Assert.NotNull(queryResults);
            Assert.NotNull(queryResults.HomePhone);
        }

        [Fact]
        public async Task NestedComplexPropertyQuery_ReturnsExpectedResults()
        {
            var queryResults1 = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag"))).ToArray();
            var queryResults2 = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Aliases>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias"))).ToArray();
            var queryResults3 = (await _context.ExecuteAsync<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias/AlternativeNames"))).ToArray();
            var queryResults4 = (await _context.ExecuteAsync<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames"))).ToArray();

            Assert.Equal(10, queryResults1.Length);

            var phone = queryResults1.First();
            Assert.Equal("essfchpbmodumdlbssaoygvcecnegßumuvszyo", phone.PhoneNumber);
            Assert.Equal("ilvxmcmkixinhonuxeqfcbsnlgufneqhijddgurdkuvvj", phone.Extension);

            Assert.Single(queryResults2);
            Assert.Single(queryResults3);

            var alternativeNames = queryResults3.First();
            Assert.Equal("ゼポソソァんマａグぴ九縷亜ぞゼソグバぼダぽママぽポチボソぼぜゾんミぴほダミミ畚珱九ｚべ弌畚タソｚゼソぁび裹ァソマｦひ匚亜ポべポぽマゼたチ裹歹ミポ", alternativeNames.First());
            Assert.Single(queryResults4);
        }

        [Fact]
        public async Task CollectionOfComplexPropertyQuery_ExecutesSuccessfully()
        {
            var queryResults = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag"))).ToArray();

            Assert.Equal(10, queryResults.Length);
        }

        [Fact]
        public async Task CollectionOfPrimitivePropertyQuery_ExecutesSuccessfully()
        {
            var queryResults = (await _context.ExecuteAsync<ICollection<decimal>>(new Uri(_baseUri.OriginalString + "MappedEntityTypes(-10)/BagOfDecimals"))).ToArray();

            Assert.Single(queryResults);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "jsonwithouttyperesolver/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
