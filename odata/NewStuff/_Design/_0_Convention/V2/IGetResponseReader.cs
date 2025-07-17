namespace NewStuff._Design._0_Convention.Readers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using NewStuff._Design._0_Convention.V2;

    public interface IReader<TNext> where TNext : allows ref struct
    {
        ValueTask Read();

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see langword="null"/> if <see cref="IReader{T}.Read"/> needs to be called first</returns>
        RefNullable<TNext> Next();
    }

    public interface IReader<TNext, TValue> : IReader<TNext>
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


    public interface IGetResponseReader : IReader<IGetResponseHeaderReader>, IAsyncDisposable
    {
    }

    public interface IGetResponseHeaderReader : IReader<GetResponseHeaderToken>
    {
    }

    public readonly ref struct GetResponseHeaderToken
    {
        private enum Type
        {
            ContentType,
            Custom,
            Body,
        }

        private readonly object value;

        private readonly Type type;

        public GetResponseHeaderToken(IContentTypeHeaderReader contentTypeHeaderReader)
        {
            this.value = contentTypeHeaderReader;

            this.type = Type.ContentType;
        }

        public GetResponseHeaderToken(ICustomHeaderReader customHeaderReader)
        {
            this.value = customHeaderReader;

            this.type = Type.Custom;
        }

        public GetResponseHeaderToken(IGetResponseBodyReader getResponseBodyReader)
        {
            this.value = getResponseBodyReader;

            this.type = Type.Body;
        }

        public interface IAccepter<TResult>
        {
            TResult Accept(IContentTypeHeaderReader contentTypeHeaderReader);

            TResult Accept(ICustomHeaderReader customHeaderReader);

            TResult Accept(IGetResponseBodyReader getResponseBodyReader);
        }

        public TResult Dispatch<TResult>(IAccepter<TResult> accepter)
        {
            switch (this.type)
            {
                case Type.ContentType:
                    return accepter.Accept((IContentTypeHeaderReader)this.value);
                case Type.Custom:
                    return accepter.Accept((ICustomHeaderReader)this.value);
                case Type.Body:
                    return accepter.Accept((IGetResponseBodyReader)this.value);
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

    public interface INonRootObjectReader<T> : IReader<NonRootObjectToken<T>>
    {
    }

    public readonly ref struct NonRootObjectToken<T>
    {
        public NonRootObjectToken(INonRootObjectOdataContextReader<T> nonRootObjectOdataContextReader)
        {
        }

        public NonRootObjectToken(INonRootObjectOdataIdReader<T> nonRootObjectOdataIdReader)
        {
        }

        public NonRootObjectToken(INonRootObjectPropertyReader<T> nonRootObjectPropertyReader)
        {
        }

        public NonRootObjectToken(T next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectOdataContextReader<T> : IReader<INonRootObjectReader<T>, OdataContext>
    {
    }

    public interface INonRootObjectOdataIdReader<T> : IReader<INonRootObjectReader<T>, OdataId>
    {
    }

    public interface INonRootObjectPropertyReader<T>
    {
    }

    public interface INonRootObjectPropertyNameReader<T> : IReader<PropertyName, INonRootObjectPropertyValueReader<T>>
    {
    }

    public interface INonRootObjectPropertyValueReader<T> : IReader<NonRootObjectPropertyValueToken<T>>
    {
    }

    public readonly ref struct NonRootObjectPropertyValueToken<T>
    {
        public NonRootObjectPropertyValueToken(INonRootObjectPrimitivePropertyValueReader<T> nonRootObjectPrimitivePropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectNullPropertyValueReader<T> nonRootObjectNullPropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectMultiValuedPropertyValueReader<T> nonRootObjectMultiValuedPropertyValueReader)
        {
        }

        public NonRootObjectPropertyValueToken(INonRootObjectComplexPropertyValueReader<T> nonRootObjectComplexPropertyValueReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectPrimitivePropertyValueReader<T> : IReader<T, PrimitivePropertyValue>
    {
    }

    public interface INonRootObjectNullPropertyValueReader<T> : IReader<T>
    {
    }

    public interface INonRootObjectMultiValuedPropertyValueReader<T> : IReader<NonRootObjectMultiValuedPropertyValueToken<T>>
    {
    }

    public readonly ref struct NonRootObjectMultiValuedPropertyValueToken<T>
    {
        public NonRootObjectMultiValuedPropertyValueToken(INonRootObjectPrimitiveElementReader<T> nonRootObjectPrimitiveElementReader)
        {
        }

        public NonRootObjectMultiValuedPropertyValueToken(INonRootObjectComplexElementReader<T> nonRootObjectComplexElementReader)
        {
        }

        public NonRootObjectMultiValuedPropertyValueToken(T next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INonRootObjectPrimitiveElementReader<T> : IReader<INonRootObjectMultiValuedPropertyValueReader<T>, PrimitiveElement>
    {
    }

    public interface INonRootObjectComplexElementReader<T> : IReader<INonRootObjectReader<INonRootObjectMultiValuedPropertyValueReader<T>>>
    {
    }

    public interface INonRootObjectComplexPropertyValueReader<T> : IReader<INonRootObjectReader<T>>
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
