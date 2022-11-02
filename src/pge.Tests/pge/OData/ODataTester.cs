namespace pge.OData
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public sealed class ODataTester
    {
        public void Foo(IOData odata)
        {
            //// TODO
            var response = odata.ServeRequest(null, null, null);
            Assert.AreEqual(0, response);
        }
    }
}
