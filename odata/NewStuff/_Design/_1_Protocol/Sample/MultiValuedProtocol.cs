namespace NewStuff._Design._1_Protocol.Sample
{
    using System;
    using System.Collections.Generic;
    using __GeneratedOdata.CstNodes.Rules;
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

            public GetMultiValuedProtocol(IConvention convention, Uri multiValuedUri)
            {
                this.convention = convention;
                this.multiValuedUri = multiValuedUri;
            }

            private sealed class MultiValuedResponseBuilder
            {
                public IReadOnlyList<SingleValue> Value { get; set; }

                public Uri? NextLink { get; set; } //// TODO make this strongly-typed

                public int? Count { get; set; } //// TODO does this need to be strongly-typed? (maybe to avoid nullable?)

                public IEnumerable<object> Annotations { get; set; } //// TODO make this strongly-typed

                public MultiValuedResponse Build()
                {
                    return new MultiValuedResponse(this.Value, this.NextLink, this.Count, this.Annotations);
                }
            }

            public MultiValuedResponse Evaluate()
            {
                var requestWriter = this.convention.Get();
                var uriWriter = requestWriter.Commit();
                var schemeWriter = uriWriter.Commit();
                var domainWriter = schemeWriter.Commit(new UriScheme(this.multiValuedUri.Scheme));
                var portWriter = domainWriter.Commit(new UriDomain(this.multiValuedUri.DnsSafeHost));

                IUriPathSegmentWriter<IGetHeaderWriter> uriPathSegmentWriter;
                if (this.multiValuedUri.IsDefaultPort)
                {
                    uriPathSegmentWriter = portWriter.Commit();
                }
                else
                {
                    uriPathSegmentWriter = portWriter.Commit(new UriPort(this.multiValuedUri.Port));
                }

                foreach (var pathSegment in this.multiValuedUri.Segments)
                {
                    uriPathSegmentWriter = uriPathSegmentWriter.Commit(new UriPathSegment(pathSegment));
                }

                var queryOptionWriter = uriPathSegmentWriter.Commit();
                foreach (var queryOption in Split(this.multiValuedUri.Query))
                {
                    var parameterWriter = queryOptionWriter.CommitParameter();
                    var valueWriter = parameterWriter.Commit(new QueryParameter(queryOption.Item1));
                    if (queryOption.Item2 == null)
                    {
                        queryOptionWriter = valueWriter.Commit();
                    }
                    else
                    {
                        queryOptionWriter = valueWriter.Commit(new QueryValue(queryOption.Item2));
                    }
                }

                IGetHeaderWriter getHeaderWriter;
                if (string.IsNullOrEmpty(this.multiValuedUri.Fragment))
                {
                    getHeaderWriter = queryOptionWriter.Commit();
                }
                else
                {
                    var fragmentWriter = queryOptionWriter.CommitFragment();
                    getHeaderWriter = fragmentWriter.Commit(new Fragment(this.multiValuedUri.Fragment));
                }

                //// TODO not writing any headers...
                var getBodyWriter = getHeaderWriter.Commit();

                // wait for a response
                var getResponseReader = getBodyWriter.Commit(); //// TODO this should be async

                var multiValueResponseBuilder = new MultiValuedResponseBuilder();

                var headerReader = getResponseReader.Next();
                var getResponseBodyReader = SkipHeaders(headerReader);

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
                            //// TODO you are here
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

            private static IGetResponseBodyReader SkipPropertyValue(IPropertyValueReader<IGetResponseBodyReader> propertyValueReader)
            {
                var propertyValueToken = propertyValueReader.Next(); //// TODO do you want to add `skip` methods?
                if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.Complex complex)
                {
                    return SkipComplexPropertyValue(complex.ComplexPropertyValueReader);
                }
                else if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.MultiValued multiValued)
                {
                    var multiValuedPropertyValueReader = multiValued.MultiValuedPropertyValueReader;
                    while (true)
                    {
                        var multiValuedPropertyValueToken = multiValuedPropertyValueReader.Next();
                        if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<IGetResponseBodyReader>.Object @object)
                        {
                            multiValuedPropertyValueReader = SkipComplexPropertyValue(@object.ComplexPropertyValueReader);
                        }
                        else if (multiValuedPropertyValueToken is MultiValuedPropertyValueToken<IGetResponseBodyReader>.End end)
                        {
                            return end.Reader;
                        }
                    }
                }
                else if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.Null @null)
                {
                    var nullPropertyValueReader = @null.NullPropertyValueReader;
                    return nullPropertyValueReader.Next();
                }
                else if (propertyValueToken is PropertyValueToken<IGetResponseBodyReader>.Primitive primitive)
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
                }
            }

            private static IComplexPropertyValueReader<T> SkipNestedProperty<T>(IPropertyReader<IComplexPropertyValueReader<T>> propertyReader)
            {
                var propertyReader = property.PropertyReader;
                var propertyNameReader = propertyReader.Next();
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

            private static IEnumerable<Tuple<string, string?>> Split(string queryString)
            {
                return Split(queryString, 0);
            }

            private static IEnumerable<Tuple<string, string?>> Split(string queryString, int currentIndex)
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

                    var parameterNameDelimiterIndex = queryString.IndexOf('=', currentIndex, queryOptionDelimiterIndex - currentIndex + 1);
                    if (parameterNameDelimiterIndex == -1)
                    {
                        yield return Tuple.Create(
                            queryString.Substring(currentIndex, parameterNameDelimiterIndex - currentIndex + 1),
                            (string?)null);
                    }
                    else
                    {
                        yield return Tuple.Create(
                            queryString.Substring(currentIndex, parameterNameDelimiterIndex - currentIndex + 1),
                            (string?)queryString.Substring(parameterNameDelimiterIndex + 1, queryOptionDelimiterIndex - parameterNameDelimiterIndex));
                    }

                    currentIndex = queryOptionDelimiterIndex + 1;
                }
            }

            public IGetMultiValuedProtocol Expand(object expander)
            {
                throw new System.NotImplementedException();
            }

            public IGetMultiValuedProtocol Filter(object predicate)
            {
                throw new System.NotImplementedException();
            }

            public IGetMultiValuedProtocol Select(object selector)
            {
                throw new System.NotImplementedException();
            }

            public IGetMultiValuedProtocol Skip(object count)
            {
                throw new System.NotImplementedException();
            }
        }

        public IGetSingleValuedProtocol Get(KeyPredicate key)
        {
            throw new System.NotImplementedException();
        }

        public IPatchSingleValuedProtocol Patch(KeyPredicate key, SingleValuedRequest request)
        {
            throw new System.NotImplementedException();
        }

        public IPostSingleValuedProtocol Post(SingleValuedRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
