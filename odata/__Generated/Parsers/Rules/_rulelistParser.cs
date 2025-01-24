namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rulelistParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._rulelist> Instance { get; } = from _ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1 in __Generated.Parsers.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃParser.Instance.Repeat(1, null)
select new __Generated.CstNodes.Rules._rulelist(new __Generated.CstNodes.Inners.HelperRangedAtLeast1<__Generated.CstNodes.Inners._ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ>(_ⲤruleⳆⲤЖcⲻwsp_cⲻnlↃↃ_1));
    }
    
}
