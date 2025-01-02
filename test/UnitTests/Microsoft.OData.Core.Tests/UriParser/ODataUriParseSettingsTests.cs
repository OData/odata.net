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
            Assert.Equal(int.MaxValue, settings.MaximumExpansionDepth);
            Assert.Equal(int.MaxValue, settings.MaximumExpansionCount);
            Assert.True(settings.EnableParsingKeyAsSegment);
        }
    }
}
