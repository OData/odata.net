namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_orderbyItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ> Instance { get; } = from _COMMA_orderbyItem_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_orderbyItemParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_orderbyItemↃ(_COMMA_orderbyItem_1);
    }
    
}
