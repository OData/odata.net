namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_selectItemↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_selectItemↃ> Instance { get; } = from _COMMA_selectItem_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_selectItemParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_selectItemↃ(_COMMA_selectItem_1);
    }
    
}
