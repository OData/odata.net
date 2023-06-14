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
    using Microsoft.OData.OData;

    public static class Program //// TODO public for tests only?
    {
        public static async Task Main(string[] args) //// TODO public for tests only?
        {
            var assembly = Assembly.GetExecutingAssembly();
            var csdlResourceName = assembly.GetManifestResourceNames().Where(name => name.EndsWith("epm.csdl")).Single();
            using (var csdlResourceStream = assembly.GetManifestResourceStream(csdlResourceName))
            {
                var epm = new Epm(csdlResourceStream);

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

        private sealed class Epm : IOData
        {
            private readonly string csdl; //// TODO is readonly correct here if we generalize beyond epm?

            public Epm(Stream csdl)
            {
                var stringBuilder = new StringBuilder();
                using (var xmlWriter = XmlWriter.Create(stringBuilder))
                {
                    using (var xmlReader = XmlReader.Create(csdl)) //// TODO exception handling
                    {
                        while (xmlReader.Read())
                        {
                            xmlWriter.WriteNode(xmlReader, false);
                        }
                    }
                }

                this.csdl = stringBuilder.ToString();

                /*csdlResourceStream.Position = 0;
                using (var streamReader = new StreamReader(csdlResourceStream, Encoding.UTF8, leaveOpen: true)) //// TODO encoding?
                {
                    this.csdl = streamReader.ReadToEnd(); //// TODO async?
                }*/
            }

            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var odataUri = url;
                if (odataUri.ToString() == "/$metadata") //// TODO case sensitive?
                {
                    var stream = new MemoryStream(); //// TODO error handling
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(this.csdl)); //// TODO is this the right encoding?
                    stream.Position = 0;
                    return stream;
                }
                else
                {
                    var stream = new MemoryStream(); //// TODO error handling
                    await stream.WriteAsync(Encoding.UTF8.GetBytes("TODO this should be a 404")); //// TODO is this the right encoding?
                    stream.Position = 0;
                    return stream;
                }
            }
        }
    }
}