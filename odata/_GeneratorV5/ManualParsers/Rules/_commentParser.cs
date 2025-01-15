namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _commentParser
    {
        public static Parser<__Generated.CstNodes.Rules._comment> Instance { get; } =
            from _ʺx3Bʺ_1 in _GeneratorV5.ManualParsers.Inners._ʺx3BʺParser.Instance
            from _ⲤWSPⳆVCHARↃ_1 in _GeneratorV5.ManualParsers.Inners._ⲤWSPⳆVCHARↃParser.Instance.Many()
            from _CRLF_1 in _GeneratorV5.ManualParsers.Rules._CRLFParser.Instance
            select new __Generated.CstNodes.Rules._comment(_ʺx3Bʺ_1, new __Generated.CstNodes.Inners.HelperRangedAtMost0<__Generated.CstNodes.Inners._ⲤWSPⳆVCHARↃ>(_ⲤWSPⳆVCHARↃ_1), _CRLF_1);
    }
}
