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
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void DerivedTypeFeedQuery_Executes_Successfully()
        {
            var baseQueryResults = _context.Execute<Common.Clients.EndToEnd.PageView>(new Uri(_baseUri.OriginalString + "PageViews/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.ProductPageView")).ToArray();

            var derivedQueryResults = _context.Execute<Common.Clients.EndToEnd.DiscontinuedProduct>(new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")).ToArray();

            Assert.NotNull(baseQueryResults); // Verify that baseQueryResults is not null
            Assert.NotEmpty(baseQueryResults); // Verify that baseQueryResults is not empty

            Assert.NotNull(derivedQueryResults); // Verify that derivedQueryResults is not null
            Assert.NotEmpty(derivedQueryResults); // Verify that derivedQueryResults is not empty
        }

        [Fact]
        public void SelectQuery_Executes_Successfully()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$select=Name,PrimaryContactInfo")).ToArray();

            Assert.NotNull(queryResults); // Verify that queryResults is not null
            Assert.NotEmpty(queryResults); // Verify that queryResults is not empty
        }

        [Fact]
        public void ExpandEntryQuery_ExecutesSuccessfully()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info")).ToArray();

            Assert.NotNull(queryResults);
            Assert.NotEmpty(queryResults);

            Assert.NotNull(queryResults.SingleOrDefault().Info);
        }

        [Fact]
        public void ExpandEntryQueryWithNestedSelect_WorksCorrectly()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info($select=Information)")).ToArray();
            Assert.NotNull(queryResults);
            Assert.NotEmpty(queryResults);

            Assert.NotNull(queryResults.SingleOrDefault().Info.Information);
        }

        [Fact]
        public void DerivedTypeExpandWithProjectionFeedQuery_ExecutesSuccessfully()
        {
            var queryUri = new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct?$expand=RelatedProducts($select=*),Detail($select=*),Reviews($select=*),Photos($select=*)");
            var queryResults = _context.Execute<Common.Clients.EndToEnd.DiscontinuedProduct>(queryUri).ToArray();

            Assert.NotNull(queryResults);
            Assert.NotEmpty(queryResults);

            //Get one of the products and check the expanded navigation props are not null. 
            var productWithId9 = queryResults.SingleOrDefault(a=>a.ProductId==-9);

            Assert.NotNull(productWithId9.RelatedProducts);
            Assert.NotNull(productWithId9.Detail);
            Assert.NotNull(productWithId9.Reviews);
            Assert.NotNull(productWithId9.Photos);
        }

        [Fact]
        public void QueryDerivedTypeProductId_ReturnsExpectedProductId()
        {
            var queryResults = _context.Execute<int>(new Uri(_baseUri.OriginalString + "Products(-9)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/ProductId")).ToArray();

            // Assert that the queryResults is not null
            Assert.NotNull(queryResults);

            // Assert that the queryResults contains at least one item
            Assert.NotEmpty(queryResults);

            // Assert that the first item in queryResults matches the expected ProductId
            Assert.Equal(-9, queryResults.First());
        }

        [Fact]
        public void ComplexPropertyQuery_ReturnsExpectedContactDetails()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.ContactDetails>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo")).ToArray();

            Assert.NotNull(queryResults);
            Assert.NotEmpty(queryResults);

            var contactDetails = queryResults.First();
            Assert.NotNull(contactDetails);
        }

        [Fact]
        public void NestedComplexPropertyQuery_ReturnsExpectedResults()
        {
            var queryResults1 = _context.Execute<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
            var queryResults2 = _context.Execute<Common.Clients.EndToEnd.Aliases>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias")).ToArray();
            var queryResults3 = _context.Execute<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
            var queryResults4 = _context.Execute<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();

            // Assert that queryResults1 is not null and not empty
            Assert.NotNull(queryResults1);
            Assert.NotEmpty(queryResults1);

            // Get the first item in queryResults1
            var phone = queryResults1.First();
            Assert.NotNull(phone);
            Assert.Equal("essfchpbmodumdlbssaoygvcecnegßumuvszyo", phone.PhoneNumber);
            Assert.Equal("ilvxmcmkixinhonuxeqfcbsnlgufneqhijddgurdkuvvj", phone.Extension);

            Assert.NotNull(queryResults2);
            Assert.NotEmpty(queryResults2);

            var contactAliasCount = queryResults2.Count();
            Assert.Equal(1, contactAliasCount);

            Assert.NotNull(queryResults3);
            Assert.NotEmpty(queryResults3);

            var alternativeName = queryResults3.First();

            Assert.Contains("ゼポソソァんマａグぴ九縷亜ぞゼソグバぼダぽママぽポチボソぼぜゾんミぴほダミミ畚珱九ｚべ弌畚タソｚゼソぁび裹ァソマｦひ匚亜ポべポぽマゼたチ裹歹ミポ", alternativeName);

            Assert.NotNull(queryResults4);
            Assert.NotEmpty(queryResults4);

        }

        [Fact]
        public void CollectionOfComplexPropertyQuery_ExecutesSuccessfully()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();

            Assert.NotNull(queryResults);
            Assert.Equal(10, queryResults.Length);
        }

        [Fact]
        public void CollectionOfPrimitivePropertyQuery_ExecutesSuccessfully()
        {
            var queryResults = _context.Execute<ICollection<decimal>>(new Uri(_baseUri.OriginalString + "MappedEntityTypes(-10)/BagOfDecimals")).ToArray();

            Assert.NotNull(queryResults);
            Assert.Single(queryResults);
        }
    }
}
