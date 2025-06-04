namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Threading.Tasks;

    public interface IGetResponseReader : IAsyncDisposable
    {
        Task<IGetResponseHeaderReader> Next();
    }

    public interface IGetResponseHeaderReader
    {
        Task<GetResponseHeaderToken> Next();
    }

    public abstract class GetResponseHeaderToken
    {
        private GetResponseHeaderToken()
        {
        }

        public sealed class ContentType : GetResponseHeaderToken
        {
            public ContentType(IContentTypeHeaderReader contentTypeHeaderReader)
            {
                this.ContentTypeHeaderReader = contentTypeHeaderReader;
            }

            public IContentTypeHeaderReader ContentTypeHeaderReader { get; }
        }

        public sealed class Custom : GetResponseHeaderToken
        {
            public Custom(ICustomHeaderReader<IGetResponseHeaderReader> customHeaderReader)
            {
                this.CustomHeaderReader = customHeaderReader;
            }

            public ICustomHeaderReader<IGetResponseHeaderReader> CustomHeaderReader { get; }
        }

        public sealed class Body : GetResponseHeaderToken
        {
            public Body(IGetResponseBodyReader getResponseBodyReader)
            {
                GetResponseBodyReader = getResponseBodyReader;
            }

            public IGetResponseBodyReader GetResponseBodyReader { get; }
        }

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(ContentType node);

            Task<TResult> Accept(Custom node);

            Task<TResult> Accept(Body node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is ContentType contentType)
            {
                return await accepter.Accept(contentType).ConfigureAwait(false);
            }
            else if (this is Custom custom)
            {
                return await accepter.Accept(custom).ConfigureAwait(false);
            }
            else if (this is Body body)
            {
                return await accepter.Accept(body).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<ContentType, Task<TResult>> contentTypeAccepter;
            private readonly Func<Custom, Task<TResult>> customAccepter;
            private readonly Func<Body, Task<TResult>> bodyAccepter;

            public DelegateAccepter(
                Func<ContentType, Task<TResult>> contentTypeAccepter,
                Func<Custom, Task<TResult>> customAccepter,
                Func<Body, Task<TResult>> bodyAccepter)
            {
                this.contentTypeAccepter = contentTypeAccepter;
                this.customAccepter = customAccepter;
                this.bodyAccepter = bodyAccepter;
            }

            public Task<TResult> Accept(ContentType node)
            {
                return this.contentTypeAccepter(node);
            }

            public Task<TResult> Accept(Custom node)
            {
                return this.customAccepter(node);
            }

            public Task<TResult> Accept(Body node)
            {
                return this.bodyAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<ContentType, Task<TResult>> contentTypeAccepter,
            Func<Custom, Task<TResult>> customAccepter,
            Func<Body, Task<TResult>> bodyAccepter)
        {
            return this.Dispatch(new DelegateAccepter<TResult>(contentTypeAccepter, customAccepter, bodyAccepter));
        }
    }

    public interface IContentTypeHeaderReader
    {
        ContentType ContentType { get; }

        Task<IGetResponseHeaderReader> Next();
    }

    public sealed class ContentType
    {
        internal ContentType(string value)
        {
            Value = value;
        }

        internal string Value { get; } //// TODO more strongly-type this
    }











    //// TODO you are doing all of this to expose inversion of control; this is what allows the framework to be externally extensible; sure, you could just write code that has a bunch of `if` statements, but then you need to have all of this callback injection everywhere for the cases where the customer wants to do their own thing; having the inversion of control let's you codify the standard without the logic; the logic is inherent in the model










    public interface IGetResponseBodyReader
    {
        Task<GetResponseBodyToken> Next();
    }

    public abstract class GetResponseBodyToken
    {
        private GetResponseBodyToken()
        {
        }

        public sealed class OdataContext : GetResponseBodyToken
        {
            public OdataContext(IOdataContextReader<IGetResponseBodyReader> odataContextReader)
            {
                OdataContextReader = odataContextReader;
            }

            public IOdataContextReader<IGetResponseBodyReader> OdataContextReader { get; }
        }

        public sealed class NextLink : GetResponseBodyToken
        {
            public NextLink(INextLinkReader nextLinkReader)
            {
                this.NextLinkReader = nextLinkReader;
            }

            //// TODO do you like that you're allowing a nextlink in a response that is otherwise single-valued? which layer should be responsible for figuring that stuff out?
            
            public INextLinkReader NextLinkReader { get; }
        }

        public sealed class Property : GetResponseBodyToken
        {
            public Property(IPropertyReader<IGetResponseBodyReader> propertyReader)
            {
                this.PropertyReader = propertyReader;
            }

            public IPropertyReader<IGetResponseBodyReader> PropertyReader { get; }
        }

        public sealed class End : GetResponseBodyToken
        {
            private End()
            {
            }

            public static End Instance { get; } = new End();

            //// TODO is this a good way to model this?
        }

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(OdataContext node);

            Task<TResult> Accept(NextLink node);

            Task<TResult> Accept(Property node);

            Task<TResult> Accept(End node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is OdataContext odataContext)
            {
                return await accepter.Accept(odataContext).ConfigureAwait(false);
            }
            else if (this is NextLink nextLink)
            {
                return await accepter.Accept(nextLink).ConfigureAwait(false);
            }
            else if (this is Property property)
            {
                return await accepter.Accept(property).ConfigureAwait(false);
            }
            else if (this is End end)
            {
                return await accepter.Accept(end).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<OdataContext, Task<TResult>> odataContextAccepter;
            private readonly Func<NextLink, Task<TResult>> nextLinkAccepter;
            private readonly Func<Property, Task<TResult>> propertyAccepter;
            private readonly Func<End, Task<TResult>> endAccepter;

            public DelegateAccepter(
                Func<OdataContext, Task<TResult>> odataContextAccepter,
                Func<NextLink, Task<TResult>> nextLinkAccepter,
                Func<Property, Task<TResult>> propertyAccepter,
                Func<End, Task<TResult>> endAccepter)
            {
                this.odataContextAccepter = odataContextAccepter;
                this.nextLinkAccepter = nextLinkAccepter;
                this.propertyAccepter = propertyAccepter;
                this.endAccepter = endAccepter;
            }

            public Task<TResult> Accept(OdataContext node)
            {
                return this.odataContextAccepter(node);
            }

            public Task<TResult> Accept(NextLink node)
            {
                return this.nextLinkAccepter(node);
            }

            public Task<TResult> Accept(Property node)
            {
                return this.propertyAccepter(node);
            }

            public Task<TResult> Accept(End node)
            {
                return this.endAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<OdataContext, Task<TResult>> odataContextAccepter,
            Func<NextLink, Task<TResult>> nextLinkAccepter,
            Func<Property, Task<TResult>> propertyAccepter,
            Func<End, Task<TResult>> endAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    odataContextAccepter, 
                    nextLinkAccepter, 
                    propertyAccepter, 
                    endAccepter));
        }
    }

    public interface IOdataContextReader<T>
    {
        OdataContext OdataContext { get; }

        Task<T> Next();
    }

    public sealed class OdataContext
    {
        internal OdataContext(string context)
        {
            Context = context;
        }

        internal string Context { get; }
    }

    public interface INextLinkReader
    {
        NextLink NextLink { get; }

        Task<IGetResponseBodyReader> Next();
    }

    public sealed class NextLink
    {
        internal NextLink(Uri uri)
        {
            Uri = uri;
        }

        internal Uri Uri { get; }
    }

    public interface IPropertyReader<T>
    {
        Task<IPropertyNameReader<T>> Next();
    }

    public interface IPropertyNameReader<T>
    {
        PropertyName PropertyName { get; }

        Task<IPropertyValueReader<T>> Next();
    }

    public sealed class PropertyName
    {
        internal PropertyName(string name)
        {
            Name = name;
        }

        internal string Name { get; }
    }

    public interface IPropertyValueReader<T>
    {
        Task<PropertyValueToken<T>> Next();
    }

    public abstract class PropertyValueToken<T>
    {
        private PropertyValueToken()
        {
        }

        public sealed class Primitive : PropertyValueToken<T>
        {
            public Primitive(IPrimitivePropertyValueReader<T> primitivePropertyValueReader)
            {
                this.PrimitivePropertyValueReader = primitivePropertyValueReader;
            }

            public IPrimitivePropertyValueReader<T> PrimitivePropertyValueReader { get; }
        }

        public sealed class Complex : PropertyValueToken<T>
        {
            public Complex(IComplexPropertyValueReader<T> complexPropertyValueReader)
            {
                this.ComplexPropertyValueReader = complexPropertyValueReader;
            }

            public IComplexPropertyValueReader<T> ComplexPropertyValueReader { get; }
        }

        public sealed class MultiValued : PropertyValueToken<T>
        {
            public MultiValued(IMultiValuedPropertyValueReader<T> multiValuedPropertyValueReader)
            {
                this.MultiValuedPropertyValueReader = multiValuedPropertyValueReader;
            }

            public IMultiValuedPropertyValueReader<T> MultiValuedPropertyValueReader { get; }
        }

        public sealed class Null : PropertyValueToken<T>
        {
            public Null(INullPropertyValueReader<T> nullPropertyValueReader)
            {
                this.NullPropertyValueReader = nullPropertyValueReader;
            }

            public INullPropertyValueReader<T> NullPropertyValueReader { get; }
        }

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(Primitive node);

            Task<TResult> Accept(Complex node);

            Task<TResult> Accept(MultiValued node);

            Task<TResult> Accept(Null node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is Primitive primitive)
            {
                return await accepter.Accept(primitive).ConfigureAwait(false);
            }
            else if (this is Complex complex)
            {
                return await accepter.Accept(complex).ConfigureAwait(false);
            }
            else if (this is MultiValued multiValued)
            {
                return await accepter.Accept(multiValued).ConfigureAwait(false);
            }
            else if (this is Null @null)
            {
                return await accepter.Accept(@null).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<Primitive, Task<TResult>> primitiveAccepter;
            private readonly Func<Complex, Task<TResult>> complexAccepter;
            private readonly Func<MultiValued, Task<TResult>> multiValuedAccepter;
            private readonly Func<Null, Task<TResult>> nullAccepter;

            public DelegateAccepter(
                Func<Primitive, Task<TResult>> primitiveAccepter,
                Func<Complex, Task<TResult>> complexAccepter,
                Func<MultiValued, Task<TResult>> multiValuedAccepter,
                Func<Null, Task<TResult>> nullAccepter)
            {
                this.primitiveAccepter = primitiveAccepter;
                this.complexAccepter = complexAccepter;
                this.multiValuedAccepter = multiValuedAccepter;
                this.nullAccepter = nullAccepter;
            }

            public Task<TResult> Accept(Primitive node)
            {
                return this.primitiveAccepter(node);
            }

            public Task<TResult> Accept(Complex node)
            {
                return this.complexAccepter(node);
            }

            public Task<TResult> Accept(MultiValued node)
            {
                return this.multiValuedAccepter(node);
            }

            public Task<TResult> Accept(Null node)
            {
                return this.nullAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<Primitive, Task<TResult>> primitiveAccepter,
            Func<Complex, Task<TResult>> complexAccepter,
            Func<MultiValued, Task<TResult>> multiValuedAccepter,
            Func<Null, Task<TResult>> nullAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    primitiveAccepter,
                    complexAccepter,
                    multiValuedAccepter,
                    nullAccepter));
        }
    }

    public interface IPrimitivePropertyValueReader<T>
    {
        PrimitivePropertyValue PrimitivePropertyValue { get; }

        Task<T> Next();
    }

    public sealed class PrimitivePropertyValue
    {
        internal PrimitivePropertyValue(string value)
        {
            Value = value;
        }

        internal string Value { get; }

        //// TODO this should probably be a "token" and have different members for each kind of primitive property; or maybe you can't do that without the edm model so this isn't the right place?
    }

    public interface IComplexPropertyValueReader<T>
    {
        Task<ComplexPropertyValueToken<T>> Next();
    }

    public abstract class ComplexPropertyValueToken<T>
    {
        private ComplexPropertyValueToken()
        {
        }

        public sealed class OdataContext : ComplexPropertyValueToken<T>
        {
            public OdataContext(IOdataContextReader<IComplexPropertyValueReader<T>> odataContextReader)
            {
                this.OdataContextReader = odataContextReader;
            }

            public IOdataContextReader<IComplexPropertyValueReader<T>> OdataContextReader { get; }
        }

        public sealed class OdataId : ComplexPropertyValueToken<T>
        {
            public OdataId(IOdataIdReader<IComplexPropertyValueReader<T>> odataIdReader)
            {
                this.OdataIdReader = odataIdReader;
            }

            public IOdataIdReader<IComplexPropertyValueReader<T>> OdataIdReader { get; }
        }

        public sealed class Property : ComplexPropertyValueToken<T>
        {
            public Property(IPropertyReader<IComplexPropertyValueReader<T>> propertyReader)
            {
                this.PropertyReader = propertyReader;
            }

            public IPropertyReader<IComplexPropertyValueReader<T>> PropertyReader { get; }
        }

        public sealed class End : ComplexPropertyValueToken<T>
        {
            public End(T reader)
            {
                this.Reader = reader;
            }

            public T Reader { get; }
        }

        public interface IAccepterAsync<TResult>
        {
            Task<TResult> Accept(OdataContext node);

            Task<TResult> Accept(OdataId node);

            Task<TResult> Accept(Property node);

            Task<TResult> Accept(End node);
        }

        public async Task<TResult> Dispatch<TResult>(IAccepterAsync<TResult> accepter)
        {
            if (this is OdataContext odataContext)
            {
                return await accepter.Accept(odataContext).ConfigureAwait(false);
            }
            else if (this is OdataId odataId)
            {
                return await accepter.Accept(odataId).ConfigureAwait(false);
            }
            else if (this is Property property)
            {
                return await accepter.Accept(property).ConfigureAwait(false);
            }
            else if (this is End end)
            {
                return await accepter.Accept(end).ConfigureAwait(false);
            }
            else
            {
                throw new Exception("TODO use a visitor");
            }
        }

        private sealed class DelegateAccepter<TResult> : IAccepterAsync<TResult>
        {
            private readonly Func<OdataContext, Task<TResult>> odataContextAccepter;
            private readonly Func<OdataId, Task<TResult>> odataIdAccepter;
            private readonly Func<Property, Task<TResult>> propertyAccepter;
            private readonly Func<End, Task<TResult>> endAccepter;

            public DelegateAccepter(
                Func<OdataContext, Task<TResult>> odataContextAccepter,
                Func<OdataId, Task<TResult>> odataIdAccepter,
                Func<Property, Task<TResult>> propertyAccepter,
                Func<End, Task<TResult>> endAccepter)
            {
                this.odataContextAccepter = odataContextAccepter;
                this.odataIdAccepter = odataIdAccepter;
                this.propertyAccepter = propertyAccepter;
                this.endAccepter = endAccepter;
            }

            public Task<TResult> Accept(OdataContext node)
            {
                return this.odataContextAccepter(node);
            }

            public Task<TResult> Accept(OdataId node)
            {
                return this.odataIdAccepter(node);
            }

            public Task<TResult> Accept(Property node)
            {
                return this.propertyAccepter(node);
            }

            public Task<TResult> Accept(End node)
            {
                return this.endAccepter(node);
            }
        }

        public Task<TResult> Dispatch<TResult>(
            Func<OdataContext, Task<TResult>> odataContextAccepter,
            Func<OdataId, Task<TResult>> odataIdAccepter,
            Func<Property, Task<TResult>> propertyAccepter,
            Func<End, Task<TResult>> endAccepter)
        {
            return this.Dispatch(
                new DelegateAccepter<TResult>(
                    odataContextAccepter,
                    odataIdAccepter,
                    propertyAccepter,
                    endAccepter));
        }
    }

    public interface IOdataIdReader<T>
    {
        OdataId OdataId { get; }

        Task<T> Next();
    }

    public sealed class OdataId
    {
        internal OdataId(string value)
        {
            Value = value;
        }

        internal string Value { get; }
    }

    public interface IMultiValuedPropertyValueReader<T>
    {
        Task<MultiValuedPropertyValueToken<T>> Next(); //// TODO what about edm.untyped nested collections?
    }

    public abstract class MultiValuedPropertyValueToken<T>
    {
        private MultiValuedPropertyValueToken()
        {
        }

        public sealed class Object : MultiValuedPropertyValueToken<T>
        {
            public Object(IComplexPropertyValueReader<IMultiValuedPropertyValueReader<T>> complexPropertyValueReader)
            {
                this.ComplexPropertyValueReader = complexPropertyValueReader;
            }

            public IComplexPropertyValueReader<IMultiValuedPropertyValueReader<T>> ComplexPropertyValueReader { get; }
        }

        public sealed class End : MultiValuedPropertyValueToken<T>
        {
            public End(T reader)
            {
                this.Reader = reader;
            }

            public T Reader { get; }
        }
    }

    public interface INullPropertyValueReader<T>
    {
        Task<T> Next();
    }
}
