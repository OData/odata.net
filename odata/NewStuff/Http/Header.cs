namespace NewStuff.Http
{
    /// <summary>
    /// RFC 2616: https://www.rfc-editor.org/rfc/rfc2616#section-4.2
    /// </summary>
    public abstract class Header
    {
        private Header()
        {
        }

        public sealed class General : Header
        {
            public General(GeneralHeader header)
            {
                Header = header;
            }

            public GeneralHeader Header { get; }
        }

        public sealed class Request : Header
        {
            public Request(RequestHeader header)
            {
                Header = header;
            }

            public RequestHeader Header { get; }
        }

        public sealed class Response : Header
        {
            public Response(ResponseHeader header)
            {
                Header = header;
            }

            public ResponseHeader Header { get; }
        }

        public sealed class Entity : Header
        {
            public Entity(EntityHeader header)
            {
                Header = header;
            }

            public EntityHeader Header { get; }
        }

        public sealed class Message : Header
        {
            public Message(MessageHeader header)
            {
                Header = header;
            }

            public MessageHeader Header { get; }
        }
    }
}
