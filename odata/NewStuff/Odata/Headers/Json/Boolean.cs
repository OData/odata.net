namespace NewStuff.Odata.Headers.Json
{
    using NewStuff.Http.Inners;

    /// <summary>
    /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_ControllingtheRepresentationofNumber
    /// </summary>
    public abstract class Boolean
    {
        private Boolean()
        {
            //// TODO this might be more general than the `Json` namespace
        }

        public sealed class True : Boolean
        {
            private True()
            {
                //// TODO this is supposed to be case-insensitive
                _t = TokenChar._t.Instance;
                _r = TokenChar._r.Instance;
                _u = TokenChar._u.Instance;
                _e = TokenChar._e.Instance;
            }

            public static True Instance { get; } = new True();

            public TokenChar._t _t { get; }
            public TokenChar._r _r { get; }
            public TokenChar._u _u { get; }
            public TokenChar._e _e { get; }
        }

        public sealed class False : Boolean
        {
            private False()
            {
                //// TODO this is supposed to be case-insensitive
                _f = TokenChar._f.Instance;
                _a = TokenChar._a.Instance;
                _l = TokenChar._l.Instance;
                _s = TokenChar._s.Instance;
                _e = TokenChar._e.Instance;
            }

            public TokenChar._f _f { get; }
            public TokenChar._a _a { get; }
            public TokenChar._l _l { get; }
            public TokenChar._s _s { get; }
            public TokenChar._e _e { get; }
        }
    }
}
