namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterNamesParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._parameterNames> Instance { get; } = from _parameterName_1 in __GeneratedOdataV2.Parsers.Rules._parameterNameParser.Instance
from _ⲤCOMMA_parameterNameↃ_1 in Inners._ⲤCOMMA_parameterNameↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._parameterNames(_parameterName_1, _ⲤCOMMA_parameterNameↃ_1);
    }
    
}
