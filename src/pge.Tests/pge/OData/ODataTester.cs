namespace pge.OData
{
    using pge.ODataFactory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public sealed class ODataTester
    {
        public void Line1737(IODataFactory odataFactory)
        {
        }

        public void Line1738(IODataFactory odataFactory)
        {
        }

        public void Line1739(IODataFactory odataFactory)
        {
        }

        public void Line1740(IODataFactory odataFactory)
        {
        }

        public void Line1741(IODataFactory odataFactory)
        {
        }

        public void Line1742(IODataFactory odataFactory)
        {
        }

        public void Line1743(IODataFactory odataFactory)
        {
            //// TODO create class template

            //// TODO write this test correctly
            var model = new ODataModel(@"
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
</edmx:Edmx>
",
                ODataModelFormat.Xml);

            var odata = odataFactory.Create(model);
            var response = odata.ServeRequest("asdf");
            Assert.AreEqual(0, response);
        }
    }
}
