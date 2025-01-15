namespace _GeneratorV5.ManualParsers.Inners
{
    using Sprache;

    public static class _Жcⲻwsp_cⲻnlParser
    {
        public static Parser<__Generated.CstNodes.Inners._Жcⲻwsp_cⲻnl> Instance { get; } =
            from _cⲻwsp_1 in _GeneratorV5.ManualParsers.Rules._cⲻwspParser.Instance.Many()
            from _cⲻnl_1 in _GeneratorV5.ManualParsers.Rules._cⲻnlParser.Instance
            select new __Generated.CstNodes.Inners._Жcⲻwsp_cⲻnl(new __Generated.CstNodes.Inners.HelperRangedAtMost0<__Generated.CstNodes.Rules._cⲻwsp>(_cⲻwsp_1), _cⲻnl_1);
    }
}
