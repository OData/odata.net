namespace NewStuff._Design._0_Convention
{
    using System;
    using System.IO;

    public interface IGetResponseReader
    {
        IGetResponseHeaderReader Next();
    }

    public interface IGetResponseHeaderReader
    {
        GetResponseHeaderToken Next();
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
    }

    public interface IContentTypeHeaderReader
    {
        ContentType ContentType { get; }

        IGetResponseHeaderReader Next();
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
        GetResponseBodyToken Next();
    }

    public abstract class GetResponseBodyToken
    {
        private GetResponseBodyToken()
        {
        }

        public sealed class OdataContext : GetResponseBodyToken
        {
            private OdataContext()
            {
            }

            public IOdataContextReader<IGetResponseBodyReader> OdataContextReader { get; }
        }

        public sealed class NextLink : GetResponseBodyToken
        {
            private NextLink()
            {
            }

            //// TODO do you like that you're allowing a nextlink in a response that is otherwise single-valued? which layer should be responsible for figuring that stuff out?
            
            public INextLinkReader NextLinkReader { get; }
        }

        public sealed class Property : GetResponseBodyToken
        {
            private Property()
            {
            }

            public IPropertyReader<IGetResponseBodyReader> PropertyReader { get; }
        }

        public sealed class End : GetResponseBodyToken
        {
            private End()
            {
            }

            //// TODO is this a good way to model this?
        }
    }

    public interface IOdataContextReader<T>
    {
        OdataContext OdataContext { get; }

        T Next();
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

        IGetResponseBodyReader Next();
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
        IPropertyNameReader<T> Next();
    }

    public interface IPropertyNameReader<T>
    {
        PropertyName PropertyName { get; }

        IPropertyValueReader<T> Next();
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
        PropertyValueToken<T> Next();
    }

    public abstract class PropertyValueToken<T>
    {
        private PropertyValueToken()
        {
        }

        public sealed class Primitive : PropertyValueToken<T>
        {
            private Primitive()
            {
            }

            public IPrimitivePropertyValueReader<T> PrimitivePropertyValueReader { get; }
        }

        public sealed class Complex : PropertyValueToken<T>
        {
            private Complex()
            {
            }

            public IComplexPropertyValueReader<T> ComplexPropertyValueReader { get; }
        }

        public sealed class MultiValued : PropertyValueToken<T>
        {
            private MultiValued()
            {
            }

            public IMultiValuedPropertyValueReader<T> MultiValuedPropertyValueReader { get; }
        }

        public sealed class Null : PropertyValueToken<T>
        {
            private Null()
            {
            }

            public INullPropertyValueReader<T> NullPropertyValueReader { get; }
        }
    }

    public interface IPrimitivePropertyValueReader<T>
    {
        PrimitivePropertyValue PrimitivePropertyValue { get; }

        T Next();
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
        ComplexPropertyValueToken<T> Next();
    }

    public abstract class ComplexPropertyValueToken<T>
    {
        private ComplexPropertyValueToken()
        {
        }

        public sealed class OdataContext : ComplexPropertyValueToken<T>
        {
            private OdataContext()
            {
            }

            public IOdataContextReader<IComplexPropertyValueReader<T>> OdataContextReader { get; }
        }

        public sealed class OdataId : ComplexPropertyValueToken<T>
        {
            private OdataId()
            {
            }

            public IOdataIdReader<IComplexPropertyValueReader<T>> OdataIdReader { get; }
        }

        public sealed class Property : ComplexPropertyValueToken<T>
        {
            private Property()
            {
            }

            public IPropertyReader<IComplexPropertyValueReader<T>> PropertyReader { get; }
        }

        public sealed class End : ComplexPropertyValueToken<T>
        {
            private End()
            {
            }

            public T Reader { get; }
        }
    }

    public interface IOdataIdReader<T>
    {
        OdataId OdataId { get; }

        T Next();
    }

    public sealed class OdataId
    {
        private OdataId()
        {
        }
    }

    public interface IMultiValuedPropertyValueReader<T>
    {
        MultiValuedPropertyValueToken<T> Next(); //// TODO what about edm.untyped nested collections?
    }

    public abstract class MultiValuedPropertyValueToken<T>
    {
        private MultiValuedPropertyValueToken()
        {
        }

        public sealed class Object : MultiValuedPropertyValueToken<T>
        {
            private Object()
            {
            }

            public IComplexPropertyValueReader<IMultiValuedPropertyValueReader<T>> ComplexPropertyValueReader { get; }
        }

        public sealed class End : MultiValuedPropertyValueToken<T>
        {
            private End()
            {
            }

            public T Reader { get; }
        }
    }

    public interface INullPropertyValueReader<T>
    {
        T Next();
    }
}
