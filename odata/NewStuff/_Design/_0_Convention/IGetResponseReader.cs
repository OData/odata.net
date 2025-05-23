namespace NewStuff._Design._0_Convention
{
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
            private ContentType()
            {
            }

            public IContentTypeHeaderReader ContentTypeHeaderReader { get; }
        }

        public sealed class Custom : GetResponseHeaderToken
        {
            private Custom()
            {
            }

            public ICustomHeaderReader<IGetResponseHeaderReader> CustomHeaderReader { get; }
        }

        public sealed class Body : GetResponseHeaderToken
        {
            private Body()
            {
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
        private ContentType()
        {
        }
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

            public IPropertyReader PropertyReader { get; }
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
        private OdataContext()
        {
        }
    }

    public interface INextLinkReader
    {
        NextLink NextLink { get; }

        IGetResponseBodyReader Next();
    }

    public sealed class NextLink
    {
        private NextLink()
        {
        }
    }

    public interface IPropertyReader
    {
        IPropertyNameReader Next();
    }

    public interface IPropertyNameReader
    {
        PropertyName PropertyName { get; }

        IPropertyValueReader Next();
    }

    public sealed class PropertyName
    {
        private PropertyName()
        {
        }
    }

    public interface IPropertyValueReader
    {
        PropertyValueToken Next();
    }

    public abstract class PropertyValueToken
    {
        private PropertyValueToken()
        {
        }

        public sealed class Primitive : PropertyValueToken
        {
            private Primitive()
            {
            }

            public IPrimitivePropertyValueReader PrimitivePropertyValueReader { get; }
        }

        public sealed class Complex : PropertyValueToken
        {
            private Complex()
            {
            }

            public IComplexPropertyValueReader ComplexPropertyValueReader { get; }
        }

        public sealed class MultiValued : PropertyValueToken
        {
            private MultiValued()
            {
            }

            public IMultiValuedPropertyValueReader MultiValuedPropertyValueReader { get; }
        }

        public sealed class Null : PropertyValueToken
        {
            private Null()
            {
            }

            public INullPropertyValueReader NullPropertyValueReader { get; }
        }
    }

    public interface IPrimitivePropertyValueReader
    {
        PrimitivePropertyValue PrimitivePropertyValue { get; }

        IGetResponseBodyReader Next();
    }

    public sealed class PrimitivePropertyValue
    {
        private PrimitivePropertyValue()
        {
        }

        //// TODO this should probably be a "token" and have different members for each kind of primitive property; or maybe you can't do that without the edm model so this isn't the right place?
    }

    public interface IComplexPropertyValueReader
    {
        ComplexPropertyValueToken Next();
    }

    public abstract class ComplexPropertyValueToken
    {
        private ComplexPropertyValueToken()
        {
        }

        public sealed class OdataContext : ComplexPropertyValueToken
        {
            private OdataContext()
            {
            }

            public IOdataContextReader<IComplexPropertyValueReader> OdataContextReader { get; }
        }

        public sealed class OdataId : ComplexPropertyValueToken
        {
            private OdataId()
            {
            }

            public IOdataIdReader OdataIdReader { get; }
        }

        public sealed class Property : ComplexPropertyValueToken
        {
            private Property()
            {
            }

            public IPropertyReader PropertyReader { get; }
        }
    }

    public interface IOdataIdReader
    {
        OdataId OdataId { get; }

        IComplexPropertyValueReader Next();
    }

    public sealed class OdataId
    {
        private OdataId()
        {
        }
    }

    public interface IMultiValuedPropertyValueReader
    {
        IComplexPropertyValueReader Next(); //// TODO what about edm.untyped nested collections?
        //// TODO this doesn't even loop back to the next value; you need a generic somewhere
    }

    public interface INullPropertyValueReader
    {
        //// TODO you are here unless you've had an epiphany about the above todos
        //// TODO have an "end" member of complexpropertyvaluetoken and have it return the T reader
    }
}
