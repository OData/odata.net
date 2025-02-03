namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _OWSParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._OWS> Instance { get; } = from _ⲤSPⳆHTABↃ_1 in Inners._ⲤSPⳆHTABↃParser.Instance.Many()
select new __GeneratedOdataV3.CstNodes.Rules._OWS(_ⲤSPⳆHTABↃ_1);
    }
    
}
