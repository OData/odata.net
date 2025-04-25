namespace NewStuff.Odata.Headers
{
    using NewStuff.Http.Inners;
    using NewStuff.Odata.SyntacticCst.Json;

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
            public ExponentialDecimals(Boolean value)
            {
                //// TODO this is supposed to be case-insensitive
                Semicolon = TokenChar.Semicolon.Instance;
                E = TokenChar.E.Instance;
                _x = TokenChar._x.Instance;
                _p = TokenChar._p.Instance;
                _o = TokenChar._o.Instance;
                _n = TokenChar._n.Instance;
                _e = TokenChar._e.Instance;
                _n_2 = TokenChar._n.Instance;
                _t = TokenChar._t.Instance;
                _i = TokenChar._i.Instance;
                _a = TokenChar._a.Instance;
                _l = TokenChar._l.Instance;
                _s = TokenChar._s.Instance;
                D = TokenChar.D.Instance;
                _e_2 = TokenChar._e.Instance;
                _c = TokenChar._c.Instance;
                _i_2 = TokenChar._i.Instance;
                _m = TokenChar._m.Instance;
                _a_2 = TokenChar._a.Instance;
                _l_2 = TokenChar._l.Instance;
                _s = TokenChar._s.Instance;
                EqualsSign = TokenChar.EqualsSign.Instance;
                Value = value;
            }

            public TokenChar.Semicolon Semicolon { get; }
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
            public TokenChar.EqualsSign EqualsSign { get; }
            public Boolean Value { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
        /// </summary>
        public sealed class IEEE754Compatible : FormatParameter
        {
            public IEEE754Compatible(Boolean value)
            {
                //// TODO this is supposed to be case-insensitive
                Semicolon = TokenChar.Semicolon.Instance;
                I = TokenChar.I.Instance;
                E = TokenChar.E.Instance;
                E_2 = TokenChar.E.Instance;
                E_3 = TokenChar.E.Instance;
                _7 = TokenChar._7.Instance;
                _5 = TokenChar._5.Instance;
                _4 = TokenChar._4.Instance;
                C = TokenChar.C.Instance;
                _o = TokenChar._o.Instance;
                _m = TokenChar._m.Instance;
                _p = TokenChar._p.Instance;
                _a = TokenChar._a.Instance;
                _t = TokenChar._t.Instance;
                _i = TokenChar._i.Instance;
                _b = TokenChar._b.Instance;
                _l = TokenChar._l.Instance;
                _e = TokenChar._e.Instance;
                EqualsSign = TokenChar.EqualsSign.Instance;
                Value = value;
            }

            public TokenChar.Semicolon Semicolon { get; }
            public TokenChar.I I { get; }
            public TokenChar.E E { get; }
            public TokenChar.E E_2 { get; }
            public TokenChar.E E_3 { get; }
            public TokenChar._7 _7 { get; }
            public TokenChar._5 _5 { get; }
            public TokenChar._4 _4 { get; }
            public TokenChar.C C { get; }
            public TokenChar._o _o { get; }
            public TokenChar._m _m { get; }
            public TokenChar._p _p { get; }
            public TokenChar._a _a { get; }
            public TokenChar._t _t { get; }
            public TokenChar._i _i { get; }
            public TokenChar._b _b { get; }
            public TokenChar._l _l { get; }
            public TokenChar._e _e { get; }
            public TokenChar.EqualsSign EqualsSign { get; }
            public Boolean Value { get; }
        }

        public sealed class Metadata : FormatParameter
        {
            public Metadata(MetadataFormatParameter metadataFormatParameter)
            {
                MetadataFormatParameter = metadataFormatParameter;
            }

            public MetadataFormatParameter MetadataFormatParameter { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_PayloadOrderingConstraints
        /// </summary>
        public sealed class Streaming : FormatParameter
        {
            private Streaming()
            {
                //// TODO this is supposed to be case-insensitive
                Semicolon = TokenChar.Semicolon.Instance;
                _s = TokenChar._s.Instance;
                _t = TokenChar._t.Instance;
                _r = TokenChar._r.Instance;
                _e = TokenChar._e.Instance;
                _a = TokenChar._a.Instance;
                _m = TokenChar._m.Instance;
                _i = TokenChar._i.Instance;
                _n = TokenChar._n.Instance;
                _g = TokenChar._g.Instance;
                EqualsSign = TokenChar.EqualsSign.Instance;
                _t_2 = TokenChar._t.Instance;
                _r_2 = TokenChar._r.Instance;
                _u = TokenChar._u.Instance;
                _e_2 = TokenChar._e.Instance;
            }

            public static Streaming Instance { get; } = new Streaming();

            public TokenChar.Semicolon Semicolon { get; }
            public TokenChar._s _s { get; }
            public TokenChar._t _t { get; }
            public TokenChar._r _r { get; }
            public TokenChar._e _e { get; }
            public TokenChar._a _a { get; }
            public TokenChar._m _m { get; }
            public TokenChar._i _i { get; }
            public TokenChar._n _n { get; }
            public TokenChar._g _g { get; }
            public TokenChar.EqualsSign EqualsSign { get; }
            // it's a bit strange, but the standard doesn't seem to indicate that `streaming=false` is an allowed parameter
            public TokenChar._t _t_2 { get; }
            public TokenChar._r _r_2 { get; }
            public TokenChar._u _u { get; }
            public TokenChar._e _e_2 { get; }
        }
    }
}
