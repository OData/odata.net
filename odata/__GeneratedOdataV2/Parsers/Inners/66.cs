namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _parameterAliasⳆprimitiveLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral> Instance { get; } = (_parameterAliasParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral>(_primitiveLiteralParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdataV2.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias(_parameterAlias_1);
        }
        
        public static class _primitiveLiteralParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdataV2.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral(_primitiveLiteral_1);
        }
    }
    
}
