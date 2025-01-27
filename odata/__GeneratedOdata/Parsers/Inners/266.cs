namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _propertyPathExprⳆboundFunctionExprⳆannotationExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr> Instance { get; } = (_propertyPathExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr>(_boundFunctionExprParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr>(_annotationExprParser.Instance);
        
        public static class _propertyPathExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr> Instance { get; } = from _propertyPathExpr_1 in __GeneratedOdata.Parsers.Rules._propertyPathExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr(_propertyPathExpr_1);
        }
        
        public static class _boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr> Instance { get; } = from _boundFunctionExpr_1 in __GeneratedOdata.Parsers.Rules._boundFunctionExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr(_boundFunctionExpr_1);
        }
        
        public static class _annotationExprParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr> Instance { get; } = from _annotationExpr_1 in __GeneratedOdata.Parsers.Rules._annotationExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr(_annotationExpr_1);
        }
    }
    
}
