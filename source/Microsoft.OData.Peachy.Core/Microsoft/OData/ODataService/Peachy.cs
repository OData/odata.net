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

        public async Task<ODataResponse> GetAsync(ODataRequest request)
        {
            var odataUri = new Uri($"http://testing{request.Url}", UriKind.Absolute);
            if (odataUri.ToString() == "/$metadata") //// TODO case sensitive?
            {
                var stream = new MemoryStream(); //// TODO error handling
                await stream.WriteAsync(Encoding.UTF8.GetBytes(this.csdl)); //// TODO is this the right encoding?
                stream.Position = 0;
                return new ODataResponse(200, Enumerable.Empty<string>(), stream); //// TODO why do we have to get the whole byte array before we even start stream back the response?
            }

            //// TODO finish the generalized segment stuff below
            /*using (var segmentsEnumerator = odataUri.Segments.Select(segment => segment.Trim('/')).GetEnumerator())
            {
                if (!segmentsEnumerator.MoveNext() || !segmentsEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("TODO no segments");
                }

                //// TODO how would they know what key sets they need to support? TODO we could tell them by having them add "hooks" to a builder, and when they "build" we validate that they have all the necessary hooks
                var keys = new List<(EdmElementLookup Property, string Key)>();
                var editUrlSegments = new List<string>();
                TraverseUriSegments(segmentsEnumerator, this.entityDataModel.RootElementLookup, keys, editUrlSegments);
            }*/


            using (var segmentsEnumerator = odataUri.Segments.Select(segment => segment.Trim('/')).GetEnumerator())
            {
                if (!segmentsEnumerator.MoveNext() || !segmentsEnumerator.MoveNext())
                {
                    throw new InvalidOperationException("TODO no segments");
                }

                //// TODO how would they know what key sets they need to support? TODO we could tell them by having them add "hooks" to a builder, and when they "build" we validate that they have all the necessary hooks
                var keys = new List<(EdmElement Property, string PropertyValue)>();
                var editUrlSegments = new List<string>();
                var path = new List<string>();
                return TraverseUriSegments(segmentsEnumerator, this.entityDataModel.RootElement, this.entityDataModel.RootElementLookup, path, keys, editUrlSegments);
            }

            //// TODO handle other urls here by reading the CSDL

            return await this.featureGapOdata.GetAsync(request);
        }

        private ODataResponse TraverseUriSegments(IEnumerator<string> segmentsEnumerator, EdmElement edmElement, EdmElementLookupBase rootElementLookup, List<string> path, List<(EdmElement Property, string PropertyValue)> keys, List<string> editUrlSegments)
        {
            var segment = segmentsEnumerator.Current;
            if (rootElementLookup.SingletonElements.TryGetValue(segment, out var singleValuedElementLookup))
            {
                path.Add(segment);
                if (segmentsEnumerator.MoveNext())
                {
                    return TraverseUriSegments(segmentsEnumerator, singleValuedElementLookup.Element, singleValuedElementLookup, path, keys, editUrlSegments);
                }
                else
                {
                    return null;
                }
            }
            else if (rootElementLookup.CollectionElements.TryGetValue(segment, out var multiValuedElementLookup))
            {
                path.Add(segment);
                if (segmentsEnumerator.MoveNext())
                {
                    segment = segmentsEnumerator.Current;
                    keys.Add((multiValuedElementLookup.Element, segment));
                    var dataStore = multiValuedElementLookup.Element.DataStore;
                    //// TODO do we really want to give a 404 if there's an entity in the middle of a path that can't be found?
                    if (!dataStore.TryGet(keys.ToDictionary(key => key.Property.Name, key => key.PropertyValue), out var entity))
                    {
                        return new ODataResponse(
                            404,
                            Enumerable.Empty<string>(),
                            GenerateStream(new ODataError(
                                "NotFound",
                                $"No entity with key '{segment}' found in the collection at '/{string.Join('/', path)}'.",
                                null,
                                null)));
                    }

                    if (segmentsEnumerator.MoveNext())
                    {
                        path.Add(segment);
                        return TraverseUriSegments(segmentsEnumerator, multiValuedElementLookup.Element, multiValuedElementLookup, path, keys, editUrlSegments);
                    }
                    else
                    {
                        return new ODataResponse(
                            200,
                            Enumerable.Empty<string>(),
                            GenerateStream(multiValuedElementLookup.Element.Selector(entity)));
                    }
                }
                else
                {
                    var dataStore = multiValuedElementLookup.Element.DataStore;
                    var entities = dataStore.GetAll();
                    return new ODataResponse(
                        200,
                        Enumerable.Empty<string>(),
                        GenerateCollectionStream(entities.Select(multiValuedElementLookup.Element.Selector)));
                }
            }
            else
            {
                var entityTypeName = edmElement.ElementType;
                return new ODataResponse(
                    404, //// TODO 404 or 400?
                    Enumerable.Empty<string>(),
                    GenerateStream(new ODataError(
                        "NotFound", //// TODO NotFound or BadRequest?
                        $"The path '/{string.Join('/', path)}' refers to an instance of the type with name '{entityTypeName}'. There is no property with name '{segment}' defined on '{entityTypeName}'.",
                        null,
                        null)));
            }
        }

        private static Stream GenerateCollectionStream<T>(IEnumerable<T> values)
        {
            return GenerateStream(new { value = values });
        }

        private static Stream GenerateStream<T>(T response)
        {
            var serialzied = System.Text.Json.JsonSerializer.Serialize(response);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(serialzied));
            stream.Position = 0;
            return stream;
        }

        public sealed class Settings
        {
            public IODataService FeatureGapOData { get; set; } = EmptyODataService.Instance;
        }
    }
}
