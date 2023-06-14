namespace Microsoft.OData.ODataService
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.HttpServer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataExtensionsUnitTests
    {
        [TestMethod]
        public async Task HandleODataRequest()
        {
            var request = new HttpServerRequest()
            {
                HttpMethod = "GET",
                Url = "http://localhost:8080/foo",
                Headers = Enumerable.Empty<string>(),
                Body = new MemoryStream(), //// TODO dispose
            };

            var response = await new MockOData().HandleRequestAsync(request);
            Assert.AreEqual(208, response.StatusCode);
            CollectionAssert.AreEqual(new[] { "gdebruin: did this also work?" }, response.Headers.ToArray());
            Assert.AreEqual("ack", await new StreamReader(response.Body).ReadToEndAsync()); //// TODO dispose
        }

        private sealed class MockOData : IODataService
        {
            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var stream = new MemoryStream(); //// TODO where does this get disposed by the caller?
                await stream.WriteAsync(Encoding.ASCII.GetBytes("ack"));
                stream.Position = 0;
                return stream;
            }
        }
    }
}
