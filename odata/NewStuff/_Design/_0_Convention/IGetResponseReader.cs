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
    }
}
