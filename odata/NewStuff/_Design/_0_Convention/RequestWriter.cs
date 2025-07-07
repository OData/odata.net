namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Threading.Tasks;
    using NewStuff._Design._0_Convention.RefTask2;
    using NewStuff._Design._1_Protocol.Sample;

    public interface IRequestWriter
    {
        Task<IGetRequestWriter> CommitGet();
        Task<IPatchRequestWriter> CommitPatch();
        Task<IPostRequestWriter> CommitPost();
    }

    public interface IGetRequestWriter : IAsyncDisposable
    {
        Task<IUriWriter<IGetHeaderWriter>> Commit();
    }

    public interface IPatchRequestWriter : IAsyncDisposable
    {
        Task<IUriWriter<IPatchHeaderWriter>> Commit();
    }

    public interface IPatchHeaderWriter
    {
        Task<IPatchRequestBodyWriter> Commit();
        Task<ICustomHeaderWriter<IPatchHeaderWriter>> CommitCustomHeader();
        Task<IEtagWriter> CommitEtag();
    }

    public interface IEtagWriter
    {
        Task<IPatchHeaderWriter> Commit(Etag etag);
    }

    public sealed class Etag
    {
        private Etag()
        {
        }
    }

    public interface IPatchRequestBodyWriter
    {
        Task<IPropertyWriter<IPatchRequestBodyWriter>> CommitProperty();

        Task<IGetResponseReader> Commit(); //// TODO TODO TODO IMPORTANT you've re-used getresponsereader for convenience here; it should really be igetpatchresponsereader
    }

    public interface IPropertyWriter<T>
    {
        Task<IPropertyNameWriter<T>> Commit();
    }

    public interface IPropertyNameWriter<T>
    {
        Task<IPropertyValueWriter<T>> Commit(PropertyName propertyName);
    }

    public interface IPropertyValueWriter<T>
    {
        Task<IPrimitivePropertyValueWriter<T>> CommitPrimitive();

        Task<IComplexPropertyValueWriter<T>> CommitComplex();

        Task<INullPropertyValueWriter<T>> CommitNull();

        Task<IMultiValuedPropertyValueWriter<T>> CommitMultiValued();
    }

    public interface IPrimitivePropertyValueWriter<T>
    {
        Task<T> Commit(PrimitivePropertyValue primitivePropertyValue);
    }

    public interface IComplexPropertyValueWriter<T>
    {
        Task<IPropertyWriter<IComplexPropertyValueWriter<T>>> CommitProperty();

        Task<T> Commit();
    }

    public interface INullPropertyValueWriter<T>
    {
        Task<T> Commit();
    }

    public interface IMultiValuedPropertyValueWriter<T>
    {
        Task<IComplexPropertyValueWriter<IMultiValuedPropertyValueWriter<T>>> CommitValue();

        Task<T> Commit();
    }

    public interface IPostRequestWriter : IAsyncDisposable
    {
        //// TODO
    }




















    public interface IUriWriter<T> : IUriWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>, Nothing2, Nothing2, IUriPathSegmentWriter<T>, Nothing2, Nothing2, IUriPortWriter<T>, Nothing2, Nothing2, IUriDomainWriter<T>, Nothing2, Nothing2, IUriSchemeWriter<T>>
    {
    }

    public interface IUriSchemeWriter<T> : IUriSchemeWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>, Nothing2, Nothing2, IUriPathSegmentWriter<T>, Nothing2, Nothing2, IUriPortWriter<T>, Nothing2, Nothing2, IUriDomainWriter<T>>
    {
    }

    public interface IUriDomainWriter<T> : IUriDomainWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>, Nothing2, Nothing2, IUriPathSegmentWriter<T>, Nothing2, Nothing2, IUriPortWriter<T>>
    {
    }

    public interface IUriPortWriter<T> : IUriPortWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>, Nothing2, Nothing2, IUriPathSegmentWriter<T>>
    {
    }

    public interface IUriPathSegmentWriter<T> : IUriPathSegmentWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>, Nothing2, Nothing2, IUriPathSegmentWriter<T>>
    {
    }

    public interface IQueryOptionWriter<T> : IQueryOptionWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>>
    {
    }

    public interface IQueryParameterWriter<T> : IQueryParameterWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>>
    {
    }

    public interface IQueryValueWriter<T> : IQueryValueWriter<Nothing2, Nothing2, T, Nothing2, Nothing2, IFragmentWriter<T>, Nothing2, Nothing2, IQueryParameterWriter<T>, Nothing2, Nothing2, IQueryValueWriter<T>, Nothing2, Nothing2, IQueryOptionWriter<T>>
    {
    }

    public interface IFragmentWriter<T> : IFragmentWriter<Nothing2, Nothing2, T>
    {
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
