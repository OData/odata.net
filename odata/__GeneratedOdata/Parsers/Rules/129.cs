namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _collectionPathExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr> Instance { get; } = (_count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_boundFunctionExprParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_annotationExprParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_anyExprParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_allExprParser.Instance);
        
        public static class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Instance { get; } = from _count_1 in __GeneratedOdata.Parsers.Rules._countParser.Instance
from _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 in __GeneratedOdata.Parsers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(_count_1, _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.GetOrElse(null));
        }
        
        public static class _ʺx2Fʺ_boundFunctionExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _boundFunctionExpr_1 in __GeneratedOdata.Parsers.Rules._boundFunctionExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr(_ʺx2Fʺ_1, _boundFunctionExpr_1);
        }
        
        public static class _ʺx2Fʺ_annotationExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _annotationExpr_1 in __GeneratedOdata.Parsers.Rules._annotationExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr(_ʺx2Fʺ_1, _annotationExpr_1);
        }
        
        public static class _ʺx2Fʺ_anyExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _anyExpr_1 in __GeneratedOdata.Parsers.Rules._anyExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr(_ʺx2Fʺ_1, _anyExpr_1);
        }
        
        public static class _ʺx2Fʺ_allExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _allExpr_1 in __GeneratedOdata.Parsers.Rules._allExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr(_ʺx2Fʺ_1, _allExpr_1);
        }
    }
    
}
