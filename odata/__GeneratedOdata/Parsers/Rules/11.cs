namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._keyPropertyValue> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._keyPropertyValue(_primitiveLiteral_1);
    }
    
}
