namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParameterParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._functionParameter> Instance { get; } = from _parameterName_1 in __GeneratedOdata.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _ⲤparameterAliasⳆprimitiveLiteralↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤparameterAliasⳆprimitiveLiteralↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._functionParameter(_parameterName_1, _EQ_1, _ⲤparameterAliasⳆprimitiveLiteralↃ_1);
    }
    
}
