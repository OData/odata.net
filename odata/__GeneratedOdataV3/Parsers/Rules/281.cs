namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdataV3.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._singleValue(_decimalValue_1);
    }
    
}
