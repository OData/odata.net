namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_expandItemↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ> Instance { get; } = from _COMMA_expandItem_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_expandItemParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_expandItemↃ(_COMMA_expandItem_1);
    }
    
}
