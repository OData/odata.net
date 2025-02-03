namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParametersParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionExprParameters> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1 in __GeneratedOdataV2.Parsers.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionExprParameters(_OPEN_1, _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
