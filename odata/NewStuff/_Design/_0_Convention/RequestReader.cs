namespace NewStuff._Design._0_Convention
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IRequestReader
    {
        Task<RequestToken> Next();
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

    public interface IGetRequestReader : IAsyncDisposable
    {
        Task<IUriReader<IGetHeaderReader>> Next();
    }

    public interface IUriReader<T>
    {
        Task<IUriSchemeReader<T>> Next();
    }

    public interface IUriSchemeReader<T>
    {
        UriScheme UriScheme { get; }

        Task<IUriDomainReader<T>> Next();
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

        Task<UriDomainToken<T>> Next();
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

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(Port node);

            Task<TResult> Accept(PathSegment node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is Port port)
            {
                return await accepter.Accept(port).ConfigureAwait(false);
            }
            else if (this is PathSegment pathSegment)
            {
                return await accepter.Accept(pathSegment).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<Port, Task<TResult>> portAccepter;
            private readonly Func<PathSegment, Task<TResult>> pathSegmentAccepter;

            public DelegateAccepter(
                Func<Port, Task<TResult>> portAccepter,
                Func<PathSegment, Task<TResult>> pathSegmentAccepter)
            {
                this.portAccepter = portAccepter;
                this.pathSegmentAccepter = pathSegmentAccepter;
            }

            public Task<TResult> Accept(Port node)
            {
                return this.portAccepter(node);
            }

            public Task<TResult> Accept(PathSegment node)
            {
                return this.pathSegmentAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<Port, Task<TResult>> portAccepter,
            Func<PathSegment, Task<TResult>> pathSegmentAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    portAccepter,
                    pathSegmentAccepter));
        }
    }

    public interface IUriPortReader<T>
    {
        UriPort UriPort { get; }

        Task<IUriPathSegmentReader<T>> Next();
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

        Task<PathSegmentToken<T>> Next();
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

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(PathSegment node);

            Task<TResult> Accept(QueryOption node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is PathSegment pathSegment)
            {
                return await accepter.Accept(pathSegment).ConfigureAwait(false);
            }
            else if (this is QueryOption queryOption)
            {
                return await accepter.Accept(queryOption).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<PathSegment, Task<TResult>> pathSegmentAccepter;
            private readonly Func<QueryOption, Task<TResult>> queryOptionAccepter;

            public DelegateAccepter(
                Func<PathSegment, Task<TResult>> pathSegmentAccepter,
                Func<QueryOption, Task<TResult>> queryOptionAccepter)
            {
                this.pathSegmentAccepter = pathSegmentAccepter;
                this.queryOptionAccepter = queryOptionAccepter;
            }

            public Task<TResult> Accept(PathSegment node)
            {
                return this.pathSegmentAccepter(node);
            }

            public Task<TResult> Accept(QueryOption node)
            {
                return this.queryOptionAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<PathSegment, Task<TResult>> pathSegmentAccepter,
            Func<QueryOption, Task<TResult>> queryOptionAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    pathSegmentAccepter,
                    queryOptionAccepter));
        }
    }

    public interface IQueryOptionReader<T>
    {
        Task<QueryOptionToken<T>> Next();
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

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(QueryParameter node);

            Task<TResult> Accept(Fragment node);

            Task<TResult> Accept(Headers node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is QueryParameter queryParameter)
            {
                return await accepter.Accept(queryParameter).ConfigureAwait(false);
            }
            else if (this is Fragment fragment)
            {
                return await accepter.Accept(fragment).ConfigureAwait(false);
            }
            else if (this is Headers headers)
            {
                return await accepter.Accept(headers).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<QueryParameter, Task<TResult>> queryParameterAccepter;
            private readonly Func<Fragment, Task<TResult>> fragmentAccepter;
            private readonly Func<Headers, Task<TResult>> headersAccepter;

            public DelegateAccepter(
                Func<QueryParameter, Task<TResult>> queryParameterAccepter,
                Func<Fragment, Task<TResult>> fragmentAccepter,
                Func<Headers, Task<TResult>> headersAccepter)
            {
                this.queryParameterAccepter = queryParameterAccepter;
                this.fragmentAccepter = fragmentAccepter;
                this.headersAccepter = headersAccepter;
            }

            public Task<TResult> Accept(QueryParameter node)
            {
                return this.queryParameterAccepter(node);
            }

            public Task<TResult> Accept(Fragment node)
            {
                return this.fragmentAccepter(node);
            }

            public Task<TResult> Accept(Headers node)
            {
                return this.headersAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<QueryParameter, Task<TResult>> queryParameterAccepter,
            Func<Fragment, Task<TResult>> fragmentAccepter,
            Func<Headers, Task<TResult>> headersAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    queryParameterAccepter,
                    fragmentAccepter,
                    headersAccepter));
        }
    }

    public interface IQueryParameterReader<T>
    {
        QueryParameter QueryParameter { get; }

        Task<QueryParameterToken<T>> Next();
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

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(QueryValue node);

            Task<TResult> Accept(QueryOption node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is QueryValue QueryValue)
            {
                return await accepter.Accept(QueryValue).ConfigureAwait(false);
            }
            else if (this is QueryOption QueryOption)
            {
                return await accepter.Accept(QueryOption).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<QueryValue, Task<TResult>> queryValueAccepter;
            private readonly Func<QueryOption, Task<TResult>> queryOptionAccepter;

            public DelegateAccepter(
                Func<QueryValue, Task<TResult>> queryValueAccepter,
                Func<QueryOption, Task<TResult>> queryOptionAccepter)
            {
                this.queryValueAccepter = queryValueAccepter;
                this.queryOptionAccepter = queryOptionAccepter;
            }

            public Task<TResult> Accept(QueryValue node)
            {
                return this.queryValueAccepter(node);
            }

            public Task<TResult> Accept(QueryOption node)
            {
                return this.queryOptionAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<QueryValue, Task<TResult>> queryValueAccepter,
            Func<QueryOption, Task<TResult>> queryOptionAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    queryValueAccepter,
                    queryOptionAccepter));
        }
    }

    public interface IQueryValueReader<T>
    {
        QueryValue QueryValue { get; }

        Task<IQueryOptionReader<T>> Next();
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

        Task<T> Next();
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
        Task<GetHeaderToken> Next(); //// TODO as written, it's possible for no headers to be specified; i don't know if the standard allows that; if it *doesn't*, then there needs to be something like a `getheadersstartreader` that has a `getheadersstarttoken` that has the same members as `getheaderstoken`, but *doesn't* have the `getbody` member
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

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(OdataMaxVersion node);

            Task<TResult> Accept(OdataMaxPageSize node);

            Task<TResult> Accept(Custom node);

            Task<TResult> Accept(GetBody node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is OdataMaxVersion odataMaxVersion)
            {
                return await accepter.Accept(odataMaxVersion).ConfigureAwait(false);
            }
            else if (this is OdataMaxPageSize odataMaxPageSize)
            {
                return await accepter.Accept(odataMaxPageSize).ConfigureAwait(false);
            }
            else if (this is Custom custom)
            {
                return await accepter.Accept(custom).ConfigureAwait(false);
            }
            else if (this is GetBody getBody)
            {
                return await accepter.Accept(getBody).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<OdataMaxVersion, Task<TResult>> odataMaxVersionAccepter;
            private readonly Func<OdataMaxPageSize, Task<TResult>> odataMaxPageSizeAccepter;
            private readonly Func<Custom, Task<TResult>> customAccepter;
            private readonly Func<GetBody, Task<TResult>> getBodyAccepter;

            public DelegateAccepter(
                Func<OdataMaxVersion, Task<TResult>> odataMaxVersionAccepter,
                Func<OdataMaxPageSize, Task<TResult>> odataMaxPageSizeAccepter,
                Func<Custom, Task<TResult>> customAccepter,
                Func<GetBody, Task<TResult>> getBodyAccepter)
            {
                this.odataMaxVersionAccepter = odataMaxVersionAccepter;
                this.odataMaxPageSizeAccepter = odataMaxPageSizeAccepter;
                this.customAccepter = customAccepter;
                this.getBodyAccepter = getBodyAccepter;
            }

            public Task<TResult> Accept(OdataMaxVersion node)
            {
                return this.odataMaxVersionAccepter(node);
            }

            public Task<TResult> Accept(OdataMaxPageSize node)
            {
                return this.odataMaxPageSizeAccepter(node);
            }

            public Task<TResult> Accept(Custom node)
            {
                return this.customAccepter(node);
            }

            public Task<TResult> Accept(GetBody node)
            {
                return this.getBodyAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<OdataMaxVersion, Task<TResult>> odataMaxVersionAccepter,
            Func<OdataMaxPageSize, Task<TResult>> odataMaxPageSizeAccepter,
            Func<Custom, Task<TResult>> customAccepter,
            Func<GetBody, Task<TResult>> getBodyAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    odataMaxVersionAccepter,
                    odataMaxPageSizeAccepter,
                    customAccepter,
                    getBodyAccepter));
        }
    }

    public interface IOdataMaxVersionHeaderReader
    {
        OdataVersion OdataVersion { get; }

        Task<IGetHeaderReader> Next();
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

        Task<IGetHeaderReader> Next();
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

        Task<CustomHeaderToken<T>> Next();
    }

    public sealed class HeaderFieldName
    {
        internal HeaderFieldName(string name)
        {
            Name = name;
        }

        internal string Name { get; }
    }

    public abstract class CustomHeaderToken<T>
    {
        private CustomHeaderToken()
        {
        }

        public sealed class FieldValue : CustomHeaderToken<T>
        {
            public FieldValue(IHeaderFieldValueReader<T> headerFieldValueReader)
            {
                HeaderFieldValueReader = headerFieldValueReader;
            }

            public IHeaderFieldValueReader<T> HeaderFieldValueReader { get; }
        }

        public sealed class Header : CustomHeaderToken<T>
        {
            public Header(T getHeaderReader)
            {
                GetHeaderReader = getHeaderReader;
            }

            public T GetHeaderReader { get; }
        }

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(FieldValue node);

            Task<TResult> Accept(Header node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is FieldValue contentType)
            {
                return await accepter.Accept(contentType).ConfigureAwait(false);
            }
            else if (this is Header custom)
            {
                return await accepter.Accept(custom).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<FieldValue, Task<TResult>> fieldValueAccepter;
            private readonly Func<Header, Task<TResult>> headerAccepter;

            public DelegateAccepter(
                Func<FieldValue, Task<TResult>> fieldValueAccepter,
                Func<Header, Task<TResult>> headerAccepter)
            {
                this.fieldValueAccepter = fieldValueAccepter;
                this.headerAccepter = headerAccepter;
            }

            public Task<TResult> Accept(FieldValue node)
            {
                return this.fieldValueAccepter(node);
            }

            public Task<TResult> Accept(Header node)
            {
                return this.headerAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<FieldValue, Task<TResult>> fieldValueAccepter,
            Func<Header, Task<TResult>> headerAccepter)
        {
            return this.Dispatch(new DelegateAccepter<TResult>(fieldValueAccepter, headerAccepter));
        }
    }

    public interface IHeaderFieldValueReader<T>
    {
        HeaderFieldValue HeaderFieldValue { get; }

        Task<T> Next();
    }

    public sealed class HeaderFieldValue
    {
        internal HeaderFieldValue(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }

























    public interface IGetBodyReader
    {
        Task<GetBody> GetBody { get; }
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
