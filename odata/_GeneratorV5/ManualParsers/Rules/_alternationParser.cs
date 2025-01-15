namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _alternationParser
    {
        public static Parser<__Generated.CstNodes.Rules._alternation> Instance { get; } =
            from _concatenation_1 in _GeneratorV5.ManualParsers.Rules._concatenationParser.Instance
            from _ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1 in _GeneratorV5.ManualParsers.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃParser.Instance.Many()
            select new __Generated.CstNodes.Rules._alternation(_concatenation_1, new __Generated.CstNodes.Inners.HelperRangedAtMost0<__Generated.CstNodes.Inners._ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ>(_ⲤЖcⲻwsp_ʺx2Fʺ_Жcⲻwsp_concatenationↃ_1));
    }
}
