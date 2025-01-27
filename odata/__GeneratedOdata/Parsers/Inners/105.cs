namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_expandItemↃParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤCOMMA_expandItemↃ> Instance { get; } = from _COMMA_expandItem_1 in __GeneratedOdata.Parsers.Inners._COMMA_expandItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ⲤCOMMA_expandItemↃ(_COMMA_expandItem_1);
    }
    
}
