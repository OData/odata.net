namespace pge.OData
{
    using pge.ODataFactory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PgeODataUnitTests
    {
        [TestMethod]
        public void Foo()
        {
            new ODataTester().Line1743(new PgeODataFactory());
        }
    }
}
