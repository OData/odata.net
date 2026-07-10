using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataUriParseSettingsTests
    {
        [Fact]
        public void DefaultSettingsValues()
        {
            var settings = new ODataUriParserSettings();
            Assert.Equal(ODataUriParserSettings.DefaultFilterLimit, settings.FilterLimit);
            Assert.Equal(ODataUriParserSettings.DefaultPathLimit, settings.PathLimit);
            Assert.Equal(ODataUriParserSettings.DefaultSelectExpandLimit, settings.SelectExpandLimit);
            Assert.Equal(ODataUriParserSettings.DefaultSearchLimit, settings.SearchLimit);
            Assert.Equal(ODataUriParserSettings.DefaultAggregateLimit, settings.AggregateLimit);
            Assert.Equal(int.MaxValue, settings.MaximumExpansionDepth);
            Assert.Equal(int.MaxValue, settings.MaximumExpansionCount);
            Assert.True(settings.EnableParsingKeyAsSegment);
        }

        [Fact]
        public void AggregateLimitCanBeSet()
        {
            var settings = new ODataUriParserSettings();
            settings.AggregateLimit = 50;
            Assert.Equal(50, settings.AggregateLimit);
        }

        [Fact]
        public void AggregateLimitNegativeValueThrows()
        {
            var settings = new ODataUriParserSettings();
            Assert.Throws<ODataException>(() => settings.AggregateLimit = -1);
        }
    }
}
