namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveKeyPropertyⳆkeyPropertyAliasParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias> Instance { get; } = (_primitiveKeyPropertyParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias>(_keyPropertyAliasParser.Instance);
        
        public static class _primitiveKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty> Instance { get; } = from _primitiveKeyProperty_1 in __GeneratedOdataV2.Parsers.Rules._primitiveKeyPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty(_primitiveKeyProperty_1);
        }
        
        public static class _keyPropertyAliasParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias> Instance { get; } = from _keyPropertyAlias_1 in __GeneratedOdataV2.Parsers.Rules._keyPropertyAliasParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias(_keyPropertyAlias_1);
        }
    }
    
}
