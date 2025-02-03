namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_singleEnumValueↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ> Instance { get; } = from _COMMA_singleEnumValue_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_singleEnumValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ(_COMMA_singleEnumValue_1);
    }
    
}
