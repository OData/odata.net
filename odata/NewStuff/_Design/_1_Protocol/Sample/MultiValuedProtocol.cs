namespace NewStuff._Design._1_Protocol.Sample
{
    using System;

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
                portWriter.Commit()
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
