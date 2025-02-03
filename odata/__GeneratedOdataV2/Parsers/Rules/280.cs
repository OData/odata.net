namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _doubleValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._doubleValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdataV2.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._doubleValue(_decimalValue_1);
    }
    
}
