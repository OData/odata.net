namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _parameterNamesParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._parameterNames> Instance { get; } = from _parameterName_1 in __GeneratedOdata.Parsers.Rules._parameterNameParser.Instance
from _ⲤCOMMA_parameterNameↃ_1 in Inners._ⲤCOMMA_parameterNameↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._parameterNames(_parameterName_1, _ⲤCOMMA_parameterNameↃ_1);
    }
    
}
