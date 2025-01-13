namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _IRIⲻinⲻheaderParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._IRIⲻinⲻheader> Instance { get; } = from _ⲤVCHARⳆobsⲻtextↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤVCHARⳆobsⲻtextↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._IRIⲻinⲻheader(_ⲤVCHARⳆobsⲻtextↃ_1);
    }
    
}
