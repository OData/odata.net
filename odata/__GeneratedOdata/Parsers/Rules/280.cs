namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _doubleValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._doubleValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdata.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._doubleValue(_decimalValue_1);
    }
    
}
