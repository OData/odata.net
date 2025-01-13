namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _parameterAliasⳆprimitiveLiteralParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral> Instance { get; } = (_parameterAliasParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral>(_primitiveLiteralParser.Instance);
        
        public static class _parameterAliasParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias> Instance { get; } = from _parameterAlias_1 in __GeneratedOdata.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._parameterAlias(_parameterAlias_1);
        }
        
        public static class _primitiveLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral> Instance { get; } = from _primitiveLiteral_1 in __GeneratedOdata.Parsers.Rules._primitiveLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Inners._parameterAliasⳆprimitiveLiteral._primitiveLiteral(_primitiveLiteral_1);
        }
    }
    
}
