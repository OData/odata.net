namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr> Instance { get; } = (_collectionPathExprParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_singleNavigationExprParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_complexPathExprParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_primitivePathExprParser.Instance);
        
        public static class _collectionPathExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr> Instance { get; } = from _collectionPathExpr_1 in __GeneratedOdata.Parsers.Rules._collectionPathExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr(_collectionPathExpr_1);
        }
        
        public static class _singleNavigationExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr> Instance { get; } = from _singleNavigationExpr_1 in __GeneratedOdata.Parsers.Rules._singleNavigationExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr(_singleNavigationExpr_1);
        }
        
        public static class _complexPathExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr> Instance { get; } = from _complexPathExpr_1 in __GeneratedOdata.Parsers.Rules._complexPathExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr(_complexPathExpr_1);
        }
        
        public static class _primitivePathExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr> Instance { get; } = from _primitivePathExpr_1 in __GeneratedOdata.Parsers.Rules._primitivePathExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr(_primitivePathExpr_1);
        }
    }
    
}
