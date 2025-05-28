namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;

    public sealed class HttpClientConvention : IConvention
    {
        private readonly Func<HttpClient> httpClientFactory;

        public HttpClientConvention(Func<HttpClient> httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public IGetRequestWriter Get()
        {
            return new GetRequestWriter(this.httpClientFactory()); //// TODO you need an idispoabel for the httpclient
        }

        private sealed class GetRequestWriter : IGetRequestWriter
        {
            private readonly HttpClient httpClient;

            public GetRequestWriter(HttpClient httpClient)
            {
                this.httpClient = httpClient;
            }

            public IUriWriter<IGetHeaderWriter> Commit()
            {
                return new UriWriter<GetHeaderWriter>(requestUri => new GetHeaderWriter(this.httpClient, requestUri));
            }

            private sealed class GetHeaderWriter : IGetHeaderWriter
            {
                private readonly HttpClient httpClient;
                private readonly Uri requestUri;

                public GetHeaderWriter(HttpClient httpClient, Uri requestUri)
                {
                    this.httpClient = httpClient;
                    this.requestUri = requestUri;
                }

                public IGetBodyWriter Commit()
                {
                    return new GetBodyWriter(this.httpClient, this.requestUri);
                }

                private sealed class GetBodyWriter : IGetBodyWriter
                {
                    private readonly HttpClient httpClient;
                    private readonly Uri requestUri;

                    public GetBodyWriter(HttpClient httpClient, Uri requestUri)
                    {
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public IGetResponseReader Commit()
                    {
                        //// TODO malformed headers will throw here
                        var httpResponseMessage = this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO you have to implement async before you get too far //// TODO you need an idisposable for the message

                        return new GetResponseReader(httpResponseMessage);
                    }

                    private sealed class GetResponseReader : IGetResponseReader
                    {
                        private readonly HttpResponseMessage httpResponseMessage;

                        public GetResponseReader(HttpResponseMessage httpResponseMessage)
                        {
                            this.httpResponseMessage = httpResponseMessage;
                        }

                        public IGetResponseHeaderReader Next()
                        {
                            return new GetResponseHeaderReader(this.httpResponseMessage, this.httpResponseMessage.Headers.GetEnumerator()); //// TODO this is disposable
                        }

                        private sealed class GetResponseHeaderReader : IGetResponseHeaderReader
                        {
                            private readonly HttpResponseMessage httpResponseMessage;
                            private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                            public GetResponseHeaderReader(HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                            {
                                this.httpResponseMessage = httpResponseMessage;
                                this.headers = headers;
                            }

                            public GetResponseHeaderToken Next()
                            {
                                if (this.headers.MoveNext())
                                {
                                    if (string.Equals(this.headers.Current.Key, "Content-Type", StringComparison.OrdinalIgnoreCase)) //// TODO should ignroing case be configurable
                                    {
                                        return new GetResponseHeaderToken.ContentType(new ContentTypeHeaderReader(this.httpResponseMessage, this.headers));
                                    }
                                    else
                                    {
                                        return new GetResponseHeaderToken.Custom(new CustomHeaderReader(this.httpResponseMessage, this.headers));
                                    }
                                }
                                else
                                {
                                    var jsonDocument = JsonDocument.ParseAsync(this.httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult()).ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO need to be async

                                    var element = jsonDocument.RootElement;
                                    if (element.ValueKind != JsonValueKind.Object)
                                    {
                                        throw new Exception("TODO the root needs to be an object");
                                    }

                                    return new GetResponseHeaderToken.Body(new GetResponseBodyReader(element.EnumerateObject()));
                                }
                            }

                            private sealed class CustomHeaderReader : ICustomHeaderReader<IGetResponseHeaderReader>
                            {
                                private readonly HttpResponseMessage httpResponseMessage;
                                private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                                public CustomHeaderReader(HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                                {
                                    this.httpResponseMessage = httpResponseMessage;
                                    this.headers = headers;
                                    this.HeaderFieldName = new HeaderFieldName(this.headers.Current.Key);
                                }

                                public HeaderFieldName HeaderFieldName { get; }

                                public CustomHeaderToken<IGetResponseHeaderReader> Next()
                                {
                                    if (this.headers.Current.Value.Any())
                                    {
                                        return new CustomHeaderToken<IGetResponseHeaderReader>.FieldValue(new HeaderFieldValueReader(this.httpResponseMessage, this.headers));
                                    }
                                    else
                                    {
                                        return new CustomHeaderToken<IGetResponseHeaderReader>.Header(new GetResponseHeaderReader(this.httpResponseMessage, this.headers));
                                    }
                                }

                                private sealed class HeaderFieldValueReader : IHeaderFieldValueReader<IGetResponseHeaderReader>
                                {
                                    private readonly HttpResponseMessage httpResponseMessage;
                                    private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                                    public HeaderFieldValueReader(HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                                    {
                                        this.httpResponseMessage = httpResponseMessage;
                                        this.headers = headers;
                                        this.HeaderFieldValue = new HeaderFieldValue(headers.Current.Value.First()); //// TODO `.first` is not handling duplicate headers or headers with multiple provided values
                                    }

                                    public HeaderFieldValue HeaderFieldValue { get; }

                                    public IGetResponseHeaderReader Next()
                                    {
                                        return new GetResponseHeaderReader(this.httpResponseMessage, this.headers);
                                    }
                                }
                            }

                            private sealed class ContentTypeHeaderReader : IContentTypeHeaderReader
                            {
                                private readonly HttpResponseMessage httpResponseMessage;
                                private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                                public ContentTypeHeaderReader(HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                                {
                                    this.httpResponseMessage = httpResponseMessage;
                                    this.headers = headers;
                                    this.ContentType = new ContentType(this.headers.Current.Value.First()); //// TODO `.first` is not handling duplicate headers or headers with multiple provided values
                                }

                                public ContentType ContentType { get; }

                                public IGetResponseHeaderReader Next()
                                {
                                    return new GetResponseHeaderReader(this.httpResponseMessage, this.headers);
                                }
                            }

                            private sealed class GetResponseBodyReader : IGetResponseBodyReader
                            {
                                private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                public GetResponseBodyReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                {
                                    this.propertyEnumerator = propertyEnumerator;
                                }

                                public GetResponseBodyToken Next()
                                {
                                    if (this.propertyEnumerator.MoveNext())
                                    {
                                        var property = this.propertyEnumerator.Current;
                                        if (string.Equals(property.Name, "@odata.context")) //// TODO ignore case?
                                        {
                                            return new GetResponseBodyToken.OdataContext(new OdataContextReader(this.propertyEnumerator, property.Value));
                                        }
                                        else if (string.Equals(property.Name, "@odata.nextLink")) //// TODO ignore case?
                                        {
                                            return new GetResponseBodyToken.NextLink(new NextLinkReader(this.propertyEnumerator, property.Value));
                                        }
                                        else
                                        {
                                            return new GetResponseBodyToken.Property(new PropertyReader(this.propertyEnumerator));
                                        }
                                    }
                                    else
                                    {
                                        return GetResponseBodyToken.End.Instance;
                                    }
                                }

                                private sealed class OdataContextReader : IOdataContextReader<IGetResponseBodyReader>
                                {
                                    private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                    public OdataContextReader(JsonElement.ObjectEnumerator propertyEnumerator, JsonElement propertyValue)
                                    {
                                        if (propertyValue.ValueKind != JsonValueKind.String)
                                        {
                                            throw new Exception("TODO the context must be a string");
                                        }

                                        this.OdataContext = new OdataContext(propertyValue.GetString()!);
                                        this.propertyEnumerator = propertyEnumerator;
                                    }

                                    public OdataContext OdataContext { get; }

                                    public IGetResponseBodyReader Next()
                                    {
                                        return new GetResponseBodyReader(this.propertyEnumerator);
                                    }
                                }

                                private sealed class NextLinkReader : INextLinkReader
                                {
                                    private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                    public NextLinkReader(JsonElement.ObjectEnumerator propertyEnumerator, JsonElement propertyValue)
                                    {
                                        if (propertyValue.ValueKind != JsonValueKind.String)
                                        {
                                            throw new Exception("TODO nextlink must be a string");
                                        }

                                        this.NextLink = new NextLink(new Uri(propertyValue.GetString()!));
                                        this.propertyEnumerator = propertyEnumerator;
                                    }

                                    public NextLink NextLink { get; }

                                    public IGetResponseBodyReader Next()
                                    {
                                        return new GetResponseBodyReader(this.propertyEnumerator);
                                    }
                                }

                                private sealed class PropertyReader : IPropertyReader<IGetResponseBodyReader>
                                {
                                    private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                    public PropertyReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                    {
                                        this.propertyEnumerator = propertyEnumerator;
                                    }

                                    public IPropertyNameReader<IGetResponseBodyReader> Next()
                                    {
                                        return new PropertyNameReader(this.propertyEnumerator);
                                    }

                                    private sealed class PropertyNameReader : IPropertyNameReader<IGetResponseBodyReader>
                                    {
                                        private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                        public PropertyNameReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                        {
                                            this.PropertyName = new PropertyName(propertyEnumerator.Current.Name);
                                            this.propertyEnumerator = propertyEnumerator;
                                        }

                                        public PropertyName PropertyName { get; }

                                        public IPropertyValueReader<IGetResponseBodyReader> Next()
                                        {
                                            return new PropertyValueReader(this.propertyEnumerator);
                                        }

                                        private sealed class PropertyValueReader : IPropertyValueReader<IGetResponseBodyReader>
                                        {
                                            private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                            public PropertyValueReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                            {
                                                this.propertyEnumerator = propertyEnumerator;
                                            }

                                            public PropertyValueToken<IGetResponseBodyReader> Next()
                                            {
                                                var propertyValue = this.propertyEnumerator.Current.Value;
                                                if (propertyValue.ValueKind == JsonValueKind.Null)
                                                {
                                                    return new PropertyValueToken<IGetResponseBodyReader>.Null(new NullPropertyValueReader(this.propertyEnumerator));
                                                }
                                                else if (propertyValue.ValueKind == JsonValueKind.Object)
                                                {
                                                    return new PropertyValueToken<IGetResponseBodyReader>.Complex(new ComplexPropertyValueReader(this.propertyEnumerator, propertyValue.EnumerateObject()));
                                                }
                                                else if (propertyValue.ValueKind == JsonValueKind.Array)
                                                {
                                                    return new PropertyValueToken<IGetResponseBodyReader>.MultiValued(new MultiValuedPropertyValueReader(this.propertyEnumerator));
                                                }
                                                else
                                                {
                                                    // primitive
                                                    return new PropertyValueToken<IGetResponseBodyReader>.Primitive(new PrimitivePropertyValueReader(this.propertyEnumerator, propertyValue.GetRawText()));
                                                }
                                            }

                                            private sealed class PrimitivePropertyValueReader : IPrimitivePropertyValueReader<IGetResponseBodyReader>
                                            {
                                                private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                                public PrimitivePropertyValueReader(JsonElement.ObjectEnumerator propertyEnumerator, string value)
                                                {
                                                    this.PrimitivePropertyValue = new PrimitivePropertyValue(value);
                                                    this.propertyEnumerator = propertyEnumerator;
                                                }

                                                public PrimitivePropertyValue PrimitivePropertyValue { get; }

                                                public IGetResponseBodyReader Next()
                                                {
                                                    return new GetResponseBodyReader(this.propertyEnumerator);
                                                }
                                            }

                                            private sealed class ComplexPropertyValueReader : IComplexPropertyValueReader<IGetResponseBodyReader>
                                            {
                                                private readonly JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                                private readonly JsonElement.ObjectEnumerator propertyValueEnumerator;

                                                public ComplexPropertyValueReader(JsonElement.ObjectEnumerator parentPropertyEnumerator, JsonElement.ObjectEnumerator propertyValueEnumerator)
                                                {
                                                    this.parentPropertyEnumerator = parentPropertyEnumerator;
                                                    this.propertyValueEnumerator = propertyValueEnumerator;
                                                }

                                                public ComplexPropertyValueToken<IGetResponseBodyReader> Next()
                                                {
                                                    if (this.propertyValueEnumerator.MoveNext())
                                                    {
                                                        var property = this.propertyValueEnumerator.Current;
                                                        if (string.Equals(property.Name, "@odata.context")) //// TODO ignore case?
                                                        {
                                                            return new ComplexPropertyValueToken<IGetResponseBodyReader>.OdataContext(new OdataContextReader(this.parentPropertyEnumerator, this.propertyValueEnumerator, property.Value));
                                                        }
                                                        else if (string.Equals(property.Name, "@odata.id")) //// TODO ignore case?
                                                        {
                                                            return new ComplexPropertyValueToken<IGetResponseBodyReader>.OdataId(new OdataIdReader(this.parentPropertyEnumerator, this.propertyValueEnumerator, property.Value));
                                                        }

                                                        return new ComplexPropertyValueToken<IGetResponseBodyReader>.Property(new PropertyReader(this.parentPropertyEnumerator, this.propertyValueEnumerator));
                                                    }
                                                    else
                                                    {
                                                        return new ComplexPropertyValueToken<IGetResponseBodyReader>.End(new GetResponseBodyReader(this.parentPropertyEnumerator));
                                                    }
                                                }

                                                private sealed class OdataContextReader : IOdataContextReader<IComplexPropertyValueReader<IGetResponseBodyReader>>
                                                {
                                                    private readonly JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                                    private readonly JsonElement.ObjectEnumerator propertyValueEnumerator;

                                                    public OdataContextReader(JsonElement.ObjectEnumerator parentPropertyEnumerator, JsonElement.ObjectEnumerator propertyValueEnumerator, JsonElement propertyValue)
                                                    {
                                                        if (propertyValue.ValueKind != JsonValueKind.String)
                                                        {
                                                            throw new Exception("TODO the context must be a string");
                                                        }

                                                        this.OdataContext = new OdataContext(propertyValue.GetString()!);
                                                        this.parentPropertyEnumerator = parentPropertyEnumerator;
                                                        this.propertyValueEnumerator = propertyValueEnumerator;
                                                    }

                                                    public OdataContext OdataContext { get; }

                                                    public IComplexPropertyValueReader<IGetResponseBodyReader> Next()
                                                    {
                                                        return new ComplexPropertyValueReader(this.parentPropertyEnumerator, this.propertyValueEnumerator);
                                                    }
                                                }

                                                private sealed class OdataIdReader : IOdataIdReader<IComplexPropertyValueReader<IGetResponseBodyReader>>
                                                {
                                                    private readonly JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                                    private readonly JsonElement.ObjectEnumerator propertyValueEnumerator;

                                                    public OdataIdReader(JsonElement.ObjectEnumerator parentPropertyEnumerator, JsonElement.ObjectEnumerator propertyValueEnumerator, JsonElement propertyValue)
                                                    {
                                                        if (propertyValue.ValueKind != JsonValueKind.String)
                                                        {
                                                            throw new Exception("TODO the context must be a string");
                                                        }

                                                        this.OdataId = new OdataId(propertyValue.GetString()!);
                                                        this.parentPropertyEnumerator = parentPropertyEnumerator;
                                                        this.propertyValueEnumerator = propertyValueEnumerator;
                                                    }

                                                    public OdataId OdataId { get; }

                                                    public IComplexPropertyValueReader<IGetResponseBodyReader> Next()
                                                    {
                                                        return new ComplexPropertyValueReader(this.parentPropertyEnumerator, this.propertyValueEnumerator);
                                                    }
                                                }

                                                private sealed class PropertyReader : IPropertyReader<IComplexPropertyValueReader<IGetResponseBodyReader>>
                                                {
                                                    private readonly JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                                    private readonly JsonElement.ObjectEnumerator propertyValueEnumerator;

                                                    public PropertyReader(JsonElement.ObjectEnumerator parentPropertyEnumerator, JsonElement.ObjectEnumerator propertyValueEnumerator)
                                                    {
                                                        this.parentPropertyEnumerator = parentPropertyEnumerator;
                                                        this.propertyValueEnumerator = propertyValueEnumerator;
                                                    }

                                                    public IPropertyNameReader<IComplexPropertyValueReader<IGetResponseBodyReader>> Next()
                                                    {
                                                        return new PropertyNameReader(this.parentPropertyEnumerator, this.propertyValueEnumerator);
                                                    }

                                                    private sealed class PropertyNameReader : IPropertyNameReader<IComplexPropertyValueReader<IGetResponseBodyReader>>
                                                    {
                                                        private readonly JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                                        private readonly JsonElement.ObjectEnumerator propertyValueEnumerator;

                                                        public PropertyNameReader(JsonElement.ObjectEnumerator parentPropertyEnumerator, JsonElement.ObjectEnumerator propertyValueEnumerator)
                                                        {
                                                            this.PropertyName = new PropertyName(this.propertyValueEnumerator.Current.Name);

                                                            this.parentPropertyEnumerator = parentPropertyEnumerator;
                                                            this.propertyValueEnumerator = propertyValueEnumerator;
                                                        }

                                                        public PropertyName PropertyName { get; }

                                                        public IPropertyValueReader<IComplexPropertyValueReader<IGetResponseBodyReader>> Next()
                                                        {
                                                        }
                                                    }
                                                }
                                            }

                                            private sealed class MultiValuedPropertyValueReader : IMultiValuedPropertyValueReader<IGetResponseBodyReader>
                                            {
                                                private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                                public MultiValuedPropertyValueReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                                {
                                                    this.propertyEnumerator = propertyEnumerator;
                                                }

                                                public MultiValuedPropertyValueToken<IGetResponseBodyReader> Next()
                                                {
                                                }
                                            }

                                            private sealed class NullPropertyValueReader : INullPropertyValueReader<IGetResponseBodyReader>
                                            {
                                                private readonly JsonElement.ObjectEnumerator propertyEnumerator;

                                                public NullPropertyValueReader(JsonElement.ObjectEnumerator propertyEnumerator)
                                                {
                                                    this.propertyEnumerator = propertyEnumerator;
                                                }

                                                public IGetResponseBodyReader Next()
                                                {
                                                    return new GetResponseBodyReader(this.propertyEnumerator);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                public ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader()
                {
                    return new CustomHeaderWriter<GetHeaderWriter>(this.httpClient, this.requestUri, (client, uri) => new GetHeaderWriter(client, uri));
                }

                private sealed class CustomHeaderWriter<T> : ICustomHeaderWriter<T>
                {
                    private readonly HttpClient httpClient;
                    private readonly Uri requestUri;
                    private readonly Func<HttpClient, Uri, T> nextFactory;

                    public CustomHeaderWriter(HttpClient httpClient, Uri requestUri, Func<HttpClient, Uri, T> nextFactory)
                    {
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                        this.nextFactory = nextFactory;
                    }

                    public IHeaderFieldValueWriter<T> Commit(HeaderFieldName headerFieldName)
                    {
                        return new HeaderFieldValueWriter(this.httpClient, this.requestUri, this.nextFactory, headerFieldName.Name);
                    }

                    private sealed class HeaderFieldValueWriter : IHeaderFieldValueWriter<T>
                    {
                        private readonly HttpClient httpClient;
                        private readonly Uri requestUri;
                        private readonly Func<HttpClient, Uri, T> nextFactory;
                        private readonly string headerName;

                        public HeaderFieldValueWriter(HttpClient httpClient, Uri requestUri, Func<HttpClient, Uri, T> nextFactory, string headerName)
                        {
                            this.httpClient = httpClient;
                            this.requestUri = requestUri;
                            this.nextFactory = nextFactory;
                            this.headerName = headerName;
                        }

                        public T Commit()
                        {
                            this.httpClient.DefaultRequestHeaders.Add(this.headerName, (string?)null);

                            return this.nextFactory(this.httpClient, this.requestUri);
                        }

                        public T Commit(HeaderFieldValue headerFieldValue)
                        {
                            this.httpClient.DefaultRequestHeaders.Add(this.headerName, headerFieldValue.Value);

                            return this.nextFactory(this.httpClient, this.requestUri);
                        }
                    }
                }

                public IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize()
                {
                    return new OdataMaxPageSizeHeaderWriter(this.httpClient, this.requestUri);
                }

                private sealed class OdataMaxPageSizeHeaderWriter : IOdataMaxPageSizeHeaderWriter
                {
                    private readonly HttpClient httpClient;
                    private readonly Uri requestUri;

                    public OdataMaxPageSizeHeaderWriter(HttpClient httpClient, Uri requestUri)
                    {
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public IGetHeaderWriter Commit(OdataMaxPageSize odataMaxPageSize)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("OData-MaxPageSize", "100"); //// TODO use `odatamaxpagesize` once the type has actually been implemented

                        return new GetHeaderWriter(this.httpClient, this.requestUri);
                    }
                }

                public IOdataMaxVersionHeaderWriter CommitOdataMaxVersion()
                {
                    return new OdataMaxVersionHeaderWriter(this.httpClient, this.requestUri);
                }

                private sealed class OdataMaxVersionHeaderWriter : IOdataMaxVersionHeaderWriter
                {
                    private readonly HttpClient httpClient;
                    private readonly Uri requestUri;

                    public OdataMaxVersionHeaderWriter(HttpClient httpClient, Uri requestUri)
                    {
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public IGetHeaderWriter Commit(OdataVersion odataVersion)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "v4.01"); //// TODO use `odataversion` once you've actually implemented that type

                        return new GetHeaderWriter(this.httpClient, this.requestUri);
                    }
                }
            }

            private sealed class UriWriter<T> : IUriWriter<T>
            {
                private readonly Func<Uri, T> nextFactory;

                public UriWriter(Func<Uri, T> nextFactory)
                {
                    this.nextFactory = nextFactory;
                }

                public IUriSchemeWriter<T> Commit()
                {
                    return new UriSchemeWriter(new StringBuilder(), this.nextFactory);
                }

                private sealed class UriSchemeWriter : IUriSchemeWriter<T>
                {
                    private readonly StringBuilder builder;
                    private readonly Func<Uri, T> nextFactory;

                    public UriSchemeWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                    {
                        this.builder = builder; //// TODO this means that writer instances won't be re-usable, are you ok with that?
                        this.nextFactory = nextFactory;
                    }

                    public IUriDomainWriter<T> Commit(UriScheme uriScheme)
                    {
                        this.builder.Append($"{uriScheme.Scheme}://");
                        return new UriDomainWriter(this.builder, this.nextFactory);
                    }

                    private sealed class UriDomainWriter : IUriDomainWriter<T>
                    {
                        private readonly StringBuilder builder;
                        private readonly Func<Uri, T> nextFactory;

                        public UriDomainWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                        {
                            this.builder = builder;
                            this.nextFactory = nextFactory;
                        }

                        public IUriPortWriter<T> Commit(UriDomain uriDomain)
                        {
                            this.builder.Append(uriDomain.Domain);
                            return new UriPortWriter(this.builder, this.nextFactory);
                        }

                        private sealed class UriPortWriter : IUriPortWriter<T>
                        {
                            private readonly StringBuilder builder;
                            private readonly Func<Uri, T> nextFactory;

                            public UriPortWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                            {
                                this.builder = builder;
                                this.nextFactory = nextFactory;
                            }

                            public IUriPathSegmentWriter<T> Commit()
                            {
                                return new UriPathSegmentWriter(this.builder, this.nextFactory);
                            }

                            public IUriPathSegmentWriter<T> Commit(UriPort uriPort)
                            {
                                this.builder.Append($":{uriPort.Port}");
                                return new UriPathSegmentWriter(this.builder, this.nextFactory);
                            }

                            private sealed class UriPathSegmentWriter : IUriPathSegmentWriter<T>
                            {
                                private readonly StringBuilder builder;
                                private readonly Func<Uri, T> nextFactory;

                                public UriPathSegmentWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                                {
                                    this.builder = builder;
                                    this.nextFactory = nextFactory;
                                }

                                public IQueryOptionWriter<T> Commit()
                                {
                                    return new QueryOptionWriter(this.builder, this.nextFactory, false);
                                }

                                private sealed class QueryOptionWriter : IQueryOptionWriter<T>
                                {
                                    private readonly StringBuilder builder;
                                    private readonly Func<Uri, T> nextFactory;
                                    private readonly bool queryParametersWritten;

                                    public QueryOptionWriter(StringBuilder builder, Func<Uri, T> nextFactory, bool queryParametersWritten)
                                    {
                                        this.builder = builder;
                                        this.nextFactory = nextFactory;
                                        this.queryParametersWritten = queryParametersWritten;
                                    }

                                    public T Commit()
                                    {
                                        return this.nextFactory(new Uri(this.builder.ToString()));
                                    }

                                    public IFragmentWriter<T> CommitFragment()
                                    {
                                        return new FragmentWriter(this.builder, this.nextFactory);
                                    }

                                    private sealed class FragmentWriter : IFragmentWriter<T>
                                    {
                                        private readonly StringBuilder builder;
                                        private readonly Func<Uri, T> nextFactory;

                                        public FragmentWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                                        {
                                            this.builder = builder;
                                            this.nextFactory = nextFactory;
                                        }

                                        public T Commit(Fragment fragment)
                                        {
                                            this.builder.Append($"#{fragment.Value}");

                                            return this.nextFactory(new Uri(this.builder.ToString()));
                                        }
                                    }

                                    public IQueryParameterWriter<T> CommitParameter()
                                    {
                                        return new QueryParameterWriter(this.builder, this.nextFactory, this.queryParametersWritten);
                                    }

                                    private sealed class QueryParameterWriter : IQueryParameterWriter<T>
                                    {
                                        private readonly StringBuilder builder;
                                        private readonly Func<Uri, T> nextFactory;
                                        private readonly bool queryParametersWritten;

                                        public QueryParameterWriter(StringBuilder builder, Func<Uri, T> nextFactory, bool queryParametersWritten)
                                        {
                                            this.builder = builder;
                                            this.nextFactory = nextFactory;
                                            this.queryParametersWritten = queryParametersWritten;
                                        }

                                        public IQueryValueWriter<T> Commit(QueryParameter queryParameter)
                                        {
                                            if (this.queryParametersWritten)
                                            {
                                                this.builder.Append("&");
                                            }
                                            else
                                            {
                                                this.builder.Append("?");
                                            }

                                            this.builder.Append(queryParameter.Name);

                                            return new QueryValueWriter(this.builder, this.nextFactory);
                                        }

                                        private sealed class QueryValueWriter : IQueryValueWriter<T>
                                        {
                                            private readonly StringBuilder builder;
                                            private readonly Func<Uri, T> nextFactory;

                                            public QueryValueWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                                            {
                                                this.builder = builder;
                                                this.nextFactory = nextFactory;
                                            }

                                            public IQueryOptionWriter<T> Commit()
                                            {
                                                return new QueryOptionWriter(this.builder, this.nextFactory, true);
                                            }

                                            public IQueryOptionWriter<T> Commit(QueryValue queryValue)
                                            {
                                                this.builder.Append($"={queryValue.Value}");

                                                return new QueryOptionWriter(this.builder, this.nextFactory, true);
                                            }
                                        }
                                    }
                                }

                                public IUriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment)
                                {
                                    this.builder.Append($"/{uriPathSegment.Segment}");

                                    return new UriPathSegmentWriter(this.builder, this.nextFactory);
                                }
                            }
                        }
                    }
                }
            }
        }

        public IPatchRequestWriter Patch()
        {
            throw new System.NotImplementedException();
        }

        public IPatchRequestWriter Post()
        {
            throw new System.NotImplementedException();
        }
    }
}
