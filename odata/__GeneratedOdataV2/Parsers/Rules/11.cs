namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._keyPropertyValue> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._keyPropertyValue(_primitiveLiteral_1);
    }
    
}
