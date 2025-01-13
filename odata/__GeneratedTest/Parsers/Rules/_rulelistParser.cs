namespace __GeneratedTest.Parsers.Rules
{
    using Sprache;
    
    public static class _rulelistParser
    {
        public static Parser<__GeneratedTest.CstNodes.Rules._rulelist> Instance { get; } = from _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in __GeneratedTest.Parsers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃParser.Instance.Many()
select new __GeneratedTest.CstNodes.Rules._rulelist(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1);
    }
    
}
