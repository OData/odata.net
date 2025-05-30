using System.Text.Json.Nodes;
using System.Text;
using System.Text.Json;
using NewStuff._Design._4.Sample;
using NewStuff._Design._0_Convention.Sample;

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

        [TestMethod]
        public void MockClient()
        {
            var employeesToUpdatePayload =
"""
{
  "value": [
    {
      "id": "00000000",
      "displayName": "garrett",
      "directReports": []
    }
  ]
}
""";
            var updatedEmployeePayload =
"""
{
  "id": "00000000",
  "displayName": "arrett",
  "directReports": []
}
""";
            var addedEmployeePayload =
"""
{
  "id": "00000003",
  "displayName": "some new employee",
  "directReports": []
}
""";
            var getCeoPayload =
"""
{
  "id": "00000001",
  "displayName": "mike",
  "directReports": [
    {
      "id": "00000000",
      "displayName": "arrett",
      "directReports": []
    },
    {
      "id": "00000002",
      "name": "christof",
      "directReports": []
    }
  ]
}
""";
            using (var employeesToUpdateContent = new StringContent(employeesToUpdatePayload))
            using (var updatedEmployeeContent = new StringContent(updatedEmployeePayload))
            using (var addedEmployeeContent = new StringContent(addedEmployeePayload))
            using (var getCeoContent = new StringContent(getCeoPayload))
            {
                using (var employeesToUpdateClient = new MockHttpClient(new[] { employeesToUpdateContent }))
                using (var updatedEmployeeClient = new MockHttpClient(new[] { updatedEmployeeContent }))
                using (var addedEmployeeClient = new MockHttpClient(new[] { addedEmployeeContent }))
                using (var getCeoClient = new MockHttpClient(new[] { getCeoContent }))
                {
                    var clients = new[] { employeesToUpdateClient, updatedEmployeeClient, addedEmployeeClient, getCeoClient };
                    var nextClientIndex = 0;
                    var driver = DriverFactory.Create(() => clients[nextClientIndex++]);
                    driver.DoWork();
                }
            }
        }
    }
}
