namespace pge.OData
{
    using pge.ODataFactory;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TODO complete this summary
    /// tests based on https://github.com/OData/odata.net/blob/84488a2dc681b8fd2faf6a78d635f7ccbc650a8c/odata_4.01.txt
    /// </summary>
    public sealed class ODataTester
    {
        public void Line1737ThroughLine1742(IODataFactory odataFactory)
        {
        }

        public void Line1743(IODataFactory odataFactory)
        {
            //// TODO create class template

            //// TODO write this test correctly
            //// TODO confirm this is correct by writing webapi implementation
            var model = new ODataModel(
@"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
</edmx:Edmx>"
                ,
                ODataModelFormat.Xml);

            var odata = odataFactory.Create(model);
            var response = odata.ServeRequest(
@"GET /$metadata HTTP/1.1"
                );
            var statusLine =
@"HTTP/1.1 200 OK";
            Assert.IsTrue(response.StartsWith(statusLine));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Substring(statusLine.Length)));
        }
    }
}
