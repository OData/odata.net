namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _searchOrExprⳆsearchAndExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr> Instance { get; } = (_searchOrExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr>(_searchAndExprParser.Instance);
        
        public static class _searchOrExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr> Instance { get; } = from _searchOrExpr_1 in __GeneratedOdataV3.Parsers.Rules._searchOrExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr(_searchOrExpr_1);
        }
        
        public static class _searchAndExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr> Instance { get; } = from _searchAndExpr_1 in __GeneratedOdataV3.Parsers.Rules._searchAndExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr(_searchAndExpr_1);
        }
    }
    
}
