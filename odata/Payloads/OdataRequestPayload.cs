namespace Payloads
{
    using System.Collections.Generic;

    //// TODO i'm still not clear if i'm defining a CST or the CLR definitions of the conventions

    public abstract class OdataRequestPayload
    {
        private OdataRequestPayload()
        {
        }

        public sealed class Json : OdataRequestPayload
        {
            private Json(FormatRequestHeader? accept)
            {
                Accept = accept;
            }

            //// TODO you need to go through the "protocol" doc to capture the things that apply to all requests/responses/etc
            
            public FormatRequestHeader? Accept { get; }
        }
    }

    public abstract class FormatRequestHeader
    {
        private FormatRequestHeader()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class Json : FormatRequestHeader
        {
            public Json(Payloads.Json.FormatRequestHeader formatRequestHeader)
            {
                FormatRequestHeader = formatRequestHeader;
            }

            public Payloads.Json.FormatRequestHeader FormatRequestHeader { get; }
        }

        //// TODO i don't think other formats are allowed by the standard at the moment; you should figure out how custom formats get supported and add that derived type, though
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
            public FormatRequestHeader(IEnumerable<FormatParameter> parameters)
            {
                this.Type = ApplicationTypeLiteral.Instance;
                this.Slash = TokenChar.ForwardSlash.Instance;
                this.Subtype = JsonSubtypeLiteral.Instance;
                Parameters = parameters;
            }

            public ApplicationTypeLiteral Type { get; }
            public TokenChar.ForwardSlash Slash { get; }
            public JsonSubtypeLiteral Subtype { get; }
            public IEnumerable<FormatParameter> Parameters { get; }
            
            //// TODO you could have an extension that returns an http accept header from this
        }



        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class ApplicationTypeLiteral
        {
            private ApplicationTypeLiteral()
            {
                this._a = TokenChar._a.Instance;
                this._p = TokenChar._p.Instance;
                this._p_2 = TokenChar._p.Instance;
                this._l = TokenChar._l.Instance;
                this._i = TokenChar._i.Instance;
                this._c = TokenChar._c.Instance;
                this._a_2 = TokenChar._a.Instance;
                this._t = TokenChar._t.Instance;
                this._i_2 = TokenChar._i.Instance;
                this._o = TokenChar._o.Instance;
                this._n = TokenChar._n.Instance;
            }

            public static ApplicationTypeLiteral Instance { get; } = new ApplicationTypeLiteral();

            public TokenChar._a _a { get; }
            public TokenChar._p _p { get; }
            public TokenChar._p _p_2 { get; }
            public TokenChar._l _l { get; }
            public TokenChar._i _i { get; }
            public TokenChar._c _c { get; }
            public TokenChar._a _a_2 { get; }
            public TokenChar._t _t { get; }
            public TokenChar._i _i_2 { get; }
            public TokenChar._o _o { get; }
            public TokenChar._n _n { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class JsonSubtypeLiteral
        {
            private JsonSubtypeLiteral()
            {
                this._j = TokenChar._j.Instance;
                this._s = TokenChar._s.Instance;
                this._o = TokenChar._o.Instance;
                this._n = TokenChar._n.Instance;
            }

            public static JsonSubtypeLiteral Instance { get; } = new JsonSubtypeLiteral();

            public TokenChar._j _j { get; }
            public TokenChar._s _s { get; }
            public TokenChar._o _o { get; }
            public TokenChar._n _n { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public abstract class FormatParameter
        {
            private FormatParameter()
            {
            }

            /// <summary>
            /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheRepresentationofNumber
            /// </summary>
            public sealed class ExponentialDecimals : FormatParameter
            {
                private ExponentialDecimals()
                {
                    this.E = TokenChar.E.Instance;
                    this._x = TokenChar._x.Instance;
                    this._p = TokenChar._p.Instance;
                    this._o = TokenChar._o.Instance;
                    this._n = TokenChar._n.Instance;
                    this._e = TokenChar._e.Instance;
                    this._n_2 = TokenChar._n.Instance;
                    this._t = TokenChar._t.Instance;
                    this._i = TokenChar._i.Instance;
                    this._a = TokenChar._a.Instance;
                    this._l = TokenChar._l.Instance;
                    this._s = TokenChar._s.Instance;
                    this.D = TokenChar.D.Instance;
                    this._e_2 = TokenChar._e.Instance;
                    this._c = TokenChar._c.Instance;
                    this._i_2 = TokenChar._i.Instance;
                    this._m = TokenChar._m.Instance;
                    this._a_2 = TokenChar._a.Instance;
                    this._l_2 = TokenChar._l.Instance;
                    this._s = TokenChar._s.Instance;
                }

                public TokenChar.E E { get; }
                public TokenChar._x _x { get; }
                public TokenChar._p _p { get; }
                public TokenChar._o _o { get; }
                public TokenChar._n _n { get; }
                public TokenChar._e _e { get; }
                public TokenChar._n _n_2 { get; }
                public TokenChar._t _t { get; }
                public TokenChar._i _i { get; }
                public TokenChar._a _a { get; }
                public TokenChar._l _l { get; }
                public TokenChar.D D { get; }
                public TokenChar._e _e_2 { get; }
                public TokenChar._c _c { get; }
                public TokenChar._i _i_2 { get; }
                public TokenChar._m _m { get; }
                public TokenChar._a _a_2 { get; }
                public TokenChar._l _l_2 { get; }
                public TokenChar._s _s { get; }
            }

            /// <summary>
            /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
            /// </summary>
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

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheRepresentationofNumber
        /// </summary>
        public sealed class Boolean
        {
            private Boolean()
            {
                //// TODO this might be more general than the `Json` namespace
            }
        }
    }
}
