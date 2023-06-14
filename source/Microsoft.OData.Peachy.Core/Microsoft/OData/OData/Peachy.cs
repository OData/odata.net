namespace Microsoft.OData.OData
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public sealed partial class Peachy : IOData
    {
        private readonly string csdl; //// TODO is readonly correct here if we generalize beyond epm?

        private readonly IOData featureGapOdata;

        public Peachy(Stream csdl)
            : this(csdl, new Settings())
        {
        }

        public Peachy(Stream csdl, Settings settings) //// TODO better name for featuregapodata
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.featureGapOdata = settings.FeatureGapOData;

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

            return await this.featureGapOdata.GetAsync(url, request);
        }

        public sealed class Settings
        {
            public IOData FeatureGapOData { get; set; } = EmptyOData.Instance;
        }
    }
}
