namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _BWSⲻhParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._BWSⲻh> Instance { get; } = from _ⲤSPⳆHTABↃ_1 in Inners._ⲤSPⳆHTABↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._BWSⲻh(_ⲤSPⳆHTABↃ_1);
    }
    
}
