namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyValuePairParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._keyValuePair> Instance { get; } = from _ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _ⲤparameterAliasⳆkeyPropertyValueↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._keyValuePair(_ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1, _EQ_1, _ⲤparameterAliasⳆkeyPropertyValueↃ_1);
    }
    
}
