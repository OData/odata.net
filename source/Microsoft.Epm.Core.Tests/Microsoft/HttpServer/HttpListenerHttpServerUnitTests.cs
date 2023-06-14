namespace Microsoft.HttpServer
{
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HttpListenerHttpServerUnitTests
    {
        [TestMethod]
        public async Task RunServer()
        {
            var statusCode = 209;
            var headerName = "testing";
            var headerValue = "a really good test";
            var header = $"{headerName}: {headerValue}";
            var body = "my neat body";

            //// TODO needs a cancelation token
            var serverTask = new HttpListenerHttpServer(async request => new HttpServerResponse() { StatusCode = statusCode, Headers = new[] { header }, Body = new MemoryStream(Encoding.ASCII.GetBytes(body)) }).ListenAsync();
            
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("http://localhost:8080/foo");
                Assert.AreEqual(statusCode, (int)response.StatusCode);

                Assert.AreEqual(headerValue, response.Headers.GetValues(headerName).Single());
                Assert.AreEqual(body, await response.Content.ReadAsStringAsync());
            }
        }
    }
}