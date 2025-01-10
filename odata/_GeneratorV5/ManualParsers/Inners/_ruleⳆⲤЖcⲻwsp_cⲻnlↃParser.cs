namespace _GeneratorV5.ManualParsers.Inners
{
    using Sprache;

    public static class _ruleⳆⲤЖcⲻwsp_cⲻnlↃParser
    {
        public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ> Instance { get; } =
            Inner._rule
            .Or<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ>(Inner._ⲤЖcⲻwsp_cⲻnlↃ);

        private static class Inner
        {
            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule> _rule { get; } =
                from _rule_1 in _GeneratorV5.ManualParsers.Rules._ruleParser.Instance
                select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule(_rule_1);

            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ> _ⲤЖcⲻwsp_cⲻnlↃ { get; } =
                from _ⲤЖcⲻwsp_cⲻnlↃ_1 in _GeneratorV5.ManualParsers.Inners._ⲤЖcⲻwsp_cⲻnlↃParser.Instance
                select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ(_ⲤЖcⲻwsp_cⲻnlↃ_1);
        }
    }
}
