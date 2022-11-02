namespace pge.OData
{
    using pge.ODataFactory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PgeODataUnitTests
    {
        [TestMethod]
        public void Line1737ThroughLine1742()
        {
            new ODataTester().Line1737ThroughLine1742(new PgeODataFactory());
        }

        [TestMethod]
        public void Line1743()
        {
            new ODataTester().Line1743(new PgeODataFactory());
        }
    }
}
