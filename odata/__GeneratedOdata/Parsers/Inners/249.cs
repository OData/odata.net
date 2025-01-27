namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_selectListItemↃParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ⲤCOMMA_selectListItemↃ> Instance { get; } = from _COMMA_selectListItem_1 in __GeneratedOdata.Parsers.Inners._COMMA_selectListItemParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ⲤCOMMA_selectListItemↃ(_COMMA_selectListItem_1);
    }
    
}
