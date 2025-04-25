namespace NewStuff.Odata.Headers
{
    using NewStuff.Http.Inners;

    public abstract class MetadataFormatParameter
    {
        private MetadataFormatParameter()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheAmountofControlInforma
        /// </summary>
        public sealed class OdataMetadata : MetadataFormatParameter
        {
            public OdataMetadata(MetadataValue value)
            {
                //// TODO this is supposed to be case-insensitive
                Semicolon = TokenChar.Semicolon.Instance;
                _o = TokenChar._o.Instance;
                _d = TokenChar._d.Instance;
                _a = TokenChar._a.Instance;
                _t = TokenChar._t.Instance;
                _a_2 = TokenChar._a.Instance;
                Period = TokenChar.Period.Instance;
                _m = TokenChar._m.Instance;
                _e = TokenChar._e.Instance;
                _t_2 = TokenChar._t.Instance;
                _a_3 = TokenChar._a.Instance;
                _d_2 = TokenChar._d.Instance;
                _a_4 = TokenChar._a.Instance;
                _t_3 = TokenChar._t.Instance;
                _a_5 = TokenChar._a.Instance;
                EqualsSign = TokenChar.EqualsSign.Instance;
                Value = value;
            }

            public TokenChar.Semicolon Semicolon { get; }
            public TokenChar._o _o { get; }
            public TokenChar._d _d { get; }
            public TokenChar._a _a { get; }
            public TokenChar._t _t { get; }
            public TokenChar._a _a_2 { get; }
            public TokenChar.Period Period { get; }
            public TokenChar._m _m { get; }
            public TokenChar._e _e { get; }
            public TokenChar._t _t_2 { get; }
            public TokenChar._a _a_3 { get; }
            public TokenChar._d _d_2 { get; }
            public TokenChar._a _a_4 { get; }
            public TokenChar._t _t_3 { get; }
            public TokenChar._a _a_5 { get; }
            public TokenChar.EqualsSign EqualsSign { get; }
            public MetadataValue Value { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheAmountofControlInforma
        /// </summary>
        public sealed class Metadata : MetadataFormatParameter
        {
            public Metadata(MetadataValue value)
            {
                //// TODO this is supposed to be case-insensitive
                Semicolon = TokenChar.Semicolon.Instance;
                _m = TokenChar._m.Instance;
                _e = TokenChar._e.Instance;
                _t = TokenChar._t.Instance;
                _a = TokenChar._a.Instance;
                _d = TokenChar._d.Instance;
                _a_2 = TokenChar._a.Instance;
                _t_2 = TokenChar._t.Instance;
                _a_3 = TokenChar._a.Instance;
                EqualsSign = TokenChar.EqualsSign.Instance;
                Value = value;
            }

            public TokenChar.Semicolon Semicolon { get; }
            public TokenChar._m _m { get; }
            public TokenChar._e _e { get; }
            public TokenChar._t _t { get; }
            public TokenChar._a _a { get; }
            public TokenChar._d _d { get; }
            public TokenChar._a _a_2 { get; }
            public TokenChar._t _t_2 { get; }
            public TokenChar._a _a_3 { get; }
            public TokenChar.EqualsSign EqualsSign { get; }
            public MetadataValue Value { get; }
        }
    }
}
