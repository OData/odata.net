namespace NewStuff.Odata.Headers
{
    using NewStuff.Http.Inners;
    using System.Collections.Generic;

    /// <summary>
    /// https://docs.oasis-open.org/odata/odata-json-format/v4.01/odata-json-format-v4.01.html#sec_RequestingtheJSONFormat
    /// 
    /// based off of https://www.rfc-editor.org/rfc/rfc2616#section-14.1
    /// </summary>
    public sealed class JsonFormatRequestHeader
    {
        public JsonFormatRequestHeader(IEnumerable<FormatParameter> parameters)
        {
            _a = TokenChar._a.Instance;
            _p = TokenChar._p.Instance;
            _p_2 = TokenChar._p.Instance;
            _l = TokenChar._l.Instance;
            _i = TokenChar._i.Instance;
            _c = TokenChar._c.Instance;
            _a_2 = TokenChar._a.Instance;
            _t = TokenChar._t.Instance;
            _i_2 = TokenChar._i.Instance;
            _o = TokenChar._o.Instance;
            _n = TokenChar._n.Instance;
            Slash = TokenChar.ForwardSlash.Instance;
            _j = TokenChar._j.Instance;
            _s = TokenChar._s.Instance;
            _o_2 = TokenChar._o.Instance;
            _n_2 = TokenChar._n.Instance;
            Parameters = parameters;
        }

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
        public TokenChar.ForwardSlash Slash { get; }
        public TokenChar._j _j { get; }
        public TokenChar._s _s { get; }
        public TokenChar._o _o_2 { get; }
        public TokenChar._n _n_2 { get; }
        public IEnumerable<FormatParameter> Parameters { get; }

        //// TODO you could have an extension that returns an http accept header from this
    }
}
