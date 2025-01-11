namespace __Generated.Parsers.Inners
{
    using Sprache;
    
    public static class _ruleⳆⲤЖcⲻwsp_cⲻnlↃParser
    {
        public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ> Instance { get; }
        
        public static class _ruleParser
        {
            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule> Instance { get; } = from _rule_1 in __Generated.Parsers.Rules._ruleParser.Instance
select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._rule(_rule_1);
        }
        
        public static class _ⲤЖcⲻwsp_cⲻnlↃParser
        {
            public static Parser<__Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ> Instance { get; } = from _ⲤЖcⲻwsp_cⲻnlↃ_1 in __Generated.Parsers.Inners._ⲤЖcⲻwsp_cⲻnlↃParser.Instance
select new __Generated.CstNodes.Inners._ruleⳆⲤЖcⲻwsp_cⲻnlↃ._ⲤЖcⲻwsp_cⲻnlↃ(_ⲤЖcⲻwsp_cⲻnlↃ_1);
        }
    }
    
}
