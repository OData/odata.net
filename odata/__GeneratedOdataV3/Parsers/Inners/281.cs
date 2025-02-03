namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_functionExprParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ> Instance { get; } = from _COMMA_functionExprParameter_1 in __GeneratedOdataV3.Parsers.Inners._COMMA_functionExprParameterParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ(_COMMA_functionExprParameter_1);
    }
    
}
