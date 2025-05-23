namespace NewStuff._Design._1_Protocol.Sample
{
    using System;
    using System.Collections.Generic;
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

                throw new Exception("tODO");
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
