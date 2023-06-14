namespace Microsoft.Epm
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.HttpServer;
    using Microsoft.OData.OData;

    internal static class Program
    {
        private static readonly HttpRequestHandler handler = async (request) =>
        {
            var stream = new MemoryStream();
            await stream.WriteAsync(Encoding.ASCII.GetBytes("ack"));
            stream.Position = 0;
            return new HttpServerResponse()
            {
                StatusCode = 207,
                Headers = new[] { "Test: did this work?" },
                Body = stream,
            };
        };

        private static async Task Main(string[] args)
        {
            var odata = new MockOData();
            await new HttpListenerHttpServer(odata.HandleRequest).ListenAsync();
        }

        private sealed class MockOData : IOData
        {
            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var stream = new MemoryStream();
                await stream.WriteAsync(Encoding.ASCII.GetBytes("ack"));
                stream.Position = 0;
                return stream;
            }
        }
    }
}