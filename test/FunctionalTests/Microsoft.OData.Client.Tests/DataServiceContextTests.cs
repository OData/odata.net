using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class DataServiceContextTests
    {
        [Fact]
        public void KeyComparisonGeneratesFilterQuery_Defaults_To_True()
        {
            var context = new DataServiceContext();
            Assert.True(context.KeyComparisonGeneratesFilterQuery);
        }
    }
}
