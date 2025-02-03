namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_singleEnumValueↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ> Instance { get; } = from _COMMA_singleEnumValue_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_singleEnumValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ(_COMMA_singleEnumValue_1);
    }
    
}
