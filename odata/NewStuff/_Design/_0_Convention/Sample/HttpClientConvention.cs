namespace NewStuff._Design._0_Convention.Sample
{
    using System;
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
                    throw new NotImplementedException();
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

                        return new GetResponseReader();
                    }

                    private sealed class GetResponseReader : IGetResponseReader
                    {
                        public IGetResponseHeaderReader Next()
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

                public ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader()
                {
                    throw new NotImplementedException();
                }

                public IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize()
                {
                    throw new NotImplementedException();
                }

                public IOdataMaxVersionHeaderWriter CommitOdataMaxVersion()
                {
                    throw new NotImplementedException();
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
