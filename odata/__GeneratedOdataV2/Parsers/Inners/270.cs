namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr> Instance { get; } = (_collectionPathExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_singleNavigationExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_complexPathExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_primitivePathExprParser.Instance);
        
        public static class _collectionPathExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr> Instance { get; } = from _collectionPathExpr_1 in __GeneratedOdataV2.Parsers.Rules._collectionPathExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr(_collectionPathExpr_1);
        }
        
        public static class _singleNavigationExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr> Instance { get; } = from _singleNavigationExpr_1 in __GeneratedOdataV2.Parsers.Rules._singleNavigationExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr(_singleNavigationExpr_1);
        }
        
        public static class _complexPathExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr> Instance { get; } = from _complexPathExpr_1 in __GeneratedOdataV2.Parsers.Rules._complexPathExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr(_complexPathExpr_1);
        }
        
        public static class _primitivePathExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr> Instance { get; } = from _primitivePathExpr_1 in __GeneratedOdataV2.Parsers.Rules._primitivePathExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr(_primitivePathExpr_1);
        }
    }
    
}
