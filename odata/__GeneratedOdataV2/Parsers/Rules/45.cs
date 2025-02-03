namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParameterParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._functionParameter> Instance { get; } = from _parameterName_1 in __GeneratedOdataV2.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤparameterAliasⳆprimitiveLiteralↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤparameterAliasⳆprimitiveLiteralↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._functionParameter(_parameterName_1, _EQ_1, _ⲤparameterAliasⳆprimitiveLiteralↃ_1);
    }
    
}
