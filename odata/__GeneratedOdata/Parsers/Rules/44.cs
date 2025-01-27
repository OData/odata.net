namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParametersParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._functionParameters> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _functionParameter_ЖⲤCOMMA_functionParameterↃ_1 in __GeneratedOdata.Parsers.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionParameters(_OPEN_1, _functionParameter_ЖⲤCOMMA_functionParameterↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
