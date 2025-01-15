namespace _GeneratorV5.ManualParsers.Rules
{
    using Sprache;

    public static class _rulelistParser
    {
        public static Parser<__Generated.CstNodes.Rules._rulelist> Instance { get; } =
            from _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in _GeneratorV5.ManualParsers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃParser.Instance.AtLeastOnce()
            select new __Generated.CstNodes.Rules._rulelist(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1.Convert2());

    }
}
