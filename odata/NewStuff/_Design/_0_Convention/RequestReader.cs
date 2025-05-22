namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Diagnostics.Contracts;

    public sealed class RequestReader
    {
        public VerbReader Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public abstract class VerbReader
    {
        private VerbReader()
        {
        }

        public sealed class Get : VerbReader
        {
            public UriReader<GetHeaderReader> Next()
            {
                throw new NotImplementedException("TODO");
            }
        }

        public sealed class Post : VerbReader
        {
        }

        public sealed class Patch : VerbReader
        {
        }
    }

    public sealed class UriReader<T>
    {
        private UriReader()
        {
        }

        public UriSchemeReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriSchemeReader<T>
    {
        private UriSchemeReader()
        {
            //// TODO what about relative uris?
        }

        public UriScheme UriScheme { get; }

        public UriDomainReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriScheme
    {
        private UriScheme()
        {
        }
    }

    public sealed class UriDomainReader<T>
    {
        private UriDomainReader()
        {
        }

        public UriDomain UriDomain { get; }

        public UriPortReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriDomain
    {
        private UriDomain()
        {
            //// TODO this could probably still be readers...
        }
    }

    public sealed class UriPortReader<T>
    {
        private UriPortReader()
        {
        }

        public UriPort UriPort { get; }

        public UriPathSegmentReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPort
    {
        private UriPort()
        {
        }
    }

    public sealed class UriPathSegmentReader<T>
    {
        private UriPathSegmentReader()
        {
        }

        public UriPathSegment UriPathSegment { get; }

        public PathSegmentToken<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPathSegment
    {
        private UriPathSegment()
        {
        }
    }

    public abstract class PathSegmentToken<T>
    {
        private PathSegmentToken()
        {
        }

        public sealed class PathSegment : PathSegmentToken<T>
        {
            private PathSegment()
            {
            }

            public UriPathSegmentReader<T> UriPathSegmentReader { get; }
        }

        public sealed class QueryOptions : PathSegmentToken<T>
        {
            private QueryOptions()
            {
            }

            public QueryOptionReader<T> QueryOptionsReader { get; }
        }
    }

    public sealed class QueryOptionReader<T>
    {
        private QueryOptionReader()
        {
        }

        public QueryOptionToken<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public abstract class QueryOptionToken<T>
    {
        private QueryOptionToken()
        {
        }

        public sealed class QueryParameter : QueryOptionToken<T>
        {
            private QueryParameter()
            {
            }

            public QueryParameterReader<T> Next()
            {
                throw new NotImplementedException("TODO");
            }
        }

        public sealed class Fragment : QueryOptionToken<T>
        {
            private Fragment()
            {
            }

            public FragmentReader<T> FragmentReader { get; }
        }
    }

    public sealed class QueryParameterReader<T>
    {
        private QueryParameterReader()
        {
        }

        public QueryParameter QueryParameter { get; }

        public QueryParameterToken<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class QueryParameter
    {
        private QueryParameter()
        {
        }
    }

    public abstract class QueryParameterToken<T>
    {
        private QueryParameterToken()
        {
        }

        public sealed class QueryValue : QueryParameterToken<T>
        {
            private QueryValue()
            {
            }

            public QueryValueReader<T> QueryValueReader { get; }
        }

        public sealed class QueryOption : QueryParameterToken<T>
        {
            private QueryOption()
            {
            }

            public QueryOptionReader<T> QueryOptionReader { get; }
        }

        //// TODO would it make sense to have a "fragment" member? because currently (the scenario being you have a queryparameter (i.e. no value provided) that's immediately followed by the fragment) what happens is you get parameter reader, you call next to get a queryoptionreader, you call next to get a fragment reader; you could skip the intermediate queryoptionreader.next...
    }

    public sealed class QueryValueReader<T>
    {
        private QueryValueReader()
        {
        }

        public QueryValue QueryValue { get; }

        public QueryOptionReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class QueryValue
    {
        private QueryValue()
        {
        }
    }

    public sealed class FragmentReader<T>
    {
        private FragmentReader()
        {
        }

        public Fragment Fragment { get; }

        public T Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class Fragment
    {
        private Fragment()
        {
        }
    }






















    public sealed class GetHeaderReader
    {
        private GetHeaderReader()
        {
        }

        public GetHeaderToken Next() //// TODO as written, it's possible for no headers to be specified; i don't know if the standard allows that; if it *doesn't*, then there needs to be something like a `getheadersstartreader` that has a `getheadersstarttoken` that has the same members as `getheaderstoken`, but *doesn't* have the `getbody` member
        {
            throw new NotImplementedException("TODO");
        }
    }

    public abstract class GetHeaderToken
    {
        private GetHeaderToken()
        {
        }

        public sealed class OdataMaxVersion : GetHeaderToken
        {
            private OdataMaxVersion()
            {
            }

            public OdataMaxVersionHeaderReader  OdataMaxVersionHeaderReader { get; }
        }

        public sealed class OdataMaxPageSize : GetHeaderToken
        {
            private OdataMaxPageSize()
            {
            }

            public OdataMaxPageSizeHeaderReader OdataMaxPageSizeHeaderReader { get; }
        }

        public sealed class Custom : GetHeaderToken
        {
            private Custom()
            {
            }

            public CustomHeaderReader CustomHeaderReader { get; }
        }

        public sealed class GetBody : GetHeaderToken
        {
            private GetBody()
            {
            }

            public GetBodyReader GetBodyReader { get; }
        }
    }

    public sealed class OdataMaxVersionHeaderReader
    {
        private OdataMaxVersionHeaderReader()
        {
        }

        public OdataVersion OdataVersion { get; }

        public GetHeaderReader Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class OdataVersion
    {
        private OdataVersion()
        {
        }
    }

    public sealed class OdataMaxPageSizeHeaderReader
    {
        private OdataMaxPageSizeHeaderReader()
        {
        }
    }

    public sealed class CustomHeaderReader
    {
        private CustomHeaderReader()
        {
        }
    }































    public sealed class GetBodyReader
    {
        private GetBodyReader()
        {
        }
    }

    public sealed class PatchHeadersReader
    {
        private PatchHeadersReader()
        {
        }
    }

    public sealed class PatchBodyReader
    {
        private PatchBodyReader()
        {
        }
    }

    public sealed class PostHeadersReader
    {
        private PostHeadersReader()
        {
        }
    }

    public sealed class PostBodyReader
    {
        private PostBodyReader()
        {
        }
    }
}
