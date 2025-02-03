namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_keyValuePairↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ> Instance { get; } = from _COMMA_keyValuePair_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_keyValuePairParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ(_COMMA_keyValuePair_1);
    }
    
}
