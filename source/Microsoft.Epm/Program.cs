namespace Microsoft.Epm
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            HttpRequestHandler handler = (request) =>
            {
                var stream = new MemoryStream();
                stream.Write(Encoding.ASCII.GetBytes("ack"));
                stream.Position = 0;
                return new HttpServerResponse()
                {
                    StatusCode = 207,
                    Headers = new[] { "Test: did this work?" },
                    Body = stream,
                };
            };
            await new HttpServer(handler).ListenAsync();
        }
    }
}