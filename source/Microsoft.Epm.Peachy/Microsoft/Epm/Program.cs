namespace Microsoft.Epm
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.HttpServer;
    using Microsoft.OData.OData;

    public static class Program //// TODO public for tests only?
    {
        public static async Task Main(string[] args) //// TODO public for tests only?
        {
            var odata = new MockOData();
            await new HttpListenerHttpServer(odata.HandleRequestAsync).ListenAsync();
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