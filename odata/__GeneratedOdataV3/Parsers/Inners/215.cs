namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSEMI_selectOptionↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_selectOptionↃ> Instance { get; } = from _SEMI_selectOption_1 in __GeneratedOdataV3.Parsers.Inners._SEMI_selectOptionParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤSEMI_selectOptionↃ(_SEMI_selectOption_1);
    }
    
}
