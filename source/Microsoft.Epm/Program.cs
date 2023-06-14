namespace Microsoft.Epm
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.HttpServer;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            HttpRequestHandler handler = async (request) =>
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
            await new HttpListenerHttpServer(handler).ListenAsync();
        }
    }
}