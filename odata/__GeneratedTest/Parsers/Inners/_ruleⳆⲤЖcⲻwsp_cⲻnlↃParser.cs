namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _ruleⳆⲤЖcⲻwsp_cⲻnlↃParser
    {
        public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ> Instance { get; } = (_ruleParser.Instance).Or<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ>(_ⲤЖcⲻwsp_cⲻnlↃParser.Instance);
        
        public static class _ruleParser
        {
            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule> Instance { get; } = from _rule_1 in __GeneratedTest.Parsers.Rules._ruleParser.Instance
select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule(_rule_1);
        }
        
        public static class _ⲤЖcⲻwsp_cⲻnlↃParser
        {
            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ> Instance { get; } = from _ⲤЖcⲻwsp_cⲻnlↃ_1 in __GeneratedTest.Parsers.Inners._ⲤЖcⲻwsp_cⲻnlↃParser.Instance
select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ(_ⲤЖcⲻwsp_cⲻnlↃ_1);
        }
    }
    
}
