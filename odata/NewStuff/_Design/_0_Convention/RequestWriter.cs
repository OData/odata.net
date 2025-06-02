namespace NewStuff._Design._0_Convention
{
    public interface IRequestWriter
    {
        IGetRequestWriter CommitGet();
        IPatchRequestWriter CommitPatch();
        IPostRequestWriter CommitPost();
    }

    public interface IGetRequestWriter
    {
        IUriWriter<IGetHeaderWriter> Commit();
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

    public interface IPropertyWriter<out T>
    {
        IPropertyNameWriter<T> Commit();
    }

    public interface IPropertyNameWriter<out T>
    {
        IPropertyValueWriter<T> Commit(PropertyName propertyName);
    }

    public interface IPropertyValueWriter<out T>
    {
        IPrimitivePropertyValueWriter<T> CommitPrimitive();

        IComplexPropertyValueWriter<T> CommitComplex();

        INullPropertyValueWriter<T> CommitNull();

        IMultiValuedPropertyValueWriter<T> CommitMultiValued();
    }

    public interface IPrimitivePropertyValueWriter<out T>
    {
        T Commit(PrimitivePropertyValue primitivePropertyValue);
    }

    public interface IComplexPropertyValueWriter<out T>
    {
        IPropertyWriter<IComplexPropertyValueWriter<T>> CommitProperty();

        T Commit();
    }

    public interface INullPropertyValueWriter<out T>
    {
        T Commit();
    }

    public interface IMultiValuedPropertyValueWriter<out T>
    {
        IComplexPropertyValueWriter<IMultiValuedPropertyValueWriter<T>> CommitValue();

        T Commit();
    }

    public interface IPostRequestWriter
    {
        //// TODO
    }




















    public interface IUriWriter<out T>
    {
        IUriSchemeWriter<T> Commit();
    }

    public interface IUriSchemeWriter<out T>
    {
        IUriDomainWriter<T> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter<out T>
    {
        IUriPortWriter<T> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter<out T>
    {
        IUriPathSegmentWriter<T> Commit();

        IUriPathSegmentWriter<T> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter<out T>
    {
        IQueryOptionWriter<T> Commit();
        IUriPathSegmentWriter<T> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter<out T>
    {
        T Commit();
        IFragmentWriter<T> CommitFragment();
        IQueryParameterWriter<T> CommitParameter();
    }

    public interface IQueryParameterWriter<out T>
    {
        IQueryValueWriter<T> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter<out T>
    {
        IQueryOptionWriter<T> Commit();
        IQueryOptionWriter<T> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter<out T>
    {
        T Commit(Fragment fragment);
    }






















    public interface IGetHeaderWriter
    {
        IGetBodyWriter Commit();
        ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader();
        IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize();
        IOdataMaxVersionHeaderWriter CommitOdataMaxVersion();
    }

    public interface IOdataMaxVersionHeaderWriter
    {
        IGetHeaderWriter Commit(OdataVersion odataVersion);
    }

    public interface IOdataMaxPageSizeHeaderWriter
    {
        IGetHeaderWriter Commit(OdataMaxPageSize odataMaxPageSize);
    }

    public interface ICustomHeaderWriter<out T>
    {
        IHeaderFieldValueWriter<T> Commit(HeaderFieldName headerFieldName);
    }

    public interface IHeaderFieldValueWriter<out T>
    {
        T Commit();
        T Commit(HeaderFieldValue headerFieldValue);
    }
























    public interface IGetBodyWriter
    {
        IGetResponseReader Commit();
    }
}
