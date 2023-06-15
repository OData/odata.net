namespace Microsoft.OData.ODataService
{
    using Microsoft.OData.EntityDataModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    public sealed class Peachy : IODataService
    {
        private readonly EntityDataModel entityDataModel;

        private readonly string csdl; //// TODO is readonly correct here if we generalize beyond epm?

        private readonly IODataService featureGapOdata;

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

            this.entityDataModel = new EntityDataModel(); //// TODO base this off of csdl; should csdl property still exist?

            /*csdlResourceStream.Position = 0;
            using (var streamReader = new StreamReader(csdlResourceStream, Encoding.UTF8, leaveOpen: true)) //// TODO encoding?
            {
                this.csdl = streamReader.ReadToEnd(); //// TODO async?
            }*/
        }

        public async Task<Stream> GetAsync(string url, Stream request)
        {
            var odataUri = new Uri($"http://testing{url}", UriKind.Absolute);
            if (odataUri.ToString() == "/$metadata") //// TODO case sensitive?
            {
                var stream = new MemoryStream(); //// TODO error handling
                await stream.WriteAsync(Encoding.UTF8.GetBytes(this.csdl)); //// TODO is this the right encoding?
                stream.Position = 0;
                return stream; //// TODO why do we have to get the whole byte array before we even start stream back the response?
            }

            //// TODO finish the generalized segment stuff below
            /*using (var segmentsEnumerator = odataUri.Segments.Select(segment => segment.Trim('/')).GetEnumerator())
            {
                if (!segmentsEnumerator.MoveNext() || !segmentsEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("TODO no segments");
                }

                //// TODO how would they know what key sets they need to support?
                var keys = new List<(EdmElementLookup Property, string Key)>();
                var editUrlSegments = new List<string>();
                TraverseUriSegments(segmentsEnumerator, this.entityDataModel.RootElementLookup, keys, editUrlSegments);
            }*/

            //// TODO handle other urls here by reading the CSDL

            return await this.featureGapOdata.GetAsync(url, request);
        }

        private void TraverseUriSegments(IEnumerator<string> segmentsEnumerator, EdmElementLookup rootElement, List<(EdmElementLookup Property, string Key)> keys, List<string> editUrlSegments)
        {
            var segment = segmentsEnumerator.Current;
            editUrlSegments.Add(segment);
            EdmElementLookup edmElement;
            if (rootElement.SingletonElements.TryGetValue(segment, out edmElement))
            {
                if (segmentsEnumerator.MoveNext())
                {
                    TraverseUriSegments(segmentsEnumerator, edmElement, keys, editUrlSegments);
                }
                else
                {
                    return;
                }
            }
            else if (rootElement.CollectionElements.TryGetValue(segment, out edmElement))
            {
                editUrlSegments.Clear();
                if (segmentsEnumerator.MoveNext())
                {
                    keys.Add((edmElement, segmentsEnumerator.Current));
                    if (segmentsEnumerator.MoveNext())
                    {
                        TraverseUriSegments(segmentsEnumerator, edmElement, keys, editUrlSegments);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                throw new Exception($"TODO the type doesn't contain a property with name {segment}");
            }
        }

        public sealed class Settings
        {
            public IODataService FeatureGapOData { get; set; } = EmptyOData.Instance;
        }
    }
}
