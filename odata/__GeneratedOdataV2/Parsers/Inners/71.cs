namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_entitySetNameↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ> Instance { get; } = from _COMMA_entitySetName_1 in __GeneratedOdataV2.Parsers.Inners._COMMA_entitySetNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ(_COMMA_entitySetName_1);
    }
    
}
