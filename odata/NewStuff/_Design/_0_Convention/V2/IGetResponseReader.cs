namespace NewStuff._Design._0_Convention.Readers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

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

    public interface IGetResponseBodyReader : IComplexObjectReader
    {
    }

    public readonly ref struct GetResponseBodyToken
    {
        public GetResponseBodyToken(INextLinkReader nextLinkReader)
        {
        }

        public GetResponseBodyToken(ComplexObjectToken<IGetResponseBodyReader> complexObjectToken)
        {
        }

        public GetResponseBodyToken()
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IComplexObjectReader : IReader<ComplexObjectToken<IComplexObjectReader>>
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

        public ComplexObjectToken()
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface INextLinkReader : IReader<IGetResponseBodyReader, NextLink>
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

        public PropertyValueToken(IComplexObjectReader complexObjectReader)
        {
        }

        public PropertyValueToken(IMultiValuedPropertyValueReader multiValuedPropertyValueReader)
        {
        }

        public PropertyValueToken(INullPropertyValueReader nullPropertyValueReader)
        {
        }

        //// TODO implement accpeter and dispatch
    }

    public interface IPrimitivePropertyValueReader<T> : IReader<T, PrimitivePropertyValue>
    {
    }

    public interface IMultiValuedPropertyValueReader
    {
        //// TODO you are here, you probably will need generics for this
    }

    public interface INullPropertyValueReader
    {
    }
}
