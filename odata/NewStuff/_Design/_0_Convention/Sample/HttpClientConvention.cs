namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;

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
                        var httpResponseMessage = this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO you have to implement async before you get too far //// TODO you need an idisposable for the message

                        return new GetResponseReader(httpResponseMessage);
                    }

                    private sealed class GetResponseReader : IGetResponseReader
                    {
                        private readonly HttpResponseMessage httpResponseMessage;
                        private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                        public GetResponseReader(HttpResponseMessage httpResponseMessage)
                        {
                            this.httpResponseMessage = httpResponseMessage;
                        }

                        public IGetResponseHeaderReader Next()
                        {
                            return new GetResponseHeaderReader(this.httpResponseMessage.Headers.GetEnumerator()); //// TODO this is disposable
                        }

                        private sealed class GetResponseHeaderReader : IGetResponseHeaderReader
                        {
                            private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                            public GetResponseHeaderReader(IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                            {
                                this.headers = headers;
                            }

                            public GetResponseHeaderToken Next()
                            {
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
