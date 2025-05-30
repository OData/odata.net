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
  "id": "00000004",
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
            var getArrettPayload =
"""
{
  "id": "00000000",
  "displayName": "arrett",
  "directReports": []
}
""";
            var getChristofPayload =
"""
{
  "id": "00000002",
  "displayName": "christof",
  "directReports": [
    {
      "id": "00000003",
      "displayName": "fake"
    }
  ]
}
""";
            var getFakePayload =
"""
{
  "id": "00000003",
  "displayName": "fake",
  "directReports": []
}
""";

            using (var employeesToUpdateContent = new StringContent(employeesToUpdatePayload))
            using (var updatedEmployeeContent = new StringContent(updatedEmployeePayload))
            using (var addedEmployeeContent = new StringContent(addedEmployeePayload))
            using (var getCeoContent = new StringContent(getCeoPayload))
            using (var getArrettContent = new StringContent(getArrettPayload))
            using (var getChristofContent = new StringContent(getChristofPayload))
            using (var getFakeContent = new StringContent(getFakePayload))
            {
                using (var employeesToUpdateClient = new MockHttpClient(new[] { employeesToUpdateContent }))
                using (var updatedEmployeeClient = new MockHttpClient(new[] { updatedEmployeeContent }))
                using (var addedEmployeeClient = new MockHttpClient(new[] { addedEmployeeContent }))
                using (var getCeoClient = new MockHttpClient(new[] { getCeoContent }))
                using (var getArrettClient = new MockHttpClient(new[] { getArrettContent }))
                using (var getChristofClient = new MockHttpClient(new[] { getChristofContent }))
                using (var getFakeClient = new MockHttpClient(new[] { getFakeContent }))
                {
                    var clients = new[] 
                    { 
                        employeesToUpdateClient, 
                        updatedEmployeeClient, 
                        addedEmployeeClient, 
                        getCeoClient, 
                        getArrettClient, 
                        getChristofClient,
                        getFakeClient 
                    };
                    var nextClientIndex = 0;
                    var driver = DriverFactory.Create(() => clients[nextClientIndex++]);
                    driver.DoWork();
                }
            }
        }
    }
}
