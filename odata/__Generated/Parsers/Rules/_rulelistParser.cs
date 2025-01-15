namespace __Generated.Parsers.Rules
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _rulelistParser
    {
        public static Parser<__Generated.CstNodes.Rules._rulelist> Instance { get; } = from _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in __Generated.Parsers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._rulelist(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1.Convert2());
    }
    
}
