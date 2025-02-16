using CombinatorParsingV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace odata.tests
{
    [TestClass]
    public sealed class DeferredTests
    {
        [TestMethod]
        public void Test1()
        {
            var url = "/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri(DeferredOutput2.FromValue(input));

            var realUri = odataUri.Realize();

            Assert.IsTrue(realUri.Success);
        }

        [TestMethod]
        public void Test2()
        {
            var url = "B/AA/A/AAA?AAAA=AAAAA";

            var input = new CombinatorParsingV3.StringInput(url);

            var odataUri = new V3ParserPlayground.OdataUri(DeferredOutput2.FromValue(input));

            var realUri = odataUri.Realize();

            Assert.IsFalse(realUri.Success);
        }
    }
}
