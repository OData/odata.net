namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nameAndValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._nameAndValue> Instance { get; } = from _parameterName_1 in __GeneratedOdataV2.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _parameterValue_1 in __GeneratedOdataV2.Parsers.Rules._parameterValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._nameAndValue(_parameterName_1, _EQ_1, _parameterValue_1);
    }
    
}
