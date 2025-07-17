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

    public interface IGetResponseBodyReader : IReader<GetResponseBodyToken>
    {
    }

    public readonly ref struct GetResponseBodyToken
    {
        public GetResponseBodyToken(IReaderOfANextLinkAtTheRootOfTheResponseBody nextLinkReader)
        {
        }

        public GetResponseBodyToken(IReaderOfAOdataContextAtTheRootOfTheResponseBody dataContextReader)
        {
        }

        public GetResponseBodyToken(IReaderOfAOdataIdAtTheRootOfTheResponseBody odataIdReader)
        {
        }

        public GetResponseBodyToken(IReaderOfAPropertyAtTheRootOfTheResponseBody propertyReader)
        {
        }

        public GetResponseBodyToken()
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IReaderOfANextLinkAtTheRootOfTheResponseBody : IReader<IGetResponseBodyReader, RootNextLink>
    {
    }

    public interface IReaderOfAOdataContextAtTheRootOfTheResponseBody : IReader<IGetResponseBodyReader, RootOdataContext>
    {
    }

    public interface IReaderOfAOdataIdAtTheRootOfTheResponseBody : IReader<IGetResponseBodyReader, RootOdataId>
    {
    }

    public interface IReaderOfAPropertyAtTheRootOfTheResponseBody : IReader<IReaderOfAPropertyNameAtTheRootOfTheResponseBody>
    {
    }

    public interface IReaderOfAPropertyNameAtTheRootOfTheResponseBody : IReader<IRootPropertyValueReader, RootPropertyName>
    {
    }

    public interface IRootPropertyValueReader : IReader<RootPropertyValueToken>
    {
    }

    public readonly ref struct RootPropertyValueToken
    {
        public RootPropertyValueToken(IRootPrimitivePropertyValueReader primitivePropertyValueReader)
        {
        }

        public RootPropertyValueToken(IRootNullPropertyValueReader nullPropertyValueReader)
        {
        }

        public RootPropertyValueToken(IRootMultiValuedPropertyValueReader multiValuedPropertyValueReader)
        {
        }

        public RootPropertyValueToken(IRootComplexPropertyValueReader complexObjectReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootPrimitivePropertyValueReader : IReader<IGetResponseBodyReader, RootPrimitivePropertyValue>
    {
    }

    public interface IRootNullPropertyValueReader : IReader<IGetResponseBodyReader>
    {
    }

    public interface IRootMultiValuedPropertyValueReader : IReader<RootMultiValuedPropertyValueToken>
    {
    }

    public readonly ref struct RootMultiValuedPropertyValueToken
    {
        public RootMultiValuedPropertyValueToken(IRootPrimitiveElementReader primitivePropertyValueReader)
        {
        }

        public RootMultiValuedPropertyValueToken(IRootComplexElementReader complexObjectReader)
        {
        }

        public RootMultiValuedPropertyValueToken(IGetResponseBodyReader getResponseBodyReader)
        {
        }
    }

    public interface IRootPrimitiveElementReader : IReader<IRootMultiValuedPropertyValueReader, RootPrimitiveElement>
    {
    }

    public interface IRootComplexElementReader : IReader<RootComplexElementToken>
    {
    }

    public readonly ref struct RootComplexElementToken
    {
        public RootComplexElementToken(IRootComplexElementOdataContextReader dataContextReader)
        {
        }

        public RootComplexElementToken(IRootComplexElementOdataIdReader odataIdReader)
        {
        }

        public RootComplexElementToken(IRootComplexElementPropertyReader propertyReader)
        {
        }

        public RootComplexElementToken(IRootMultiValuedPropertyValueReader next)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootComplexElementOdataContextReader : IReader<IRootComplexElementReader, RootComplexElementOdataContext>
    {
    }

    public interface IRootComplexElementOdataIdReader : IReader<IRootComplexElementReader, RootComplexElementOdataId>
    {
    }

    public interface IRootComplexElementPropertyReader : IReader<IRootComplexElementPropertyNameReader>
    {
    }

    public interface IRootComplexElementPropertyNameReader : IReader<IRootComplexElementPropertyValueReader, RootComplexElementPropertyName>
    {
    }

    public interface IRootComplexElementPropertyValueReader : IReader<RootComplexElementPropertyValueToken>
    {
    }

    public readonly ref struct RootComplexElementPropertyValueToken
    {
        public RootComplexElementPropertyValueToken(IRootComplexElementPrimitivePropertyValueReader primitivePropertyValueReader)
        {
        }

        public RootComplexElementPropertyValueToken(IRootComplexElementNullPropertyValueReader nullPropertyValueReader)
        {
        }

        public RootComplexElementPropertyValueToken(IRootComplexElementMultiValuedPropertyValueReader multiValuedPropertyValueReader)
        {
        }

        public RootComplexElementPropertyValueToken(IRootComplexElementComplexPropertyValueReader complexObjectReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IRootComplexElementPrimitivePropertyValueReader : IReader<IRootComplexElementReader, RootComplexElementPrimitivePropertyValue>
    {
    }

    public interface IRootComplexElementNullPropertyValueReader : IReader<IRootComplexElementReader>
    {
    }

    public interface IRootComplexElementMultiValuedPropertyValueReader : IReader<RootMultiValuedPropertyValueToken>
    {
        //// TODO you are here
        //// TODO you are trying to implement this, but you're getting confused by the naming at this point and so you don't know where the recursion is happening; this might mean that it's actually not worth it to have different types for the different "levels" within the payload, but it might also mean that this is exactly *why* you need the different types (if there can actually be differences between the levels, which currently there is for nextlink being only available in the root, but who's to say that there aren't future versions of the standard which further separate the "levels"
    }







    public interface IRootComplexElementComplexPropertyValueReader
    {
    }









    public interface IRootComplexPropertyValueReader
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

    public interface INextLinkReader : IReader<IGetResponseBodyReader, RootNextLink>
    {
    }

    public interface IOdataContextReader<T> : IReader<T, RootOdataContext>
    {
    }

    public interface IOdataIdReader<T> : IReader<T, RootOdataId>
    {
    }

    public interface IPropertyReader<T> : IReader<IPropertyNameReader<T>>
    {
    }

    public interface IPropertyNameReader<T> : IReader<IPropertyValueReader<T>, RootPropertyName>
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

    public interface IPrimitivePropertyValueReader<T> : IReader<T, RootPrimitivePropertyValue>
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
