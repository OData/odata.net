namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionExprParameter> Instance { get; } = from _parameterName_1 in __GeneratedOdataV2.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤparameterAliasⳆparameterValueↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤparameterAliasⳆparameterValueↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionExprParameter(_parameterName_1, _EQ_1, _ⲤparameterAliasⳆparameterValueↃ_1);
    }
    
}
