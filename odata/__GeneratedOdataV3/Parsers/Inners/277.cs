namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _annotationExprⳆboundFunctionExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr> Instance { get; } = (_annotationExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr>(_boundFunctionExprParser.Instance);
        
        public static class _annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr> Instance { get; } = from _annotationExpr_1 in __GeneratedOdataV3.Parsers.Rules._annotationExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr(_annotationExpr_1);
        }
        
        public static class _boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr> Instance { get; } = from _boundFunctionExpr_1 in __GeneratedOdataV3.Parsers.Rules._boundFunctionExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr(_boundFunctionExpr_1);
        }
    }
    
}
