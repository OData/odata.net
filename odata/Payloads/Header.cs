namespace Payloads
{
    using System.Collections.Generic;

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

        public sealed class Custom : Header
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

    public sealed class MediaRangeAcceptParamsPair
    {
        public MediaRangeAcceptParamsPair(MediaRange mediaRange, AcceptParams acceptParams)
        {
            MediaRange = mediaRange;
            AcceptParams = acceptParams;
        }

        public MediaRange MediaRange { get; }
        public AcceptParams AcceptParams { get; }
    }

    public sealed class MediaRange
    {
        private MediaRange()
        {
        }
    }

    public sealed class AcceptParams
    {
        private AcceptParams()
        {
        }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-5.3
    /// </summary>
    public abstract class RequestHeader
    {
        private RequestHeader()
        {
        }

        /// <summary>
        /// https://www.rfc-editor.org/rfc/rfc2616#section-14.1
        /// </summary>
        public sealed class Accept : RequestHeader
        {
            public Accept(IEnumerable<MediaRangeAcceptParamsPair> mediaRangeAcceptParamsPairs)
            {
                MediaRangeAcceptParamsPairs = mediaRangeAcceptParamsPairs;
            }

            public IEnumerable<MediaRangeAcceptParamsPair> MediaRangeAcceptParamsPairs { get; }
        }

        public sealed class AcceptCharset : RequestHeader
        {
            private AcceptCharset()
            {
            }
        }

        public sealed class AcceptEncoding : RequestHeader
        {
            private AcceptEncoding()
            {
            }
        }

        public sealed class AcceptLanguage : RequestHeader
        {
            private AcceptLanguage()
            {
            }
        }

        public sealed class Authorization : RequestHeader
        {
            private Authorization()
            {
            }
        }

        public sealed class Expect : RequestHeader
        {
            private Expect()
            {
            }
        }

        public sealed class From : RequestHeader
        {
            private From()
            {
            }
        }

        public sealed class Host : RequestHeader
        {
            private Host()
            {
            }
        }

        public sealed class IfMatch : RequestHeader
        {
            private IfMatch()
            {
            }
        }

        public sealed class IfModifiedSince : RequestHeader
        {
            private IfModifiedSince()
            {
            }
        }

        public sealed class IfNoneMatch : RequestHeader
        {
            private IfNoneMatch()
            {
            }
        }

        public sealed class IfRange : RequestHeader
        {
            private IfRange()
            {
            }
        }

        public sealed class IfUnmodifiedSince : RequestHeader
        {
            private IfUnmodifiedSince()
            {
            }
        }

        public sealed class MaxForwards : RequestHeader
        {
            private MaxForwards()
            {
            }
        }

        public sealed class ProxyAuthorization : RequestHeader
        {
            private ProxyAuthorization()
            {
            }
        }

        public sealed class Range : RequestHeader
        {
            private Range()
            {
            }
        }

        public sealed class Referer : RequestHeader
        {
            private Referer()
            {
            }
        }

        public sealed class Te : RequestHeader
        {
            private Te()
            {
            }
        }

        public sealed class UserAgent : RequestHeader
        {
            private UserAgent()
            {
            }
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
