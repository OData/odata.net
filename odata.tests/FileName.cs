using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;

namespace odata.tests
{
    [TestClass]
    public sealed class FileName
    {
        [TestMethod]
        public void JsonTest()
        {
            var json =
"""
{
  "a property": "a vlaue",
  "another property": {
    "a nested property": "the nested value"
  },
  "some collection": [
    "primitive"
  ],
  "nonprimitive": [
    {
      "the property in the collection object": "the value for that property"
    }
  ]
}
""";
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var jsonDocument = JsonDocument.Parse(memoryStream);
            }
        }

    }
}
