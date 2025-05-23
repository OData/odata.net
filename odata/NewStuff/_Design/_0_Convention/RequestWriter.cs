namespace NewStuff._Design._0_Convention
{
    using System;

    public interface IRequestWriter
    {
        IGetRequestWriter CommitGet();
        IPatchRequestWriter CommitPatch();
        IPostRequestWriter CommitPost();
    }

    public sealed class RequestWriter : IRequestWriter
    {
        private RequestWriter()
        {
        }

        public IGetRequestWriter CommitGet()
        {
            throw new NotImplementedException("TODO");
        }

        public IPatchRequestWriter CommitPatch()
        {
            throw new NotImplementedException("TODO");
        }

        public IPostRequestWriter CommitPost()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IGetRequestWriter
    {
        IUriWriter<GetHeaderWriter> Commit();
    }

    public sealed class GetRequestWriter : IGetRequestWriter
    {
        private GetRequestWriter()
        {
        }

        public IUriWriter<GetHeaderWriter> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IPatchRequestWriter
    {
        //// TODO
    }

    public interface IPostRequestWriter
    {
        //// TODO
    }

    public interface IUriWriter<T>
    {
        IUriSchemeWriter<T> Commit();
    }

    public sealed class UriWriter<T> : IUriWriter<T>
    {
        private UriWriter()
        {
        }

        public IUriSchemeWriter<T> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IUriSchemeWriter<T>
    {
        IUriDomainWriter<T> Commit(UriScheme uriScheme);
    }

    public sealed class UriSchemeWriter<T> : IUriSchemeWriter<T>
    {
        private UriSchemeWriter()
        {
        }

        public IUriDomainWriter<T> Commit(UriScheme uriScheme) //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IUriDomainWriter<T>
    {
        IUriPortWriter<T> Commit(UriDomain uriDomain);
    }

    public sealed class UriDomainWriter<T> : IUriDomainWriter<T>
    {
        private UriDomainWriter()
        {
        }

        public IUriPortWriter<T> Commit(UriDomain uriDomain)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IUriPortWriter<T>
    {
        IUriPathSegmentWriter<T> Commit();

        IUriPathSegmentWriter<T> Commit(UriPort uriPort);
    }

    public sealed class UriPortWriter<T> : IUriPortWriter<T>
    {
        private UriPortWriter()
        {
        }

        public IUriPathSegmentWriter<T> Commit(UriPort uriPort)
        {
            throw new NotImplementedException("TODO");
        }

        public IUriPathSegmentWriter<T> Commit()
        {
            throw new NotImplementedException();
        }
    }

    public interface IUriPathSegmentWriter<T>
    {
        IQueryOptionWriter<T> Commit();
        IUriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment);
    }

    public sealed class UriPathSegmentWriter<T> : IUriPathSegmentWriter<T>
    {
        private UriPathSegmentWriter()
        {
        }

        public IUriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment)
        {
            throw new NotImplementedException("TODO");
        }

        public IQueryOptionWriter<T> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IQueryOptionWriter<T>
    {
        IFragmentWriter<T> CommitFragment();
        IQueryParameterWriter<T> CommitParameter();
    }

    public sealed class QueryOptionWriter<T> : IQueryOptionWriter<T>
    {
        private QueryOptionWriter()
        {
        }

        public IQueryParameterWriter<T> CommitParameter()
        {
            throw new NotImplementedException("TODO");
        }

        public IFragmentWriter<T> CommitFragment()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IQueryParameterWriter<T>
    {
        IQueryValueWriter<T> Commit(QueryParameter queryParameter);
    }

    public sealed class QueryParameterWriter<T> : IQueryParameterWriter<T>
    {
        private QueryParameterWriter()
        {
        }

        public IQueryValueWriter<T> Commit(QueryParameter queryParameter)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IQueryValueWriter<T>
    {
        IQueryOptionWriter<T> Commit();
        IQueryOptionWriter<T> Commit(QueryValue queryValue);
    }

    public sealed class QueryValueWriter<T> : IQueryValueWriter<T>
    {
        private QueryValueWriter()
        {
        }

        public IQueryOptionWriter<T> Commit()
        {
            throw new NotImplementedException("TODO");
        }

        public IQueryOptionWriter<T> Commit(QueryValue queryValue)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IFragmentWriter<T>
    {
        T Commit(Fragment fragment);
    }

    public sealed class FragmentWriter<T> : IFragmentWriter<T>
    {
        private FragmentWriter()
        {
        }

        public T Commit(Fragment fragment)
        {
            throw new NotImplementedException("TODO");
        }
    }

























    public interface IGetHeaderWriter
    {
        IGetBodyWriter Commit();
        ICustomHeaderWriter CommitCustomHeader();
        IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize();
        IOdataMaxVersionHeaderWriter CommitOdataMaxVersion();
    }

    public sealed class GetHeaderWriter : IGetHeaderWriter
    {
        private GetHeaderWriter()
        {
        }

        public IOdataMaxVersionHeaderWriter CommitOdataMaxVersion()
        {
            throw new NotImplementedException("TODO");
        }

        public IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize()
        {
            throw new NotImplementedException("TODO");
        }

        public ICustomHeaderWriter CommitCustomHeader()
        {
            throw new NotImplementedException("TODO");
        }

        public IGetBodyWriter Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IOdataMaxVersionHeaderWriter
    {
        IGetHeaderWriter Commit(OdataVersion odataVersion);
    }

    public sealed class OdataMaxVersionHeaderWriter : IOdataMaxVersionHeaderWriter
    {
        private OdataMaxVersionHeaderWriter()
        {
        }

        public IGetHeaderWriter Commit(OdataVersion odataVersion)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IOdataMaxPageSizeHeaderWriter
    {
        IGetHeaderWriter Commit(OdataMaxPageSize odataMaxPageSize);
    }

    public sealed class OdataMaxPageSizeHeaderWriter : IOdataMaxPageSizeHeaderWriter
    {
        private OdataMaxPageSizeHeaderWriter()
        {
        }

        public IGetHeaderWriter Commit(OdataMaxPageSize odataMaxPageSize)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface ICustomHeaderWriter
    {
        IHeaderFieldValueWriter Commit(HeaderFieldName headerFieldName);
    }

    public sealed class CustomHeaderWriter : ICustomHeaderWriter
    {
        private CustomHeaderWriter()
        {
        }

        public IHeaderFieldValueWriter Commit(HeaderFieldName headerFieldName)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IHeaderFieldValueWriter
    {
        IGetHeaderWriter Commit();
        IGetHeaderWriter Commit(HeaderFieldValue headerFieldValue);
    }

    public sealed class HeaderFieldValueWriter : IHeaderFieldValueWriter
    {
        private HeaderFieldValueWriter()
        {
        }

        public IGetHeaderWriter Commit(HeaderFieldValue headerFieldValue)
        {
            throw new NotImplementedException("TODO");
        }

        public IGetHeaderWriter Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

























    public interface IGetBodyWriter
    {
        IGetResponseReader Commit();
    }

    public sealed class GetBodyWriter : IGetBodyWriter
    {
        private GetBodyWriter()
        {
        }

        public IGetResponseReader Commit() //// TODO this shouldn't return a string, it should be parameterized
        {
            throw new NotImplementedException("TODO");
        }
    }
}
