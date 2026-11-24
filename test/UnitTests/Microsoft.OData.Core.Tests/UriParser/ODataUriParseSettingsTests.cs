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
            Assert.Equal(ODataUriParserSettings.DefaultMaximumExpansionDepth, settings.MaximumExpansionDepth);
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(200)]
        [InlineData(int.MaxValue)]
        public void MaximumExpansionDepthCanBeSet(int maximumExpansionDepth)
        {
            var settings = new ODataUriParserSettings();

            settings.MaximumExpansionDepth = maximumExpansionDepth;

            Assert.Equal(maximumExpansionDepth, settings.MaximumExpansionDepth);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MaximumExpansionDepthNegativeValueThrows(int maximumExpansionDepth)
        {
            var settings = new ODataUriParserSettings();

            Assert.Throws<ODataException>(() => settings.MaximumExpansionDepth = maximumExpansionDepth);
        }

        [Fact]
        public void MaximumExpansionDepthRemainsUnchangedWhenSettingNegativeValueThrows()
        {
            var settings = new ODataUriParserSettings
            {
                MaximumExpansionDepth = 50
            };

            Assert.Throws<ODataException>(() => settings.MaximumExpansionDepth = -1);

            Assert.Equal(50, settings.MaximumExpansionDepth);
        }

        [Fact]
        public void CloneCopiesMaximumExpansionDepth()
        {
            var settings = new ODataUriParserSettings
            {
                MaximumExpansionDepth = 50
            };

            ODataUriParserSettings clone = settings.Clone();

            Assert.Equal(settings.MaximumExpansionDepth, clone.MaximumExpansionDepth);
        }
    }
}
