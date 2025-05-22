namespace NewStuff._Design._0_Convention
{
    using System;

    public sealed class RequestWriter
    {
        private RequestWriter()
        {
        }

        public GetRequestWriter CommitGet()
        {
            throw new NotImplementedException("TODO");
        }

        public PatchRequestWriter CommitPatch()
        {
            throw new NotImplementedException("TODO");
        }

        public PostRequestWriter CommitPost()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class GetRequestWriter
    {
        private GetRequestWriter()
        {
        }

        public UriWriter<GetHeaderWriter> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class PatchRequestWriter
    {
        private PatchRequestWriter()
        {
        }

        //// TODO
    }

    public sealed class PostRequestWriter
    {
        private PostRequestWriter()
        {
        }
        
        //// TODO
    }

    public sealed class UriWriter<T>
    {
        private UriWriter()
        {
        }

        public UriSchemeWriter<T> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }


    public sealed class UriSchemeWriter<T>
    {
        private UriSchemeWriter()
        {
        }

        public UriDomainWriter<T> Commit(UriScheme uriScheme) //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriDomainWriter<T>
    {
        private UriDomainWriter()
        {
        }

        public UriPortWriter<T> Commit(UriDomain uriDomain)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPortWriter<T>
    {
        private UriPortWriter()
        {
        }

        public UriPathSegmentWriter<T> Commit(UriPort uriPort)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPathSegmentWriter<T>
    {
        private UriPathSegmentWriter()
        {
        }

        public UriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment)
        {
            throw new NotImplementedException("TODO");
        }

        public QueryOptionWriter<T> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class QueryOptionWriter<T>
    {
        private QueryOptionWriter()
        {
        }
    }


























    public sealed class GetHeaderWriter
    {
        private GetHeaderWriter()
        {
        }
    }
}
