namespace Microsoft.Epm
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.HttpServer;
    using Microsoft.OData.ODataService;

    public static class Program //// TODO public for tests only?
    {
        public static async Task Main(string[] args) //// TODO public for tests only?
        {
            var assembly = Assembly.GetExecutingAssembly();
            var csdlResourceName = assembly.GetManifestResourceNames().Where(name => name.EndsWith("epm.csdl")).Single();
            using (var csdlResourceStream = assembly.GetManifestResourceStream(csdlResourceName))
            {
                var peachySettings = new Peachy.Settings()
                {
                    FeatureGapOData = new Epm(),
                };
                var epm = new Peachy(csdlResourceStream, peachySettings); //// TODO does webapi require running tests synchronously

                var port = 8080;
                await new HttpListenerHttpServer(
                    epm.HandleRequestAsync,
                    new HttpListenerHttpServer.Settings()
                    {
                        Port = port,
                    })
                    .ListenAsync();
            }
        }

        private sealed class Epm : IODataService
        {
            public Epm()
            {
            }

            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var stream = new MemoryStream(); //// TODO error handling
                await stream.WriteAsync(Encoding.UTF8.GetBytes("TODO this should really be a 501")); //// TODO is this the right encoding?
                stream.Position = 0;
                return stream;
            }
        }
    }
}