namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameterParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._functionExprParameter> Instance { get; } = from _parameterName_1 in __GeneratedOdata.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _ⲤparameterAliasⳆparameterValueↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤparameterAliasⳆparameterValueↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionExprParameter(_parameterName_1, _EQ_1, _ⲤparameterAliasⳆparameterValueↃ_1);
    }
    
}
