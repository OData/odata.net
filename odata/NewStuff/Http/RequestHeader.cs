namespace NewStuff.Http
{
    using NewStuff.Http.Inners;
    using System.Collections.Generic;

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
}
