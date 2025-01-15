namespace __Generated.Parsers.Rules
{
    using Sprache;
    
    public static class _rulelistParser
    {
        public static Parser<__Generated.CstNodes.Rules._rulelist> Instance { get; } = from _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in __Generated.Parsers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._rulelist(new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ>(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1));
    }
    
}
