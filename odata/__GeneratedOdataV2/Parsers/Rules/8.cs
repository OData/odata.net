namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _simpleKeyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._simpleKey> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _ⲤparameterAliasⳆkeyPropertyValueↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._simpleKey(_OPEN_1, _ⲤparameterAliasⳆkeyPropertyValueↃ_1, _CLOSE_1);
    }
    
}
