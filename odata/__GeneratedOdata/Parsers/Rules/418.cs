namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IRIⲻinⲻheaderParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IRIⲻinⲻheader> Instance { get; } = from _ⲤVCHARⳆobsⲻtextↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤVCHARⳆobsⲻtextↃParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._IRIⲻinⲻheader(new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ>(_ⲤVCHARⳆobsⲻtextↃ_1));
    }
    
}
