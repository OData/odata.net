namespace NewStuff._Design._0_Convention
{
    using System.Threading.Tasks;

    public interface IRequestWriter
    {
        Task<IGetRequestWriter> CommitGet();
        Task<IPatchRequestWriter> CommitPatch();
        Task<IPostRequestWriter> CommitPost();
    }

    public interface IGetRequestWriter
    {
        Task<IUriWriter<IGetHeaderWriter>> Commit();
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




















    public interface IUriWriter<T>
    {
        Task<IUriSchemeWriter<T>> Commit();
    }

    public interface IUriSchemeWriter<T>
    {
        Task<IUriDomainWriter<T>> Commit(UriScheme uriScheme); //// TODO i can't tell if this type should be committed in its writer, or in the "previous" writer (i.e. should this commit take a domain or a scheme?)
    }

    public interface IUriDomainWriter<T>
    {
        Task<IUriPortWriter<T>> Commit(UriDomain uriDomain);
    }

    public interface IUriPortWriter<T>
    {
        Task<IUriPathSegmentWriter<T>> Commit();

        Task<IUriPathSegmentWriter<T>> Commit(UriPort uriPort);
    }

    public interface IUriPathSegmentWriter<T>
    {
        Task<IQueryOptionWriter<T>> Commit();
        Task<IUriPathSegmentWriter<T>> Commit(UriPathSegment uriPathSegment);
    }

    public interface IQueryOptionWriter<T>
    {
        Task<T> Commit();
        Task<IFragmentWriter<T>> CommitFragment();
        Task<IQueryParameterWriter<T>> CommitParameter();
    }

    public interface IQueryParameterWriter<T>
    {
        Task<IQueryValueWriter<T>> Commit(QueryParameter queryParameter);
    }

    public interface IQueryValueWriter<T>
    {
        Task<IQueryOptionWriter<T>> Commit();
        Task<IQueryOptionWriter<T>> Commit(QueryValue queryValue);
    }

    public interface IFragmentWriter<T>
    {
        Task<T> Commit(Fragment fragment);
    }






















    public interface IGetHeaderWriter
    {
        Task<IGetBodyWriter> Commit();
        Task<ICustomHeaderWriter<IGetHeaderWriter>> CommitCustomHeader();
        Task<IOdataMaxPageSizeHeaderWriter> CommitOdataMaxPageSize();
        Task<IOdataMaxVersionHeaderWriter> CommitOdataMaxVersion();
    }

    public interface IOdataMaxVersionHeaderWriter
    {
        Task<IGetHeaderWriter> Commit(OdataVersion odataVersion);
    }

    public interface IOdataMaxPageSizeHeaderWriter
    {
        Task<IGetHeaderWriter> Commit(OdataMaxPageSize odataMaxPageSize);
    }

    public interface ICustomHeaderWriter<T>
    {
        Task<IHeaderFieldValueWriter<T>> Commit(HeaderFieldName headerFieldName);
    }

    public interface IHeaderFieldValueWriter<T>
    {
        Task<T> Commit();
        Task<T> Commit(HeaderFieldValue headerFieldValue);
    }
























    public interface IGetBodyWriter
    {
        Task<IGetResponseReader> Commit();
    }
}
