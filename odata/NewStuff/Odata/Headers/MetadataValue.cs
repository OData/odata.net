namespace NewStuff.Odata.Headers
{
    using NewStuff.Http.Inners;

    /// <summary>
    /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheAmountofControlInforma
    /// </summary>
    public abstract class MetadataValue
    {
        private MetadataValue()
        {
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadataminimalodatametadataminimal
        /// </summary>
        public sealed class Minimal : MetadataValue
        {
            private Minimal()
            {
                //// TODO this is supposed to be case-insensitive
                _m = TokenChar._m.Instance;
                _i = TokenChar._i.Instance;
                _n = TokenChar._n.Instance;
                _i_2 = TokenChar._i.Instance;
                _m_2 = TokenChar._m.Instance;
                _a = TokenChar._a.Instance;
                _l = TokenChar._l.Instance;
            }

            public static Minimal Instance { get; } = new Minimal();

            public TokenChar._m _m { get; }
            public TokenChar._i _i { get; }
            public TokenChar._n _n { get; }
            public TokenChar._i _i_2 { get; }
            public TokenChar._m _m_2 { get; }
            public TokenChar._a _a { get; }
            public TokenChar._l _l { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadatafullodatametadatafull
        /// </summary>
        public sealed class Full : MetadataValue
        {
            private Full()
            {
                //// TODO this is supposed to be case-insensitive
                _f = TokenChar._f.Instance;
                _u = TokenChar._u.Instance;
                _l = TokenChar._l.Instance;
                _l_2 = TokenChar._l.Instance;
            }

            public static Full Instance { get; } = new Full();

            public TokenChar._f _f { get; }
            public TokenChar._u _u { get; }
            public TokenChar._l _l { get; }
            public TokenChar._l _l_2 { get; }
        }

        /// <summary>
        /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_metadatanoneodatametadatanone
        /// </summary>
        public sealed class None : MetadataValue
        {
            private None()
            {
                //// TODO this is supposed to be case-insensitive
                _n = TokenChar._n.Instance;
                _o = TokenChar._o.Instance;
                _n_2 = TokenChar._n.Instance;
                _e = TokenChar._e.Instance;
            }

            public TokenChar._n _n { get; }
            public TokenChar._o _o { get; }
            public TokenChar._n _n_2 { get; }
            public TokenChar._e _e { get; }
        }
    }
}
