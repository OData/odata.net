namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ> Instance { get; } = from _functionExprParameter_1 in __GeneratedOdataV2.Parsers.Rules._functionExprParameterParser.Instance
from _ⲤCOMMA_functionExprParameterↃ_1 in Inners._ⲤCOMMA_functionExprParameterↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ(_functionExprParameter_1, _ⲤCOMMA_functionExprParameterↃ_1);
    }
    
}
