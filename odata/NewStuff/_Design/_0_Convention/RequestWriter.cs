namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Transactions;

    public interface IRequestWriter
    {
        IGetRequestWriter CommitGet();
        IPatchRequestWriter CommitPatch();
        IPatchRequestWriter CommitPost();//// TODO TODO TODO IMPORTANT you've re-used IPatchRequestWriter for convenience here; it should really be ipostrequestwriter
    }

    public interface IGetRequestWriter
    {
        IUriWriter<IGetHeaderWriter> Commit();
    }

    public sealed class GetRequestWriter : IGetRequestWriter
    {
        private GetRequestWriter()
        {
        }

        public IUriWriter<IGetHeaderWriter> Commit()
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IPatchRequestWriter
    {
        IUriWriter<IPatchHeaderWriter> Commit();
    }

    public interface IPatchHeaderWriter
    {
        IPatchRequestBodyWriter Commit();
        ICustomHeaderWriter<IPatchHeaderWriter> CommitCustomHeader();
        IEtagWriter CommitEtag();
    }

    public interface IEtagWriter
    {
        IPatchHeaderWriter Commit(Etag etag);
    }

    public sealed class Etag
    {
        private Etag()
        {
        }
    }

    public interface IPatchRequestBodyWriter
    {
        IPropertyWriter<IPatchRequestBodyWriter> CommitProperty();

        IGetResponseReader Commit(); //// TODO TODO TODO IMPORTANT you've re-used getresponsereader for convenience here; it should really be igetpatchresponsereader
    }

    public interface IPropertyWriter<T>
    {
        IPropertyNameWriter<T> Commit();
    }

    public interface IPropertyNameWriter<T>
    {
        IPropertyValueWriter<T> Commit(PropertyName propertyName);
    }

    public interface IPropertyValueWriter<T>
    {
        IPrimitivePropertyValueWriter<T> CommitPrimitive();

        IComplexPropertyValueWriter<T> CommitComplex();

        INullPropertyValueWriter<T> CommitNull();

        IMultiValuedPropertyValueWriter<T> CommitMultiValued();
    }

    public interface IPrimitivePropertyValueWriter<T>
    {
        T Commit(PrimitivePropertyValue primitivePropertyValue);
    }

    public interface IComplexPropertyValueWriter<T>
    {
        IPropertyWriter<IComplexPropertyValueWriter<T>> CommitProperty();

        T Commit();
    }

    public interface INullPropertyValueWriter<T>
    {
        T Commit();
    }

    public interface IMultiValuedPropertyValueWriter<T>
    {
        IComplexPropertyValueWriter<IMultiValuedPropertyValueWriter<T>> CommitValue();

        T Commit();
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
        T Commit();
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

        public T Commit()
        {
            throw new NotImplementedException();
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
        ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader();
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

        public ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader()
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

    public interface ICustomHeaderWriter<T>
    {
        IHeaderFieldValueWriter<T> Commit(HeaderFieldName headerFieldName);
    }

    public sealed class CustomHeaderWriter<T> : ICustomHeaderWriter<T>
    {
        private CustomHeaderWriter()
        {
        }

        public IHeaderFieldValueWriter<T> Commit(HeaderFieldName headerFieldName)
        {
            throw new NotImplementedException("TODO");
        }
    }

    public interface IHeaderFieldValueWriter<T>
    {
        T Commit();
        T Commit(HeaderFieldValue headerFieldValue);
    }

    public sealed class HeaderFieldValueWriter<T> : IHeaderFieldValueWriter<T>
    {
        private HeaderFieldValueWriter()
        {
        }

        public T Commit(HeaderFieldValue headerFieldValue)
        {
            throw new NotImplementedException("TODO");
        }

        public T Commit()
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
