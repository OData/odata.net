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
        public void DerivedTypeFeedQuery()
        {
            var baseQueryResults = _context.Execute<Common.Clients.EndToEnd.PageView>(new Uri(_baseUri.OriginalString + "PageViews/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.ProductPageView")).ToArray();

            var derivedQueryResults = _context.Execute<Common.Clients.EndToEnd.DiscontinuedProduct>(new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct")).ToArray();
        }

        [Fact]
        public void ProjectionEntryQuery()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$select=Name,PrimaryContactInfo")).ToArray();
        }

        [Fact]
        public void ExpandEntryQuery()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info")).ToArray();
        }

        [Fact]
        public void ExpandEntryQueryWithNestedSelect()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Customers(-9)?$expand=Info($select=Information)")).ToArray();
        }

        [Fact]
        public void DerivedTypeExpandWithProjectionFeedQuery()
        {
            var queryUri = new Uri(_baseUri.OriginalString + "Products/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct?$expand=RelatedProducts($select=*),Detail($select=*),Reviews($select=*),Photos($select=*)");
            var queryResults = _context.Execute<Common.Clients.EndToEnd.DiscontinuedProduct>(queryUri).ToArray();
        }

        [Fact]
        public void BasePropertyQueryWithinDerivedType()
        {
            var queryResults = _context.Execute<int>(new Uri(_baseUri.OriginalString + "Products(-9)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.DiscontinuedProduct/ProductId")).ToArray();
        }

        [Fact]
        public void ComplexPropertyQuery()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.ContactDetails>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo")).ToArray();
        }

        [Fact]
        public void NestedComplexPropertyQuery()
        {
            var queryResults1 = _context.Execute<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
            var queryResults2 = _context.Execute<Common.Clients.EndToEnd.Aliases>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias")).ToArray();
            var queryResults3 = _context.Execute<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
            var queryResults4 = _context.Execute<ICollection<string>>(new Uri(_baseUri.OriginalString + "Customers(-10)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
        }

        [Fact]
        public void CollectionOfComplexPropertyQuery()
        {
            var queryResults = _context.Execute<Common.Clients.EndToEnd.Phone>(new Uri(_baseUri.OriginalString + "Customers(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
        }

        [Fact]
        public void CollectionOfPrimitivePropertyQuery()
        {
            var queryResults = _context.Execute<ICollection<decimal>>(new Uri(_baseUri.OriginalString + "MappedEntityTypes(-10)/BagOfDecimals")).ToArray();
        }

        [Fact]
        public void ServiceOperationFeedQuery()
        {
            var queryResult = _context.Execute<Common.Clients.EndToEnd.Customer>(new Uri(_baseUri.OriginalString + "Default.GetSpecificCustomer?Name='enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart'"), "GET", true).ToArray();
            Assert.Equal(1, queryResult.Count());
        }
    }
}
