namespace NewStuff._Design._0_Convention
{
    using System;
    using System.IO;

    public interface IRequestReader
    {
        RequestToken Next();
    }

    public sealed class RequestReader : IRequestReader
    {
        private RequestReader()
        {
        }

        public RequestToken Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public abstract class RequestToken
    {
        private RequestToken()
        {
        }

        public sealed class Get : RequestToken
        {
            private Get()
            {
            }

            public IGetRequestReader GetRequestReader { get; }
        }

        public sealed class Post : RequestToken
        {
            private Post()
            {
            }

            //// TODO
        }

        public sealed class Patch : RequestToken
        {
            private Patch()
            {
            }

            //// TODO
        }
    }

    public interface IGetRequestReader
    {
        IUriReader<IGetHeaderReader> Next();
    }

    public sealed class GetRequestReader : IGetRequestReader
    {
        private GetRequestReader()
        {
        }

        public IUriReader<IGetHeaderReader> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IUriReader<T>
    {
        IUriSchemeReader<T> Next();
    }

    public sealed class UriReader<T> : IUriReader<T>
    {
        private UriReader()
        {
        }

        public IUriSchemeReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IUriSchemeReader<T>
    {
        UriScheme UriScheme { get; }

        IUriDomainReader<T> Next();
    }

    public sealed class UriSchemeReader<T> : IUriSchemeReader<T>
    {
        private UriSchemeReader()
        {
            //// TODO what about relative uris?
        }

        public UriScheme UriScheme { get; }

        public IUriDomainReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriScheme
    {
        internal UriScheme(string scheme)
        {
            Scheme = scheme;
        }

        internal string Scheme { get; } //// TODO there should be more structure to this
    }

    public interface IUriDomainReader<T>
    {
        UriDomain UriDomain { get; }

        UriDomainToken<T> Next();
    }

    public sealed class UriDomainReader<T> : IUriDomainReader<T>
    {
        private UriDomainReader()
        {
        }

        public UriDomain UriDomain { get; }

        public UriDomainToken<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriDomain
    {
        internal UriDomain(string domain)
        {
            Domain = domain;
            //// TODO this could probably still be readers...
        }

        internal string Domain { get; }
    }

    public abstract class UriDomainToken<T>
    {
        private UriDomainToken()
        {
        }

        public sealed class Port : UriDomainToken<T>
        {
            private Port()
            {
            }

            public IUriPortReader<T> UriPortReader { get; }
        }

        public sealed class PathSegment : UriDomainToken<T>
        {
            private PathSegment()
            {
            }

            public IUriPathSegmentReader<T> UriPathSegmentReader { get; }
        }
    }

    public interface IUriPortReader<T>
    {
        UriPort UriPort { get; }

        IUriPathSegmentReader<T> Next();
    }

    public sealed class UriPortReader<T> : IUriPortReader<T>
    {
        private UriPortReader()
        {
        }

        public UriPort UriPort { get; }

        public IUriPathSegmentReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class UriPort
    {
        internal UriPort(int port)
        {
            Port = port;
        }

        internal int Port { get; } //// TODO is this the correct structure?
    }

    public interface IUriPathSegmentReader<T>
    {
        UriPathSegment UriPathSegment { get; }

        PathSegmentToken<T> Next();
    }

    public sealed class UriPathSegmentReader<T> : IUriPathSegmentReader<T>
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
        internal UriPathSegment(string segment)
        {
            Segment = segment;
        }

        internal string Segment { get; } //// TODO do you like this just being a string? aren't there illegal characters?
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

            public IUriPathSegmentReader<T> UriPathSegmentReader { get; }
        }

        public sealed class QueryOption : PathSegmentToken<T>
        {
            private QueryOption()
            {
            }

            public IQueryOptionReader<T> QueryOptionsReader { get; }
        }
    }

    public interface IQueryOptionReader<T>
    {
        QueryOptionToken<T> Next();
    }

    public sealed class QueryOptionReader<T> : IQueryOptionReader<T>
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

            public IQueryParameterReader<T> QueryParameterReader { get; }
        }

        public sealed class Fragment : QueryOptionToken<T>
        {
            private Fragment()
            {
            }

            public IFragmentReader<T> FragmentReader { get; }
        }

        public sealed class Headers : QueryOptionToken<T>
        {
            private Headers()
            {
            }

            public T HeaderReader { get; } //// TODO this name doesn't really make sense given the generic type parameter
        }
    }

    public interface IQueryParameterReader<T>
    {
        QueryParameter QueryParameter { get; }

        QueryParameterToken<T> Next();
    }

    public sealed class QueryParameterReader<T> : IQueryParameterReader<T>
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
        internal QueryParameter(string name)
        {
            Name = name;
        }

        internal string Name { get; } //// TODO is this ok? illegal characters? do you want to differentiate between system options and custom options?
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

            public IQueryValueReader<T> QueryValueReader { get; }
        }

        public sealed class QueryOption : QueryParameterToken<T>
        {
            private QueryOption()
            {
            }

            public IQueryOptionReader<T> QueryOptionReader { get; }
        }

