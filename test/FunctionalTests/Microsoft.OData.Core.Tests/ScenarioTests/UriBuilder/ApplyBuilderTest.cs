using Microsoft.OData.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using System;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class ApplyBuilderTest : UriBuilderTestBase
    {
        [Fact]
        public void BuildUrlWithApplyFilterGroupByAggregate()
        {
            var query = ("http://gobbledygook/People?$apply=filter(Shoe eq 'blue')/groupby((FirstName,MyDog/LionWhoAteMe/AngerLevel),aggregate(LifeTime with average as avgLifeTime,FavoriteNumber with sum as sumFavoriteNumber,MyDog/LionWhoAteMe/LionHeartbeat with max as maxLionHeartbeat))");
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, ServiceRoot, new Uri(query));
            ODataUri odataUri = parser.ParseUri();

            Uri result = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(query, Uri.UnescapeDataString(result.OriginalString));
        }
    }
}
