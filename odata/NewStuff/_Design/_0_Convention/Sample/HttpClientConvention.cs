namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.Net.Http;

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
                    throw new System.NotImplementedException();
                }

                private sealed class UriSchemeWriter : IUriSchemeWriter<T>
                {
                    public IUriDomainWriter<T> Commit(UriScheme uriScheme)
                    {
                        throw new System.NotImplementedException();
                    }

                    private sealed class UriDomainWriter : IUriDomainWriter<T>
                    {
                        public IUriPortWriter<T> Commit(UriDomain uriDomain)
                        {
                            throw new System.NotImplementedException();
                        }

                        private sealed class UriPortWriter : IUriPortWriter<T>
                        {
                            public IUriPathSegmentWriter<T> Commit()
                            {
                                throw new System.NotImplementedException();
                            }

                            public IUriPathSegmentWriter<T> Commit(UriPort uriPort)
                            {
                                throw new System.NotImplementedException();
                            }

                            private sealed class UriPathSegmentWriter : IUriPathSegmentWriter<T>
                            {
                                public IQueryOptionWriter<T> Commit()
                                {
                                    throw new System.NotImplementedException();
                                }

                                private sealed class QueryOptionWriter : IQueryOptionWriter<T>
                                {
                                    public T Commit()
                                    {
                                        throw new System.NotImplementedException();
                                    }

                                    public IFragmentWriter<T> CommitFragment()
                                    {
                                        throw new System.NotImplementedException();
                                    }

                                    private sealed class FragmentWriter : IFragmentWriter<T>
                                    {
                                        public T Commit(Fragment fragment)
                                        {
                                            throw new System.NotImplementedException();
                                        }
                                    }

                                    public IQueryParameterWriter<T> CommitParameter()
                                    {
                                        throw new System.NotImplementedException();
                                    }

                                    private sealed class QueryParameterWriter : IQueryParameterWriter<T>
                                    {
                                        public IQueryValueWriter<T> Commit(QueryParameter queryParameter)
                                        {
                                            throw new System.NotImplementedException();
                                        }

                                        private sealed class QueryValueWriter : IQueryValueWriter<T>
                                        {
                                            public IQueryOptionWriter<T> Commit()
                                            {
                                                throw new System.NotImplementedException();
                                            }

                                            public IQueryOptionWriter<T> Commit(QueryValue queryValue)
                                            {
                                                throw new System.NotImplementedException();
                                            }
                                        }
                                    }
                                }

                                public IUriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment)
                                {
                                    throw new System.NotImplementedException();
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
