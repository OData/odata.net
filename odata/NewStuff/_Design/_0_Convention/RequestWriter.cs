namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Threading.Tasks;
    using CombinatorParsingV2;
    using NewStuff._Design._0_Convention.RefTask;

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




















    public interface IUriWriter<T> : IUriWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>, Nothing, Nothing, IUriPathSegmentWriter<T>, Nothing, Nothing, IUriPortWriter<T>, Nothing, Nothing, IUriDomainWriter<T>, Nothing, Nothing, IUriSchemeWriter<T>>
    {
    }

    public interface IUriSchemeWriter<T> : IUriSchemeWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>, Nothing, Nothing, IUriPathSegmentWriter<T>, Nothing, Nothing, IUriPortWriter<T>, Nothing, Nothing, IUriDomainWriter<T>>
    {
    }

    public interface IUriDomainWriter<T> : IUriDomainWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>, Nothing, Nothing, IUriPathSegmentWriter<T>, Nothing, Nothing, IUriPortWriter<T>>
    {
    }

    public interface IUriPortWriter<T> : IUriPortWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>, Nothing, Nothing, IUriPathSegmentWriter<T>>
    {
    }

    public interface IUriPathSegmentWriter<T> : IUriPathSegmentWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>, Nothing, Nothing, IUriPathSegmentWriter<T>>
    {
    }

    public interface IQueryOptionWriter<T> : IQueryOptionWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>>
    {
    }

    public interface IQueryParameterWriter<T> : IQueryParameterWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>>
    {
    }

    public interface IQueryValueWriter<T> : IQueryValueWriter<Nothing, Nothing, T, Nothing, Nothing, IFragmentWriter<T>, Nothing, Nothing, IQueryParameterWriter<T>, Nothing, Nothing, IQueryValueWriter<T>, Nothing, Nothing, IQueryOptionWriter<T>>
    {
    }

    public interface IFragmentWriter<T> : IFragmentWriter<Nothing, Nothing, T>
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
