namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆparameterValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue>(_parameterValueParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdata.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue._parameterAlias(_parameterAlias_1);
        }
        
        public static class _parameterValueParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue> Instance { get; } = from _parameterValue_1 in __GeneratedOdata.Parsers.Rules._parameterValueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆparameterValue._parameterValue(_parameterValue_1);
        }
    }
    
}
