namespace Payloads
{
    using System.Collections.Generic;

    //// TODO i'm still not clear if i'm defining an AST or the CLR definitions of the conventions

    public abstract class OdataRequestPayload
    {
        private OdataRequestPayload()
        {
        }

        public sealed class Json : OdataRequestPayload
        {
            private Json(Payloads.Json.FormatRequestHeader? accept)
            {
                Accept = accept;
            }

            /// <summary>
            /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
            /// </summary>
            public Payloads.Json.FormatRequestHeader? Accept { get; } //// TODO i *think* this is optional, but i can't tell; i guess it's not optional if the service supports other formats and the client only understands JSON, so they want to ensure they only get a JSON response?
        }
    }

    namespace Json
    {
        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// 
        /// based off of https://www.rfc-editor.org/rfc/rfc2616#section-14.1
        /// </summary>
        public sealed class FormatRequestHeader
        {
            public FormatRequestHeader(Type type, Subtype subtype, IEnumerable<FormatParameter> parameters)
            {
                Type = type;
                Subtype = subtype;
                Parameters = parameters;
            }

            public Type Type { get; }
            public Subtype Subtype { get; }
            public IEnumerable<FormatParameter> Parameters { get; }
            
            //// TODO you could have an extension that returns an http accept header from this
        }

        public abstract class FormatParameter
        {
            private FormatParameter()
            {
            }

            public sealed class ExponentialDecimals : FormatParameter
            {
                private ExponentialDecimals()
                {
                }
            }

            public sealed class IEEE754Compatible : FormatParameter
            {
                private IEEE754Compatible()
                {
                }
            }

            public sealed class Metadata : FormatParameter
            {
                private Metadata()
                {
                }
            }

            public sealed class Streaming : FormatParameter
            {
                private Streaming()
                {
                }
            }
        }
    }
}
