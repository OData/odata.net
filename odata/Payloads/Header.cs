namespace Payloads
{
    /// <summary>
    /// RFC 2616: https://www.rfc-editor.org/rfc/rfc2616#page-31
    /// </summary>
    public abstract class Header
    {
        private Header()
        {
        }

        public sealed class General
        {
            public General(GeneralHeader header)
            {
                Header = header;
            }

            public GeneralHeader Header { get; }
        }

        public sealed class Request
        {
            public Request(RequestHeader header)
            {
                Header = header;
            }

            public RequestHeader Header { get; }
        }

        public sealed class Response
        {
            public Response(ResponseHeader header)
            {
                Header = header;
            }

            public ResponseHeader Header { get; }
        }

        public sealed class Entity
        {
            public Entity(EntityHeader header)
            {
                Header = header;
            }

            public EntityHeader Header { get; }
        }

        public sealed class Custom
        {
            public Custom(CustomHeader header)
            {
                Header = header;
            }

            public CustomHeader Header { get; }
        }
    }

    public sealed class GeneralHeader
    {
        private GeneralHeader()
        {
        }
    }

    public sealed class RequestHeader
    {
        private RequestHeader()
        {
        }
    }

    public sealed class ResponseHeader
    {
        private ResponseHeader()
        {
        }
    }

    public sealed class EntityHeader
    {
        private EntityHeader()
        {
        }
    }

    public sealed class CustomHeader
    {
        private CustomHeader()
        {
        }
    }
}
