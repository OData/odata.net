namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._keyPropertyValue> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdataV3.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._keyPropertyValue(_primitiveLiteral_1);
    }
    
}
