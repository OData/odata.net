namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._enumValue> Instance { get; } = from _singleEnumValue_1 in __GeneratedOdataV2.Parsers.Rules._singleEnumValueParser.Instance
from _ⲤCOMMA_singleEnumValueↃ_1 in Inners._ⲤCOMMA_singleEnumValueↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._enumValue(_singleEnumValue_1, _ⲤCOMMA_singleEnumValueↃ_1);
    }
    
}
