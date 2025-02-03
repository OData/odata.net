namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_singleEnumValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_singleEnumValue> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _singleEnumValue_1 in __GeneratedOdataV2.Parsers.Rules._singleEnumValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_singleEnumValue(_COMMA_1, _singleEnumValue_1);
    }
    
}
