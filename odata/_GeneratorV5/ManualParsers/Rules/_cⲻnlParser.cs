namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _cⲻnlParser
    {
        public static Parser<__Generated.CstNodes.Rules._cⲻnl> Instance { get; } =
            Inners._comment
            .Or<__Generated.CstNodes.Rules._cⲻnl>(Inners._CRLF);

        private static class Inners
        {
            public static Parser<__Generated.CstNodes.Rules._cⲻnl._comment> _comment { get; } =
                from _comment_1 in _GeneratorV5.ManualParsers.Rules._commentParser.Instance
                select new __Generated.CstNodes.Rules._cⲻnl._comment(_comment_1);

            public static Parser<__Generated.CstNodes.Rules._cⲻnl._CRLF> _CRLF { get; } =
                from _CRLF_1 in _GeneratorV5.ManualParsers.Rules._CRLFParser.Instance
                select new __Generated.CstNodes.Rules._cⲻnl._CRLF(_CRLF_1);
        }
    }
}
