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
    }

    public interface IContentTypeHeaderReader
    {
    }
}
