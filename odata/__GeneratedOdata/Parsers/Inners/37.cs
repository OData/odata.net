namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _parameterAliasⳆkeyPropertyValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue> Instance { get; } = (_parameterAliasParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue>(_keyPropertyValueParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdata.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias(_parameterAlias_1);
        }
        
        public static class _keyPropertyValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue> Instance { get; } = from _keyPropertyValue_1 in __GeneratedOdata.Parsers.Rules._keyPropertyValueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue(_keyPropertyValue_1);
        }
    }
    
}
