namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _keyPropertyValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._keyPropertyValue> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._keyPropertyValue(_primitiveLiteral_1);
    }
    
}
