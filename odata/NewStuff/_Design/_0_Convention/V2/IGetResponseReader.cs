namespace NewStuff._Design._0_Convention.Readers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using NewStuff._Design._0_Convention.V2;

    public interface IReader<TNext> where TNext : allows ref struct //// TODO can you get covariance on this?
    {
        ValueTask Read();

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see langword="null"/> if <see cref="IReader{T}.Read"/> needs to be called first</returns>
        RefNullable<TNext> Next();
    }

    public interface IReader<TNext, out TValue> : IReader<TNext> //// TODO can you get covariance on this?
        where TNext : allows ref struct
        where TValue : allows ref struct
    {
        TValue Value { get; }
    }

    public readonly ref struct RefNullable<T> where T : allows ref struct
    {
        private readonly T value;

        public RefNullable(T value)
        {
            this.value = value;
            this.HasValue = true;
        }

        public bool HasValue { get; }

        public bool TryGetValue([MaybeNullWhen(false)] out T value)
        {
            if (this.HasValue)
            {
                value = this.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }

    public readonly struct Nothing
    {
    }














    //// TODO you are here
    //// TODO you've updated 2 interfaces to allow ref struct, but i think you need to do this bottom up actually

    public interface IGetResponseReader : IGetResponseReader<IGetResponseHeaderReader>
    {
    }

    public interface IGetResponseReader<TGetResponseHeaderReader> : IReader<TGetResponseHeaderReader>, IAsyncDisposable
        where TGetResponseHeaderReader : IGetResponseHeaderReader, allows ref struct
    {
    }

    public interface IGetResponseHeaderReader : IGetResponseHeaderReader<IContentTypeHeaderReader, ICustomHeaderReader, IGetResponseBodyReader>
    {
    }

    public interface IGetResponseHeaderReader<TContentTypeHeaderReader, TCustomHeaderReader, TGetResponseBodyReader>
        : IReader<GetResponseHeaderToken<TContentTypeHeaderReader, TCustomHeaderReader, TGetResponseBodyReader>>
        where TContentTypeHeaderReader : IContentTypeHeaderReader, allows ref struct
        where TCustomHeaderReader : ICustomHeaderReader, allows ref struct
        where TGetResponseBodyReader : IGetResponseBodyReader, allows ref struct
    {
    }

    public readonly ref struct GetResponseHeaderToken<TContentTypeHeaderReader, TCustomHeaderReader, TGetResponseBodyReader>
        where TContentTypeHeaderReader : IContentTypeHeaderReader, allows ref struct
        where TCustomHeaderReader : ICustomHeaderReader, allows ref struct
        where TGetResponseBodyReader : IGetResponseBodyReader, allows ref struct
    {
        private enum Type
        {
            ContentType,
            Custom,
            Body,
        }

        private readonly object value;

        private readonly Type type;

        public GetResponseHeaderToken(TContentTypeHeaderReader contentTypeHeaderReader)
        {
            this.type = Type.ContentType;
        }

        public GetResponseHeaderToken(TCustomHeaderReader customHeaderReader)
        {
            this.type = Type.Custom;
        }

        public GetResponseHeaderToken(TGetResponseBodyReader getResponseBodyReader)
        {
            this.type = Type.Body;
        }

        public interface IAccepter<TResult>
        {
            TResult Accept(TContentTypeHeaderReader contentTypeHeaderReader);

            TResult Accept(TCustomHeaderReader customHeaderReader);

            TResult Accept(TGetResponseBodyReader getResponseBodyReader);
        }

        public TResult Dispatch<TResult>(IAccepter<TResult> accepter)
        {
            switch (this.type)
            {
                default:
                    throw new Exception("TODO a visitor would prevent this");
            }
        }
    }

    public interface IContentTypeHeaderReader : IReader<IGetResponseHeaderReader, ContentType>
    {
    }

    public interface ICustomHeaderReader : IReader<CustomHeaderToken, HeaderFieldName>
    {
    }

    public readonly ref struct CustomHeaderToken
    {
        private enum Type
        {
            FieldValue,
            Header,
        }

        private readonly object value;

        private readonly Type type;

        public CustomHeaderToken(IHeaderFieldValueReader headerFieldValueReader)
        {
            this.value = headerFieldValueReader;

            this.type = Type.FieldValue;
        }

        public CustomHeaderToken(IGetResponseHeaderReader getResponseHeaderReader)
        {
            this.value = getResponseHeaderReader;

            this.type = Type.Header;
        }

        public interface IAccepter<TResult>
        {
            TResult Accept(IHeaderFieldValueReader headerFieldValueReader);

            TResult Accept(IGetResponseHeaderReader getResponseHeaderReader);
        }

        public TResult Dispatch<TResult>(IAccepter<TResult> accepter)
        {
            switch (this.type)
            {
                case Type.FieldValue:
                    return accepter.Accept((IHeaderFieldValueReader)this.value);
                case Type.Header:
                    return accepter.Accept((IGetResponseHeaderReader)this.value);
                default:
                    throw new Exception("TODO a visitor would prevent this");
            }
        }
    }

    public interface IHeaderFieldValueReader : IReader<IGetResponseHeaderReader, HeaderFieldValue>
    {
    }

    public interface IGetResponseBodyReader : IReader<IRootObjectReader>
    {
    }

    public interface IRootObjectReader : IReader<RootObjectToken>
    {
    }

    public readonly ref struct RootObjectToken
    {
        public RootObjectToken(IRootObjectOdataNextLinkReader nextLinkReader)
        {
        }

        public RootObjectToken(IRootObjectOdataContextReader odataContextReader)
        {
        }

        public RootObjectToken(IRootObjectOdataIdReader odataIdReader)
        {
        }

        public RootObjectToken(IRootObjectPropertyReader propertyReader)
        {
        }

        public RootObjectToken()
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootObjectOdataNextLinkReader : IReader<IRootObjectReader, OdataNextLink> //// TODO note that you are sharing the concrete types between root object and non-root object; you are assuming that just there's just a different syntax to "get at" the "odatanextlink", but ultimately the nextlink always has a consistent syntax

    {
    }

    public interface IRootObjectOdataContextReader : IReader<IRootObjectReader, OdataContext>
    {
    }

    public interface IRootObjectOdataIdReader : IReader<IRootObjectReader, OdataId>
    {
    }

    public interface IRootObjectPropertyReader : IReader<IRootObjectPropertyNameReader>
    {
    }

    public interface IRootObjectPropertyNameReader : IReader<PropertyName, IRootObjectPropertyValueReader>
    {
    }

    public interface IRootObjectPropertyValueReader : IReader<RootObjectPropertyValueToken>
    {
    }

    public readonly ref struct RootObjectPropertyValueToken
    {
        public RootObjectPropertyValueToken(IRootObjectPrimitivePropertyValueReader rootObjectPrimitivePropertyValueReader)
        {
        }

        public RootObjectPropertyValueToken(IRootObjectNullPropertyValueReader rootObjectNullPropertyValueReader)
        {
        }

        public RootObjectPropertyValueToken(IRootObjectMultiValuedPropertyValueReader rootObjectMultiValuedPropertyValueReader)
        {
        }

        public RootObjectPropertyValueToken(IRootObjectComplexPropertyValueReader rootObjectComplexPropertyValueReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootObjectPrimitivePropertyValueReader : IReader<IRootObjectReader, PrimitivePropertyValue>
    {
    }

    public interface IRootObjectNullPropertyValueReader : IReader<IRootObjectReader>
    {
    }

    public interface IRootObjectMultiValuedPropertyValueReader : IReader<RootObjectPropertyValueToken>
    {
    }

    public readonly ref struct RootObjectMultiValuedPropertyValueToken
    {
        public RootObjectMultiValuedPropertyValueToken(IRootObjectPrimitiveElementReader rootObjectPrimitiveElementReader)
        {
        }

        public RootObjectMultiValuedPropertyValueToken(IRootObjectComplexElementReader rootObjectComplexElementReader)
        {
        }

        public RootObjectMultiValuedPropertyValueToken(IRootObjectReader rootObjectReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootObjectPrimitiveElementReader : IReader<IRootObjectMultiValuedPropertyValueReader, PrimitiveElement>
    {
    }

    public interface IRootObjectComplexElementReader : IReader<INonRootObjectReader<IRootObjectMultiValuedPropertyValueReader>>
    {
    }

    public interface IRootObjectComplexPropertyValueReader : IReader<INonRootObjectReader<IRootObjectReader>>
    {
    }

    public interface INonRootObjectReader<TObjectReader> : IReader<NonRootObjectToken<TObjectReader>>
        where TObjectReader : allows ref struct //// TODO
    {
    }

    public readonly ref struct NonRootObjectToken<TObjectReader>
        where TObjectReader : allows ref struct
    {
        public NonRootObjectToken(INonRootObjectOdataContextReader<TObjectReader> nonRootObjectOdataContextReader)
        {
        }

        public NonRootObjectToken(INonRootObjectOdataIdReader<TObjectReader> nonRootObjectOdataIdReader)
        {
        }

        public NonRootObjectToken(INonRootObjectPropertyReader<TObjectReader> nonRootObjectPropertyReader)
        {
        }

        public NonRootObjectToken(TObjectReader next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectOdataContextReader<TObjectReader> : IReader<INonRootObjectReader<TObjectReader>, OdataContext>
    {
    }

    public interface INonRootObjectOdataIdReader<TObjectReader> : IReader<INonRootObjectReader<TObjectReader>, OdataId>
    {
    }

    public interface INonRootObjectPropertyReader<TObjectReader>
    {
    }

    public interface INonRootObjectPropertyNameReader<TObjectReader> : IReader<PropertyName, INonRootObjectPropertyValueReader<TObjectReader>>
    {
    }

    public interface INonRootObjectPropertyValueReader<TObjectReader> : IReader<NonRootObjectPropertyValueToken<TObjectReader>>
    {
    }

    public readonly ref struct NonRootObjectPropertyValueToken<TObjectReader>
    {
        public NonRootObjectPropertyValueToken(INonRootObjectPrimitivePropertyValueReader<TObjectReader> nonRootObjectPrimitivePropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectNullPropertyValueReader<TObjectReader> nonRootObjectNullPropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectMultiValuedPropertyValueReader<TObjectReader> nonRootObjectMultiValuedPropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectComplexPropertyValueReader<TObjectReader> nonRootObjectComplexPropertyValueReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectPrimitivePropertyValueReader<TObjectReader> : IReader<TObjectReader, PrimitivePropertyValue>
    {
    }

    public interface INonRootObjectNullPropertyValueReader<TObjectReader> : IReader<TObjectReader>
    {
    }

    public interface INonRootObjectMultiValuedPropertyValueReader<TObjectReader> : IReader<NonRootObjectMultiValuedPropertyValueToken<TObjectReader>>
        where TObjectReader : allows ref struct //// TODO
    {
    }

    public readonly ref struct NonRootObjectMultiValuedPropertyValueToken<TObjectReader>
    {
        public NonRootObjectMultiValuedPropertyValueToken(INonRootObjectPrimitiveElementReader<TObjectReader> nonRootObjectPrimitiveElementReader)
        {
        }

        public NonRootObjectMultiValuedPropertyValueToken(INonRootObjectComplexElementReader<TObjectReader> nonRootObjectComplexElementReader)
        {
        }

        public NonRootObjectMultiValuedPropertyValueToken(TObjectReader next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectPrimitiveElementReader<TObjectReader> 
        : INonRootObjectPrimitiveElementReader
            <
                INonRootObjectMultiValuedPropertyValueReader
                    <
                        TObjectReader
                    >, 
                TObjectReader
            >
    {
    }

    public interface INonRootObjectPrimitiveElementReader<TNonRootObjectMultiValuedPropertyValueReader, TObjectReader> 
        : IReader<TNonRootObjectMultiValuedPropertyValueReader, PrimitiveElement>
        where TNonRootObjectMultiValuedPropertyValueReader : INonRootObjectMultiValuedPropertyValueReader<TObjectReader>, allows ref struct
        where TObjectReader : allows ref struct
    {
    }

    public interface INonRootObjectComplexElementReader<TObjectReader> 
        : INonRootObjectComplexElementReader
            <
                INonRootObjectReader
                    <
                        INonRootObjectMultiValuedPropertyValueReader
                            <
                                TObjectReader
                            >
                    >, 
                INonRootObjectMultiValuedPropertyValueReader
                    <
                        TObjectReader
                    >, 
                TObjectReader
            >
    {
    }

    public interface INonRootObjectComplexElementReader<TNonRootObjectReader, TNonRootObjectMultiValuedPropertyValueReader, TObjectReader> 
        : IReader<INonRootObjectReader<INonRootObjectMultiValuedPropertyValueReader<TObjectReader>>>
        where TNonRootObjectReader : INonRootObjectReader<TNonRootObjectMultiValuedPropertyValueReader>, allows ref struct
        where TNonRootObjectMultiValuedPropertyValueReader : INonRootObjectMultiValuedPropertyValueReader<TObjectReader>, allows ref struct
        where TObjectReader : allows ref struct
    {
    }

    public interface INonRootObjectComplexPropertyValueReader<TObjectReader>
        : INonRootObjectComplexPropertyValueReader
            <
                INonRootObjectReader
                    <
                        TObjectReader
                    >, 
                TObjectReader
            >
    {
    }

    public interface INonRootObjectComplexPropertyValueReader<TNonRootObjectReader, TObjectReader> 
        : IReader<TNonRootObjectReader>
        where TNonRootObjectReader : INonRootObjectReader<TObjectReader>, allows ref struct
        where TObjectReader : allows ref struct
    {
    }






















    public interface IComplexObjectReader<T> : IReader<ComplexObjectToken<T>>
    {
    }

    public readonly ref struct ComplexObjectToken<T>
    {
        public ComplexObjectToken(IOdataContextReader<T> dataContextReader)
        {
        }

        public ComplexObjectToken(IOdataIdReader<T> odataIdReader)
        {
        }

        public ComplexObjectToken(IPropertyReader<T> propertyReader)
        {
        }

        public ComplexObjectToken(T next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INextLinkReader : IReader<IGetResponseBodyReader, OdataNextLink>
    {
    }

    public interface IOdataContextReader<T> : IReader<T, OdataContext>
    {
    }

    public interface IOdataIdReader<T> : IReader<T, OdataId>
    {
    }

    public interface IPropertyReader<T> : IReader<IPropertyNameReader<T>>
    {
    }

    public interface IPropertyNameReader<T> : IReader<IPropertyValueReader<T>, PropertyName>
    {
    }

    public interface IPropertyValueReader<T> : IReader<PropertyValueToken<T>>
    {
    }

    public readonly ref struct PropertyValueToken<T>
    {
        public PropertyValueToken(IPrimitivePropertyValueReader<T> primitivePropertyValueReader)
        {
        }

        public PropertyValueToken(IComplexObjectReader<T> complexObjectReader)
        {
        }

        public PropertyValueToken(IMultiValuedPropertyValueReader<T> multiValuedPropertyValueReader)
        {
        }

        public PropertyValueToken(INullPropertyValueReader<T> nullPropertyValueReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IPrimitivePropertyValueReader<T> : IReader<T, PrimitivePropertyValue>
    {
    }

    public interface IMultiValuedPropertyValueReader<T> : IReader<MultiValuedPropertyValueToken<T>>
    {
    }

    public readonly ref struct MultiValuedPropertyValueToken<T>
    {
        public MultiValuedPropertyValueToken(IComplexObjectReader<T> complexObjectReader)
        {
        }

        public MultiValuedPropertyValueToken(IPrimitivePropertyValueReader<T> primitivePropertyValueReader)
        {
        }

        public MultiValuedPropertyValueToken(T next)
        {
        }
    }

    public interface INullPropertyValueReader<T> : IReader<T>
    {
    }

    //// TODO i think you want to go back through the above just to be thorough and *not* reuse types; so, for example, don't use a complexobjectreader as part of the getresponsebodyreader, and don't use a complexobjectreader for complex multivalued elements, and don't use a primitivepropertyvaluereader for primitive multivalued elements
}
