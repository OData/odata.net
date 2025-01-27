namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _OWSParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._OWS> Instance { get; } = from _ⲤSPⳆHTABↃ_1 in Inners._ⲤSPⳆHTABↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._OWS(_ⲤSPⳆHTABↃ_1);
    }
    
}
