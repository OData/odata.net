namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr> Instance { get; } = (_endsWithMethodCallExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr>(_startsWithMethodCallExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr>(_containsMethodCallExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr>(_intersectsMethodCallExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr>(_hasSubsetMethodCallExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr>(_hasSubsequenceMethodCallExprParser.Instance);
        
        public static class _endsWithMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr> Instance { get; } = from _endsWithMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._endsWithMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr(_endsWithMethodCallExpr_1);
        }
        
        public static class _startsWithMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr> Instance { get; } = from _startsWithMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._startsWithMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr(_startsWithMethodCallExpr_1);
        }
        
        public static class _containsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr> Instance { get; } = from _containsMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._containsMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr(_containsMethodCallExpr_1);
        }
        
        public static class _intersectsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr> Instance { get; } = from _intersectsMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._intersectsMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr(_intersectsMethodCallExpr_1);
        }
        
        public static class _hasSubsetMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr> Instance { get; } = from _hasSubsetMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._hasSubsetMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr(_hasSubsetMethodCallExpr_1);
        }
        
        public static class _hasSubsequenceMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr> Instance { get; } = from _hasSubsequenceMethodCallExpr_1 in __GeneratedOdata.Parsers.Rules._hasSubsequenceMethodCallExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr(_hasSubsequenceMethodCallExpr_1);
        }
    }
    
}
