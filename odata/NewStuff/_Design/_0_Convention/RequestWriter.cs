namespace NewStuff._Design._0_Convention
{
    using System;

    public sealed class RequestWriter
    {
        private RequestWriter()
        {
        }

        public VerbWriter Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public abstract class VerbWriter
    {
        private VerbWriter()
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
}
