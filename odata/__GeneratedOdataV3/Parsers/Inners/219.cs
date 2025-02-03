namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_parameterNameↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ> Instance { get; } = from _COMMA_parameterName_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_parameterNameParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ(_COMMA_parameterName_1);
    }
    
}
