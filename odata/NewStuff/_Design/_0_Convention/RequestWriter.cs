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
    }


    public sealed class UriSchemeWriter
    {
        private UriSchemeWriter()
        {
        }

        public UriDomainWriter Commit(UriScheme uriScheme)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriDomainWriter
    {
        private UriDomainWriter()
        {
        }

        public UriDomainWriter Commit(UriDomain uriDomain)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPortWriter
    {
        private UriPortWriter()
        {
        }

        public UriPathSegmentWriter Commit(UriPort uriPort)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPathSegmentWriter
    {
        private UriPathSegmentWriter()
        {
        }

        public UriPathSegmentWriter Commit(UriPathSegment uriPathSegment)
        {
            throw new NotImplementedException("TODO");
        }

        public QueryOptionWriter Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class QueryOptionWriter
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
