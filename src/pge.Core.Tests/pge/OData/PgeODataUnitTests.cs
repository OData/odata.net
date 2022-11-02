namespace pge.OData
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PgeODataUnitTests
    {
        [TestMethod]
        public void Foo()
        {
            new ODataTester().Foo(new PgeOData());
        }
    }
}
