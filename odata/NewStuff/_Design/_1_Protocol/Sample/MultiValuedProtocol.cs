namespace NewStuff._Design._1_Protocol.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using NewStuff._Design._0_Convention;

    public sealed class MultiValuedProtocol : IMultiValuedProtocol
    {
        private readonly IConvention convention;

        private readonly Uri multiValuedUri;

        public MultiValuedProtocol(IConvention convention, Uri multiValuedUri)
        {
            if (!multiValuedUri.IsAbsoluteUri)
            {
                //// TODO can you have `iconvention` have the root and `imultivaluedprotocol` takes in the rest of the url?
                throw new Exception("TODO add support for relative uris");
            }

            this.convention = convention;
            this.multiValuedUri = multiValuedUri;
        }

        public IGetMultiValuedProtocol Get()
        {
            return new GetMultiValuedProtocol(this.convention, this.multiValuedUri);
        }

        private sealed class GetMultiValuedProtocol : IGetMultiValuedProtocol
        {
            private readonly IConvention convention;
            private readonly Uri multiValuedUri;

            private readonly IEnumerable<string> expands;
            private readonly IEnumerable<string> selects;
            private readonly IEnumerable<string> filters;
            private readonly int? skip;

            public GetMultiValuedProtocol(IConvention convention, Uri multiValuedUri)
                : this(convention, multiValuedUri, Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), null)
            {
            }

            private GetMultiValuedProtocol(
                IConvention convention, 
                Uri multiValuedUri,
                IEnumerable<string> expands,
                IEnumerable<string> selects,
                IEnumerable<string> filters,
                int? skip)
            {
                this.convention = convention;
                this.multiValuedUri = multiValuedUri;

                this.expands = expands;
                this.selects = selects;
                this.filters = filters;
                this.skip = skip;
            }

            private sealed class MultiValuedResponseBuilder
            {
                public List<SingleValue> Value { get; set; } = new List<SingleValue>();

                public Uri? NextLink { get; set; } //// TODO make this strongly-typed

                public int? Count { get; set; } //// TODO does this need to be strongly-typed? (maybe to avoid nullable?)

                public IEnumerable<object> Annotations { get; set; } = new List<object>(); //// TODO make this strongly-typed

                public MultiValuedResponse Build()
                {
                    return new MultiValuedResponse(this.Value, this.NextLink, this.Count, this.Annotations);
                }
            }

            private Uri GenerateRequestedUri()
            {
                var builder = new UriBuilder(this.multiValuedUri);
                builder.Query = string.Join(
                    "&",
                    string.Join(",", this.expands),
                    string.Join(",", this.selects),
                    string.Join(" and ", this.filters),
                    this.skip == null ? $"skip={this.skip}" : null);
                return builder.Uri;
            }

            public async Task<MultiValuedResponse> Evaluate()
            {
                var requestWriter = await this.convention.Get().ConfigureAwait(false);
                var uriWriter = await requestWriter.Commit().ConfigureAwait(false);

                var getHeaderWriter = await WriteUri(GenerateRequestedUri(), uriWriter).ConfigureAwait(false);

                //// TODO not writing any headers...
                var getBodyWriter = await getHeaderWriter.Commit().ConfigureAwait(false);

                // wait for a response
                var getResponseReader = await getBodyWriter.Commit().ConfigureAwait(false);

                var multiValueResponseBuilder = new MultiValuedResponseBuilder();
                multiValueResponseBuilder.Annotations = Enumerable.Empty<object>(); //// TODO annotations aren't supported yet

                var headerReader = getResponseReader.Next();
                var getResponseBodyReader = MultiValuedProtocol.SkipHeaders(headerReader);

                while (true)
                {
                    var getResponseBodyToken = getResponseBodyReader.Next();
                    if (getResponseBodyToken is GetResponseBodyToken.NextLink nextLink)
                    {
                        var nextLinkReader = nextLink.NextLinkReader;
                        multiValueResponseBuilder.NextLink = nextLinkReader.NextLink.Uri;
                        getResponseBodyReader = nextLinkReader.Next();
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.OdataContext odataContext)
                    {
                        //// TODO this implementation is skipping the odatacontext
                        getResponseBodyReader = odataContext.OdataContextReader.Next();
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.Property property)
                    {
                        var propertyReader = property.PropertyReader;
                        var propertyNameReader = propertyReader.Next();
                        var propertyName = propertyNameReader.PropertyName;
                        if (string.Equals(propertyName.Name, "value", StringComparison.Ordinal)) //// TODO do we want to configurably ignore casing?
                        {
                            var propertyValueReader = propertyNameReader.Next();
                            var propertyValueToken = propertyValueReader.Next();
                            if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.MultiValued multiValued)
                            {
                                var multiValuedPropertyValueReader = multiValued.MultiValuedPropertyValueReader;
                                while (true)
                                {
                                    var multiValuedPropertyValueToken = multiValuedPropertyValueReader.Next();
                                    if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<IGetResponseBodyReader>.Object @object)
                                    {
                                        var complexPropertyValueReader = @object.ComplexPropertyValueReader;
                                        var singleValueBuilder = new MultiValuedProtocol.SingleValueBuilder();

                                        multiValuedPropertyValueReader = MultiValuedProtocol.ReadComplexPropertyValue(complexPropertyValueReader, singleValueBuilder);

                                        multiValueResponseBuilder.Value.Add(singleValueBuilder.Build());
                                    }
                                    else if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<IGetResponseBodyReader>.End end)
                                    {
                                        getResponseBodyReader = end.Reader;
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception("TODO implement visitor");
                                    }
                                }
                            }
                            /*if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.Complex complex)
                            {
                                var complexPropertyValueReader = complex.ComplexPropertyValueReader;
                                var singleValueBuilder = new MultiValuedProtocol.SingleValueBuilder();

                                getResponseBodyReader = MultiValuedProtocol.ReadComplexPropertyValue(complexPropertyValueReader, singleValueBuilder);

                                multiValueResponseBuilder.Value.Add(singleValueBuilder.Build());
                            }*/
                            else
                            {
                                //// TODO have you modeled an empty collection yet?
                                throw new Exception("TODO error occurred parsing; we expected a mulit-valued response and found a property named 'value', but that property didn't have some number of objects inside");
                                //// TODO use a visitor
                            }
                        }
                        else
                        {
                            var propertyValueReader = propertyNameReader.Next();
                            getResponseBodyReader = SkipPropertyValue(propertyValueReader);
                        }
                    }
                    //// TODO count isn't available here...
                    else if (getResponseBodyToken is GetResponseBodyToken.End end)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }

                return multiValueResponseBuilder.Build();
            }

            private static T SkipPropertyValue<T>(IPropertyValueReader<T> propertyValueReader)
            {
                var propertyValueToken = propertyValueReader.Next(); //// TODO do you want to add `skip` methods?
                if (propertyValueToken is PropertyValueToken<T>.Complex complex)
                {
                    return SkipComplexPropertyValue(complex.ComplexPropertyValueReader);
                }
                else if (propertyValueToken is PropertyValueToken<T>.MultiValued multiValued)
                {
                    var multiValuedPropertyValueReader = multiValued.MultiValuedPropertyValueReader;
                    while (true)
                    {
                        var multiValuedPropertyValueToken = multiValuedPropertyValueReader.Next();
                        if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<T>.Object @object)
                        {
                            multiValuedPropertyValueReader = SkipComplexPropertyValue(@object.ComplexPropertyValueReader);
                        }
                        else if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<T>.End end)
                        {
                            return end.Reader;
                        }
                        else
                        {
                            throw new Exception("TODO implement visitor");
                        }
                    }
                }
                else if (propertyValueToken is PropertyValueToken<T>.Null @null)
                {
                    var nullPropertyValueReader = @null.NullPropertyValueReader;
                    return nullPropertyValueReader.Next();
                }
                else if (propertyValueToken is PropertyValueToken<T>.Primitive primitive)
                {
                    var primitivePropertyValueReader = primitive.PrimitivePropertyValueReader;
                    return primitivePropertyValueReader.Next();
                }
                else
                {
                    throw new Exception("TODO implement visitor");
                }
            }

            private static T SkipComplexPropertyValue<T>(IComplexPropertyValueReader<T> complexPropertyValueReader)
            {
                while (true)
                {
                    var complexPropertyValueToken = complexPropertyValueReader.Next();
                    if (complexPropertyValueToken is ComplexPropertyValueToken<T>.Property property)
                    {
                        complexPropertyValueReader = SkipNestedProperty(property.PropertyReader);
                    }
                    else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.OdataContext odataContext)
                    {
                        complexPropertyValueReader = odataContext.OdataContextReader.Next();
                    }
                    else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.OdataId odataId)
                    {
                        complexPropertyValueReader = odataId.OdataIdReader.Next();
                    }
                    else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.End end)
                    {
                        return end.Reader;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }
            }

            private static IComplexPropertyValueReader<T> SkipNestedProperty<T>(IPropertyReader<IComplexPropertyValueReader<T>> propertyReader)
            {
                var propertyNameReader = propertyReader.Next();
                var propertyValueReader = propertyNameReader.Next();
                return SkipPropertyValue(propertyValueReader);
            }

            public IGetMultiValuedProtocol Expand(object expander)
            {
                return new GetMultiValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.expands.Append("TODO"),
                    this.selects,
                    this.filters,
                    this.skip);
            }

            public IGetMultiValuedProtocol Filter(object predicate)
            {
                return new GetMultiValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.expands,
                    this.selects,
                    this.filters.Append("TODO"),
                    this.skip);
            }

            public IGetMultiValuedProtocol Select(object selector)
            {
                return new GetMultiValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.expands,
                    this.selects.Append("TODO"),
                    this.filters,
                    this.skip);
            }

            public IGetMultiValuedProtocol Skip(object count)
            {
                return new GetMultiValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.expands,
                    this.selects,
                    this.filters,
                    -1);
            }
        }

        public IGetSingleValuedProtocol Get(KeyPredicate key)
        {
            var keySegments = GenerateKeySegmentsFromKeyPredicate(key);
            return new GetSingleValuedProtocol(this.convention, new Uri(this.multiValuedUri, keySegments));
        }

        private static string GenerateKeySegmentsFromKeyPredicate(KeyPredicate keyPredicate)
        {
            Func<KeyArgument, string> convert = keyArgument => $"{keyArgument.PropertyName}={keyArgument.PropertyValue}";

            string path;
            if (keyPredicate is KeyPredicate.MultiPart multiPart)
            {
                path = string.Join(
                    '/',
                    multiPart
                        .KeyArguments
                        .Select(convert)); //// TODO what about string values? they need single quotes
            }
            else if (keyPredicate is KeyPredicate.SinglePart singlePart)
            {
                if (singlePart.KeyPredicate is SinglePartKeyPredicate.Canonical canonical)
                {
                    path = canonical.PropertyValue;
                }
                else if (singlePart.KeyPredicate is SinglePartKeyPredicate.NonCanonical nonCanonical)
                {
                    path = convert(nonCanonical.KeyArgument);
                }
                else
                {
                    throw new Exception("TODO implement visitor");
                }
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }

            return path;
        }

        private sealed class GetSingleValuedProtocol : IGetSingleValuedProtocol
        {
            private readonly IConvention convention;
            private readonly Uri singleValuedUri;
            private readonly IEnumerable<string> expands;
            private readonly IEnumerable<string> selects;

            public GetSingleValuedProtocol(IConvention convention, Uri singleValuedUri)
                : this(convention, singleValuedUri, Enumerable.Empty<string>(), Enumerable.Empty<string>())
            {
            }

            private GetSingleValuedProtocol(
                IConvention convention, 
                Uri singleValuedUri, 
                IEnumerable<string> expands, 
                IEnumerable<string> selects)
            {
                this.convention = convention;
                this.singleValuedUri = singleValuedUri;

                this.expands = expands;
                this.selects = selects;
            }

            private Uri GenerateRequestedUri()
            {
                var builder = new UriBuilder(this.singleValuedUri);
                builder.Query = string.Join(
                    "&",
                    string.Join(",", this.expands),
                    string.Join(",", this.selects));
                return builder.Uri;
            }

            public async Task<SingleValuedResponse> Evaluate()
            {
                var requestWriter = await this.convention.Get().ConfigureAwait(false);
                var uriWriter = await requestWriter.Commit().ConfigureAwait(false);

                var getHeaderWriter = await WriteUri(GenerateRequestedUri(), uriWriter).ConfigureAwait(false);

                var customHeaderWriter = await getHeaderWriter.CommitCustomHeader().ConfigureAwait(false); //// TODO should "content-type: application/json" really be considered "custom"?
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Content-Type")).ConfigureAwait(false);
                getHeaderWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue("application/json")).ConfigureAwait(false);

                var getBodyWriter = await getHeaderWriter.Commit().ConfigureAwait(false);

                // send the request
                var getResponseReader = await getBodyWriter.Commit().ConfigureAwait(false);

                var getResponseHeaderReader = getResponseReader.Next();
                var getResponseBodyReader = MultiValuedProtocol.SkipHeaders(getResponseHeaderReader);

                var singleValuedResponseBuilder = new SingleValuedResponseBuilder();
                var singleValueBuilder = new SingleValueBuilder();
                while (true)
                {
                    var getResponseBodyToken = getResponseBodyReader.Next();
                    if (getResponseBodyToken is GetResponseBodyToken.Property property)
                    {
                        var propertyReader = property.PropertyReader;
                        var propertyNameReader = propertyReader.Next();
                        var propertyName = propertyNameReader.PropertyName;
                        var propertyValueReader = propertyNameReader.Next();

                        getResponseBodyReader = MultiValuedProtocol.ReadPropertyValue(propertyValueReader, propertyName.Name, singleValueBuilder);
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.OdataContext odataContext)
                    {
                        //// TODO this implementation is not using the context
                        getResponseBodyReader = odataContext.OdataContextReader.Next();
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.NextLink nextLink)
                    {
                        throw new Exception("TODO no nextlinks for single-valued responses");
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.End end)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }

                singleValuedResponseBuilder.Value = singleValueBuilder.Build();
                return singleValuedResponseBuilder.Build();
            }

            public IGetSingleValuedProtocol Expand(object expander)
            {
                return new GetSingleValuedProtocol(
                    this.convention,
                    this.singleValuedUri,
                    this.expands.Append("TODO"),
                    this.selects);
            }

            public IGetSingleValuedProtocol Select(object selector)
            {
                return new GetSingleValuedProtocol(
                    this.convention,
                    this.singleValuedUri,
                    this.expands,
                    this.selects.Append("TODO"));
            }
        }

        public IPatchSingleValuedProtocol Patch(KeyPredicate key, SingleValuedRequest request)
        {
            var keySegments = GenerateKeySegmentsFromKeyPredicate(key);
            return new PatchSingleValuedProtocol(this.convention, new Uri(this.multiValuedUri, keySegments), request);
        }

        private sealed class PatchSingleValuedProtocol : IPatchSingleValuedProtocol
        {
            private readonly IConvention convention;
            private readonly Uri singleValuedUri;
            private readonly SingleValuedRequest singleValuedRequest;

            private readonly IEnumerable<string> expands;
            private readonly IEnumerable<string> selects;

            public PatchSingleValuedProtocol(IConvention convention, Uri singleValuedUri, SingleValuedRequest singleValuedRequest)
                : this(
                      convention, 
                      singleValuedUri, 
                      singleValuedRequest,
                      Enumerable.Empty<string>(), 
                      Enumerable.Empty<string>())
            {
            }

            private PatchSingleValuedProtocol(
                IConvention convention,
                Uri singleValuedUri, 
                SingleValuedRequest singleValuedRequest,
                IEnumerable<string> expands,
                IEnumerable<string> selects)
            {
                this.convention = convention;
                this.singleValuedUri = singleValuedUri;
                this.singleValuedRequest = singleValuedRequest;

                this.expands = expands;
                this.selects = selects;
            }

            public async Task<SingleValuedResponse> Evaluate()
            {
                var patchRequestWriter = this.convention.Patch();

                var uriWriter = await patchRequestWriter.Commit().ConfigureAwait(false);
                var patchHeaderWriter = await MultiValuedProtocol.WriteUri(this.singleValuedUri, uriWriter).ConfigureAwait(false);

                var customHeaderWriter = await patchHeaderWriter.CommitCustomHeader().ConfigureAwait(false);
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Content-Type")).ConfigureAwait(false); //// TODO probably this should be a "built-in" header
                patchHeaderWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue("application/json")).ConfigureAwait(false);
                var patchRequestBodyWriter = await patchHeaderWriter.Commit().ConfigureAwait(false);

                //// TODO what about dynamic and untyped properties?
                foreach (var complexProperty in this.singleValuedRequest.ComplexProperties)
                {
                    //// TODO you haven't actually fleshed out complex properties yet in the protocol layer
                }

                foreach (var multiValuedProperty in this.singleValuedRequest.MultiValuedProperties)
                {
                    var propertyWriter = await patchRequestBodyWriter.CommitProperty().ConfigureAwait(false);
                    var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                    var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(multiValuedProperty.Name)).ConfigureAwait(false);
                    var multiValuedPropertyValueWriter = await propertyValueWriter.CommitMultiValued().ConfigureAwait(false);

                    var objectWriter = await multiValuedPropertyValueWriter.CommitValue().ConfigureAwait(false);

                    foreach (var value in multiValuedProperty.Values)
                    {
                        multiValuedPropertyValueWriter = await WriteSingleValuedRequest(value, objectWriter).ConfigureAwait(false);
                    }

                    patchRequestBodyWriter = await multiValuedPropertyValueWriter.Commit().ConfigureAwait(false);
                }

                foreach (var primitiveProperty in this.singleValuedRequest.PrimitiveProperties)
                {
                    var propertyWriter = await patchRequestBodyWriter.CommitProperty().ConfigureAwait(false);
                    var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                    var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(primitiveProperty.Name)).ConfigureAwait(false);

                    if (primitiveProperty.Value == null)
                    {
                        var nullPropertyValueWriter = await propertyValueWriter.CommitNull().ConfigureAwait(false);
                        patchRequestBodyWriter = await nullPropertyValueWriter.Commit().ConfigureAwait(false);
                    }
                    else
                    {
                        var primitivePropertyValueWriter = await propertyValueWriter.CommitPrimitive().ConfigureAwait(false);
                        patchRequestBodyWriter = await primitivePropertyValueWriter.Commit(new PrimitivePropertyValue(primitiveProperty.Value)).ConfigureAwait(false);
                    }
                }

                // send the request
                var patchResponseReader = await patchRequestBodyWriter.Commit().ConfigureAwait(false);

                var patchResponseHeaderReader = patchResponseReader.Next();
                var patchResponseBodyReader = MultiValuedProtocol.SkipHeaders(patchResponseHeaderReader);

                var singleValuedResponseBuilder = new SingleValuedResponseBuilder();
                var singleValueBuilder = new SingleValueBuilder();
                while (true)
                {
                    var getResponseBodyToken = patchResponseBodyReader.Next();
                    if (getResponseBodyToken is GetResponseBodyToken.Property property)
                    {
                        var propertyReader = property.PropertyReader;
                        var propertyNameReader = propertyReader.Next();
                        var propertyName = propertyNameReader.PropertyName;
                        var propertyValueReader = propertyNameReader.Next();

                        patchResponseBodyReader = MultiValuedProtocol.ReadPropertyValue(propertyValueReader, propertyName.Name, singleValueBuilder);
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.OdataContext odataContext)
                    {
                        //// TODO this implementation is not using the context
                        patchResponseBodyReader = odataContext.OdataContextReader.Next();
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.NextLink nextLink)
                    {
                        throw new Exception("TODO no nextlinks for single-valued responses");
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.End end)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }

                singleValuedResponseBuilder.Value = singleValueBuilder.Build();
                return singleValuedResponseBuilder.Build();
            }

            public IPatchSingleValuedProtocol Expand(object expander)
            {
                return new PatchSingleValuedProtocol(
                    this.convention,
                    this.singleValuedUri,
                    this.singleValuedRequest,
                    this.expands.Append("TODO"),
                    this.selects);
            }

            public IPatchSingleValuedProtocol Select(object selector)
            {
                return new PatchSingleValuedProtocol(
                    this.convention,
                    this.singleValuedUri,
                    this.singleValuedRequest,
                    this.expands,
                    this.selects.Append("TODO"));
            }
        }

        public IPostSingleValuedProtocol Post(SingleValuedRequest request)
        {
            return new PostSingleValuedProtocol(this.convention, this.multiValuedUri, request);
        }

        private sealed class PostSingleValuedProtocol : IPostSingleValuedProtocol
        {
            private readonly IConvention convention;
            private readonly Uri multiValuedUri;
            private readonly SingleValuedRequest singleValuedRequest;

            private readonly IEnumerable<string> expands;
            private readonly IEnumerable<string> selects;

            public PostSingleValuedProtocol(IConvention convention, Uri multiValuedUri, SingleValuedRequest singleValuedRequest)
                : this(
                      convention,
                      multiValuedUri,
                      singleValuedRequest,
                      Enumerable.Empty<string>(),
                      Enumerable.Empty<string>())
            {
            }

            private PostSingleValuedProtocol(
                IConvention convention,
                Uri multiValuedUri,
                SingleValuedRequest singleValuedRequest,
                IEnumerable<string> expands,
                IEnumerable<string> selects)
            {
                this.convention = convention;
                this.multiValuedUri = multiValuedUri;
                this.singleValuedRequest = singleValuedRequest;

                this.expands = expands;
                this.selects = selects;
            }

            public async Task<SingleValuedResponse> Evaluate()
            {
                var patchRequestWriter = this.convention.Post();

                var uriWriter = await patchRequestWriter.Commit().ConfigureAwait(false);
                var patchHeaderWriter = await MultiValuedProtocol.WriteUri(this.multiValuedUri, uriWriter).ConfigureAwait(false);

                var customHeaderWriter = await patchHeaderWriter.CommitCustomHeader().ConfigureAwait(false);
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Content-Type")).ConfigureAwait(false); //// TODO probably this should be a "built-in" header
                patchHeaderWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue("application/json")).ConfigureAwait(false);
                var patchRequestBodyWriter = await patchHeaderWriter.Commit().ConfigureAwait(false);

                //// TODO what about dynamic and untyped properties?
                foreach (var complexProperty in this.singleValuedRequest.ComplexProperties)
                {
                    //// TODO you haven't actually fleshed out complex properties yet in the protocol layer
                }

                foreach (var multiValuedProperty in this.singleValuedRequest.MultiValuedProperties)
                {
                    var propertyWriter = await patchRequestBodyWriter.CommitProperty().ConfigureAwait(false);
                    var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                    var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(multiValuedProperty.Name)).ConfigureAwait(false);
                    var multiValuedPropertyValueWriter = await propertyValueWriter.CommitMultiValued().ConfigureAwait(false);

                    var objectWriter = await multiValuedPropertyValueWriter.CommitValue().ConfigureAwait(false);

                    foreach (var value in multiValuedProperty.Values)
                    {
                        multiValuedPropertyValueWriter = await WriteSingleValuedRequest(value, objectWriter).ConfigureAwait(false);
                    }

                    patchRequestBodyWriter = await multiValuedPropertyValueWriter.Commit().ConfigureAwait(false);
                }

                foreach (var primitiveProperty in this.singleValuedRequest.PrimitiveProperties)
                {
                    var propertyWriter = await patchRequestBodyWriter.CommitProperty().ConfigureAwait(false);
                    var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                    var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(primitiveProperty.Name)).ConfigureAwait(false);

                    if (primitiveProperty.Value == null)
                    {
                        var nullPropertyValueWriter = await propertyValueWriter.CommitNull().ConfigureAwait(false);
                        patchRequestBodyWriter = await nullPropertyValueWriter.Commit().ConfigureAwait(false);
                    }
                    else
                    {
                        var primitivePropertyValueWriter = await propertyValueWriter.CommitPrimitive().ConfigureAwait(false);
                        patchRequestBodyWriter = await primitivePropertyValueWriter.Commit(new PrimitivePropertyValue(primitiveProperty.Value)).ConfigureAwait(false);
                    }
                }

                // send the request
                var patchResponseReader = await patchRequestBodyWriter.Commit().ConfigureAwait(false);

                var patchResponseHeaderReader = patchResponseReader.Next();
                var patchResponseBodyReader = MultiValuedProtocol.SkipHeaders(patchResponseHeaderReader);

                var singleValuedResponseBuilder = new SingleValuedResponseBuilder();
                var singleValueBuilder = new SingleValueBuilder();
                while (true)
                {
                    var getResponseBodyToken = patchResponseBodyReader.Next();
                    if (getResponseBodyToken is GetResponseBodyToken.Property property)
                    {
                        var propertyReader = property.PropertyReader;
                        var propertyNameReader = propertyReader.Next();
                        var propertyName = propertyNameReader.PropertyName;
                        var propertyValueReader = propertyNameReader.Next();

                        patchResponseBodyReader = MultiValuedProtocol.ReadPropertyValue(propertyValueReader, propertyName.Name, singleValueBuilder);
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.OdataContext odataContext)
                    {
                        //// TODO this implementation is not using the context
                        patchResponseBodyReader = odataContext.OdataContextReader.Next();
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.NextLink nextLink)
                    {
                        throw new Exception("TODO no nextlinks for single-valued responses");
                    }
                    else if (getResponseBodyToken is GetResponseBodyToken.End end)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }

                singleValuedResponseBuilder.Value = singleValueBuilder.Build();
                return singleValuedResponseBuilder.Build();
            }

            public IPostSingleValuedProtocol Expand(object expander)
            {
                return new PostSingleValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.singleValuedRequest,
                    this.expands.Append("TODO"),
                    this.selects);
            }

            public IPostSingleValuedProtocol Select(object selector)
            {
                return new PostSingleValuedProtocol(
                    this.convention,
                    this.multiValuedUri,
                    this.singleValuedRequest,
                    this.expands,
                    this.selects.Append("TODO"));
            }
        }

        private static async Task<T> WriteSingleValuedRequest<T>(SingleValuedRequest singleValuedRequest, IComplexPropertyValueWriter<T> complexPropertyValueWriter)
        {
            //// TODO dynamic properties, untyped properties, complex properties

            foreach (var multiValuedProperty in singleValuedRequest.MultiValuedProperties)
            {
                var propertyWriter = await complexPropertyValueWriter.CommitProperty().ConfigureAwait(false);
                var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(multiValuedProperty.Name)).ConfigureAwait(false);
                var multiValuedPropertyValueWriter = await propertyValueWriter.CommitMultiValued().ConfigureAwait(false);

                var objectWriter = await multiValuedPropertyValueWriter.CommitValue().ConfigureAwait(false);

                foreach (var value in multiValuedProperty.Values)
                {
                    multiValuedPropertyValueWriter = await WriteSingleValuedRequest(value, objectWriter).ConfigureAwait(false);
                }

                complexPropertyValueWriter = await multiValuedPropertyValueWriter.Commit().ConfigureAwait(false);
            }

            foreach (var primitiveProperty in singleValuedRequest.PrimitiveProperties)
            {
                var propertyWriter = await complexPropertyValueWriter.CommitProperty().ConfigureAwait(false);
                var propertyNameWriter = await propertyWriter.Commit().ConfigureAwait(false);
                var propertyValueWriter = await propertyNameWriter.Commit(new PropertyName(primitiveProperty.Name)).ConfigureAwait(false);

                if (primitiveProperty.Value == null)
                {
                    var nullPropertyValueWriter = await propertyValueWriter.CommitNull().ConfigureAwait(false);
                    complexPropertyValueWriter = await nullPropertyValueWriter.Commit().ConfigureAwait(false);
                }
                else
                {
                    var primitivePropertyValueWriter = await propertyValueWriter.CommitPrimitive().ConfigureAwait(false);
                    complexPropertyValueWriter = await primitivePropertyValueWriter.Commit(new PrimitivePropertyValue(primitiveProperty.Value)).ConfigureAwait(false);
                }
            }

            return await complexPropertyValueWriter.Commit().ConfigureAwait(false);
        }

        private sealed class SingleValuedResponseBuilder
        {
            public SingleValue? Value { get; set; }

            public List<object> Annotations { get; set; } = new List<object>();

            public SingleValuedResponse Build()
            {
                return new SingleValuedResponse(this.Value, this.Annotations);
            }
        }

        private static T ReadComplexPropertyValue<T>(IComplexPropertyValueReader<T> complexPropertyValueReader, MultiValuedProtocol.SingleValueBuilder singleValueBuilder)
        {
            while (true)
            {
                var complexPropertyValueToken = complexPropertyValueReader.Next();
                if (complexPropertyValueToken is ComplexPropertyValueToken<T>.Property property)
                {
                    var propertyReader = property.PropertyReader;
                    var propertyNameReader = propertyReader.Next();

                    var propertyName = propertyNameReader.PropertyName.Name;

                    var propertyValueReader = propertyNameReader.Next();
                    complexPropertyValueReader = MultiValuedProtocol.ReadPropertyValue(propertyValueReader, propertyName, singleValueBuilder);
                }
                else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.OdataContext odataContext)
                {
                    var odataContextReader = odataContext.OdataContextReader;
                    singleValueBuilder.Context = odataContextReader.OdataContext.Context;

                    complexPropertyValueReader = odataContext.OdataContextReader.Next();
                }
                else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.OdataId odataId)
                {
                    //// TODO this implementation is skipping the odataid

                    complexPropertyValueReader = odataId.OdataIdReader.Next();
                }
                else if (complexPropertyValueToken is ComplexPropertyValueToken<T>.End end)
                {
                    return end.Reader;
                }
                else
                {
                    throw new Exception("TODO implement visitor");
                }
            }
        }

        private static T ReadPropertyValue<T>(IPropertyValueReader<T> propertyValueReader, string propertyName, MultiValuedProtocol.SingleValueBuilder singleValueBuilder)
        {
            //// TODO you are here
            var propertyValueToken = propertyValueReader.Next();
            if (propertyValueToken is PropertyValueToken<T>.Complex complex)
            {
                var nestedSingleValueBuilder = new MultiValuedProtocol.SingleValueBuilder();
                var nextReader = MultiValuedProtocol.ReadComplexPropertyValue(complex.ComplexPropertyValueReader, nestedSingleValueBuilder);

                singleValueBuilder.ComplexProperties.Add(new ComplexResponseProperty(propertyName, nestedSingleValueBuilder.Build(), Enumerable.Empty<string>()));

                return nextReader;
            }
            else if (propertyValueToken is PropertyValueToken<T>.MultiValued multiValued)
            {
                var values = new List<SingleValue>();

                var multiValuedPropertyValueReader = multiValued.MultiValuedPropertyValueReader;
                while (true)
                {
                    var multiValuedPropertyValueToken = multiValuedPropertyValueReader.Next();
                    if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<T>.Object @object)
                    {
                        var complexPropertyValueReader = @object.ComplexPropertyValueReader;
                        var nestedSinlgeValueBuilder = new MultiValuedProtocol.SingleValueBuilder();
                        multiValuedPropertyValueReader = MultiValuedProtocol.ReadComplexPropertyValue(complexPropertyValueReader, nestedSinlgeValueBuilder);
                        values.Add(nestedSinlgeValueBuilder.Build());
                    }
                    else if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<T>.End end)
                    {
                        //// TODO nextlink isn't modeled in the readers
                        singleValueBuilder.MultiValuedProperties.Add(new MultiValuedResponseProperty(propertyName, values, null));

                        return end.Reader;
                    }
                    else
                    {
                        throw new Exception("TODO implement visitor");
                    }
                }
            }
            else if (propertyValueToken is PropertyValueToken<T>.Null @null)
            {
                //// TODO you need to add null properties to `singlevalue`
                return @null.NullPropertyValueReader.Next();
            }
            else if (propertyValueToken is PropertyValueToken<T>.Primitive primitive)
            {
                var primitivePropertyValueReader = primitive.PrimitivePropertyValueReader;
                singleValueBuilder.PrimitiveProperties.Add(new PrimitiveResponseProperty(propertyName, primitivePropertyValueReader.PrimitivePropertyValue.Value, Enumerable.Empty<object>()));
                return primitivePropertyValueReader.Next();
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private sealed class SingleValueBuilder
        {
            public List<ComplexResponseProperty> ComplexProperties { get; set; } = new List<ComplexResponseProperty>();

            public List<MultiValuedResponseProperty> MultiValuedProperties { get; set; } = new List<MultiValuedResponseProperty>();

            public List<UntypedResponseProperty> UntypedProperties { get; set; } = new List<UntypedResponseProperty>();

            public List<PrimitiveResponseProperty> PrimitiveProperties { get; set; } = new List<PrimitiveResponseProperty>();

            public List<DynamicResponseProperty> DynamicProperties { get; set; } = new List<DynamicResponseProperty>();

            public string? Context { get; set; } //// TODO make this strongly typed

            public SingleValue Build()
            {
                return new SingleValue(this.ComplexProperties, this.MultiValuedProperties, this.UntypedProperties, this.PrimitiveProperties, this.DynamicProperties, this.Context);
            }
        }

        private static IGetResponseBodyReader SkipHeaders(IGetResponseHeaderReader getResponseHeaderReader)
        {
            var headerToken = getResponseHeaderReader.Next();
            if (headerToken is GetResponseHeaderToken.ContentType contentType)
            {
                return SkipHeaders(contentType.ContentTypeHeaderReader.Next());
            }
            else if (headerToken is GetResponseHeaderToken.Custom custom)
            {
                var customHeaderToken = custom.CustomHeaderReader.Next();
                if (customHeaderToken is CustomHeaderToken<IGetResponseHeaderReader>.FieldValue fieldValue)
                {
                    return SkipHeaders(fieldValue.HeaderFieldValueReader.Next());
                }
                else if (customHeaderToken is CustomHeaderToken<IGetResponseHeaderReader>.Header header)
                {
                    return SkipHeaders(header.GetHeaderReader);
                }
                else
                {
                    throw new Exception("TODO implement visitor");
                }
            }
            else if (headerToken is GetResponseHeaderToken.Body body)
            {
                return body.GetResponseBodyReader;
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static IEnumerable<Tuple<string, string?>> SplitQueryQueryString(string queryString)
        {
            return MultiValuedProtocol.SplitQueryQueryString(queryString, 1);
        }

        private static IEnumerable<Tuple<string, string?>> SplitQueryQueryString(string queryString, int currentIndex)
        {
            while (true)
            {
                if (currentIndex >= queryString.Length)
                {
                    yield break;
                }

                var queryOptionDelimiterIndex = queryString.IndexOf('&', currentIndex);
                if (queryOptionDelimiterIndex == -1)
                {
                    queryOptionDelimiterIndex = queryString.Length;
                }

                var parameterNameDelimiterIndex = queryString.IndexOf('=', currentIndex, queryOptionDelimiterIndex - currentIndex);
                if (parameterNameDelimiterIndex == -1)
                {
                    yield return Tuple.Create(
                        queryString.Substring(currentIndex, queryOptionDelimiterIndex - currentIndex),
                        (string?)null);
                }
                else
                {
                    var name = queryString.Substring(currentIndex, parameterNameDelimiterIndex - currentIndex);
                    string? value;
                    if (parameterNameDelimiterIndex + 1 == queryOptionDelimiterIndex)
                    {
                        value = null;
                    }
                    else
                    {
                        value = queryString.Substring(parameterNameDelimiterIndex + 1, queryOptionDelimiterIndex - parameterNameDelimiterIndex);
                    }

                    yield return Tuple.Create(
                        name,
                        value);
                }

                currentIndex = queryOptionDelimiterIndex + 1;
            }
        }

        private static async Task<T> WriteUri<T>(Uri uri, IUriWriter<T> uriWriter)
        {
            var schemeWriter = await uriWriter.Commit().ConfigureAwait(false);

            var domainWriter = await schemeWriter.Commit(new UriScheme(uri.Scheme)).ConfigureAwait(false);
            var portWriter = await domainWriter.Commit(new UriDomain(uri.DnsSafeHost)).ConfigureAwait(false);

            IUriPathSegmentWriter<T> uriPathSegmentWriter;
            if (uri.IsDefaultPort)
            {
                uriPathSegmentWriter = await portWriter.Commit().ConfigureAwait(false);
            }
            else
            {
                uriPathSegmentWriter = await portWriter.Commit(new UriPort(uri.Port)).ConfigureAwait(false);
            }

            foreach (var pathSegment in uri.Segments)
            {
                uriPathSegmentWriter = await uriPathSegmentWriter.Commit(new UriPathSegment(pathSegment)).ConfigureAwait(false);
            }

            var queryOptionWriter = await uriPathSegmentWriter.Commit().ConfigureAwait(false);
            foreach (var queryOption in MultiValuedProtocol.SplitQueryQueryString(uri.Query))
            {
                var parameterWriter = await queryOptionWriter.CommitParameter().ConfigureAwait(false);
                var valueWriter = await parameterWriter.Commit(new QueryParameter(queryOption.Item1)).ConfigureAwait(false);
                if (queryOption.Item2 == null)
                {
                    queryOptionWriter = await valueWriter.Commit().ConfigureAwait(false);
                }
                else
                {
                    queryOptionWriter = await valueWriter.Commit(new QueryValue(queryOption.Item2)).ConfigureAwait(false);
                }
            }

            T getHeaderWriter;
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                getHeaderWriter = await queryOptionWriter.Commit().ConfigureAwait(false);
            }
            else
            {
                var fragmentWriter = await queryOptionWriter.CommitFragment().ConfigureAwait(false);
                getHeaderWriter = await fragmentWriter.Commit(new Fragment(uri.Fragment)).ConfigureAwait(false);
            }

            return getHeaderWriter;
        }
    }
}
