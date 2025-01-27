namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _aliasAndValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._aliasAndValue> Instance { get; } = from _parameterAlias_1 in __GeneratedOdata.Parsers.Rules._parameterAliasParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _parameterValue_1 in __GeneratedOdata.Parsers.Rules._parameterValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._aliasAndValue(_parameterAlias_1, _EQ_1, _parameterValue_1);
    }
    
}
