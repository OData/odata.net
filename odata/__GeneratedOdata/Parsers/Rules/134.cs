namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParametersParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._functionExprParameters> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1 in __GeneratedOdata.Parsers.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionExprParameters(_OPEN_1, _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
