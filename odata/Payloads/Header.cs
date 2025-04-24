namespace Payloads
{
    using System;
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

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-14.1
    /// </summary>
    public sealed class MediaRangeAcceptParamsPair
    {
        public MediaRangeAcceptParamsPair(MediaRange mediaRange, AcceptParams? acceptParams)
        {
            MediaRange = mediaRange;
            AcceptParams = acceptParams;
        }

        public MediaRange MediaRange { get; }
        public AcceptParams? AcceptParams { get; }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-2.2
    /// </summary>
    public sealed class TokenChar
    {
        public TokenChar(char character)
        {
            switch (character)
            {
                case (char)0x00:
                case (char)0x01:
                case (char)0x02:
                case (char)0x03:
                case (char)0x04:
                case (char)0x05:
                case (char)0x06:
                case (char)0x07:
                case (char)0x08:
                case (char)0x09:
                case (char)0x0A:
                case (char)0x0B:
                case (char)0x0C:
                case (char)0x0D:
                case (char)0x0E:
                case (char)0x0F:
                case (char)0x10:
                case (char)0x11:
                case (char)0x12:
                case (char)0x13:
                case (char)0x14:
                case (char)0x15:
                case (char)0x16:
                case (char)0x17:
                case (char)0x18:
                case (char)0x19:
                case (char)0x1A:
                case (char)0x1B:
                case (char)0x1C:
                case (char)0x1D:
                case (char)0x1E:
                case (char)0x1F:
                case (char)0x20:
                case (char)0x21:
                case (char)0x22:
                case (char)0x23:
                case (char)0x24:
                case (char)0x25:
                case (char)0x26:
                case (char)0x27:
                case (char)0x28:
                case (char)0x29:
                case (char)0x2A:
                case (char)0x2B:
                case (char)0x2C:
                case (char)0x2D:
                case (char)0x2E:
                case (char)0x2F:
                case (char)0x30:
                case (char)0x31:
                case (char)127:
                    throw new ArgumentException("TODO");
            }

            Character = character;
            //// TODO if you did this "right", you would have a discriminated union with each supported character
        }

        public char Character { get; }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-2.2
    /// </summary>
    public sealed class Token
    {
        public Token(TokenChar firstCharacter, IEnumerable<TokenChar> subsequentCharacters)
        {
            FirstCharacter = firstCharacter;
            SubsequentCharacters = subsequentCharacters;
        }

        public TokenChar FirstCharacter { get; }
        public IEnumerable<TokenChar> SubsequentCharacters { get; }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-3.7
    /// </summary>
    public sealed class Type
    {
        public Type(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-3.7
    /// </summary>
    public sealed class Subtype
    {
        public Subtype(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
    }

    public sealed class Parameter
    {
        private Parameter()
        {
        }
    }

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-14.1
    /// </summary>
    public abstract class MediaRange
    {
        private MediaRange()
        {
        }

        public sealed class All : MediaRange
        {
            private All()
            {
            }
        }

        public sealed class TypeOnly : MediaRange
        {
            private TypeOnly()
            {
            }
        }

        public sealed class TypeAndSubType : MediaRange
        {
            public TypeAndSubType(Type type, Subtype subtype, IEnumerable<Parameter> parameters)
            {
                Type = type;
                Subtype = subtype;
                Parameters = parameters;
            }

            public Type Type { get; }
            public Subtype Subtype { get; }
            public IEnumerable<Parameter> Parameters { get; }
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
