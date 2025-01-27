namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ> Instance { get; } = from _functionExprParameter_1 in __GeneratedOdata.Parsers.Rules._functionExprParameterParser.Instance
from _ⲤCOMMA_functionExprParameterↃ_1 in Inners._ⲤCOMMA_functionExprParameterↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ(_functionExprParameter_1, _ⲤCOMMA_functionExprParameterↃ_1);
    }
    
}
