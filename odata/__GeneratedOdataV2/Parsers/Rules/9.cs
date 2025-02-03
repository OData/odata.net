namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _compoundKeyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._compoundKey> Instance { get; } = from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _keyValuePair_1 in __GeneratedOdataV2.Parsers.Rules._keyValuePairParser.Instance
from _ⲤCOMMA_keyValuePairↃ_1 in Inners._ⲤCOMMA_keyValuePairↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._compoundKey(_OPEN_1, _keyValuePair_1, _ⲤCOMMA_keyValuePairↃ_1, _CLOSE_1);
    }
    
}
