namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆkeyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue>(_keyPropertyValueParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdataV3.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._parameterAlias(_parameterAlias_1);
        }
        
        public static class _keyPropertyValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue> Instance { get; } = from _keyPropertyValue_1 in __GeneratedOdataV3.Parsers.Rules._keyPropertyValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._parameterAliasⳆkeyPropertyValue._keyPropertyValue(_keyPropertyValue_1);
        }
    }
    
}