        //// TODO would it make sense to have a "fragment" member? because currently (the scenario being you have a queryparameter (i.e. no value provided) that's immediately followed by the fragment) what happens is you get parameter reader, you call next to get a queryoptionreader, you call next to get a fragment reader; you could skip the intermediate queryoptionreader.next...
    }

    public interface IQueryValueReader<T>
    {
        QueryValue QueryValue { get; }

        IQueryOptionReader<T> Next();
    }

    public sealed class QueryValueReader<T> : IQueryValueReader<T>
    {
        private QueryValueReader()
        {
        }

        public QueryValue QueryValue { get; }

        public IQueryOptionReader<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class QueryValue
    {
        internal QueryValue(string value)
        {
            Value = value;
        }

        internal string Value { get; } //// TODO illegal characters? parsing of system options?
    }

    public interface IFragmentReader<T>
    {
        Fragment Fragment { get; }

        T Next();
    }

    public sealed class FragmentReader<T> : IFragmentReader<T>
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
        internal Fragment(string value)
        {
            Value = value;
        }

        internal string Value { get; } //// TODO do you need more structure here?
    }























    public interface IGetHeaderReader
    {
        GetHeaderToken Next();
    }

    public sealed class GetHeaderReader : IGetHeaderReader
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

            public IOdataMaxVersionHeaderReader OdataMaxVersionHeaderReader { get; }
        }

        public sealed class OdataMaxPageSize : GetHeaderToken
        {
            private OdataMaxPageSize()
            {
            }

            public IOdataMaxPageSizeHeaderReader OdataMaxPageSizeHeaderReader { get; }
        }

        public sealed class Custom : GetHeaderToken
        {
            private Custom()
            {
            }

            public ICustomHeaderReader<IGetHeaderReader> CustomHeaderReader { get; }
        }

        public sealed class GetBody : GetHeaderToken
        {
            private GetBody()
            {
            }

            public IGetBodyReader GetBodyReader { get; }
        }
    }

    public interface IOdataMaxVersionHeaderReader
    {
        OdataVersion OdataVersion { get; }

        IGetHeaderReader Next();
    }

    public sealed class OdataMaxVersionHeaderReader : IOdataMaxVersionHeaderReader
    {
        private OdataMaxVersionHeaderReader()
        {
        }

        public OdataVersion OdataVersion { get; }

        public IGetHeaderReader Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class OdataVersion
    {
        private OdataVersion()
        {
        }

        //// TODO this should be a discriminated union with like an "unknown" member of something
    }

    public interface IOdataMaxPageSizeHeaderReader
    {
        OdataMaxPageSize OdataMaxPageSize { get; }

        IGetHeaderReader Next();
    }

    public sealed class OdataMaxPageSizeHeaderReader : IOdataMaxPageSizeHeaderReader
    {
        private OdataMaxPageSizeHeaderReader()
        {
        }

        public OdataMaxPageSize OdataMaxPageSize { get; }

        public IGetHeaderReader Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class OdataMaxPageSize
    {
        private OdataMaxPageSize()
        {
        }
    }

    public interface ICustomHeaderReader<T>
    {
        HeaderFieldName HeaderFieldName { get; }

        CustomHeaderToken<T> Next();
    }

    public sealed class CustomHeaderReader<T> : ICustomHeaderReader<T>
    {
        private CustomHeaderReader()
        {
        }

        public HeaderFieldName HeaderFieldName { get; }

        public CustomHeaderToken<T> Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class HeaderFieldName
    {
        private HeaderFieldName()
        {
        }
    }

    public abstract class CustomHeaderToken<T>
    {
        private CustomHeaderToken()
        {
        }

        public sealed class FieldValue : CustomHeaderToken<T>
        {
            private FieldValue()
            {
            }

            public IHeaderFieldValueReader<T> HeaderFieldValueReader { get; }
        }

        public sealed class Header : CustomHeaderToken<T>
        {
            private Header()
            {
            }

            public T GetHeaderReader { get; }
        }
    }

    public interface IHeaderFieldValueReader<T>
    {
        HeaderFieldValue HeaderFieldValue { get; }

        T Next();
    }

    public sealed class HeaderFieldValueReader<T> : IHeaderFieldValueReader<T>
    {
        private HeaderFieldValueReader()
        {
        }

        public HeaderFieldValue HeaderFieldValue { get; }

        public T Next()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public sealed class HeaderFieldValue
    {
        private HeaderFieldValue()
        {
        }
    }

























    public interface IGetBodyReader
    {
        GetBody GetBody { get; }
    }

    public sealed class GetBodyReader : IGetBodyReader
    {
        private GetBodyReader()
        {
        }

        public GetBody GetBody { get; }
    }

    public sealed class GetBody
    {
        private GetBody()
        {
        }

        public Stream Data { get; } //// TODO there shouldn't be a body for a get request, but HTTP does allow it; i didn't add properties to any of the other "terminal" nodes, but this one seemed like it was worth setting a precedent for
    }





































    public interface IPatchHeadersReader
    {
    }

    public interface IPatchBodyReader
    {
    }

    public interface IPostHeadersReader
    {
    }

    public interface IPostBodyReader
    {
    }
}
