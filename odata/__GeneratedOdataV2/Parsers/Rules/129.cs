namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionPathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr> Instance { get; } = (_count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_boundFunctionExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_annotationExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_anyExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_allExprParser.Instance);
        
        public static class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Instance { get; } = from _count_1 in __GeneratedOdataV2.Parsers.Rules._countParser.Instance
from _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 in __GeneratedOdataV2.Parsers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(_count_1, _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.GetOrElse(null));
        }
        
        public static class _ʺx2Fʺ_boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _boundFunctionExpr_1 in __GeneratedOdataV2.Parsers.Rules._boundFunctionExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr(_ʺx2Fʺ_1, _boundFunctionExpr_1);
        }
        
        public static class _ʺx2Fʺ_annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _annotationExpr_1 in __GeneratedOdataV2.Parsers.Rules._annotationExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr(_ʺx2Fʺ_1, _annotationExpr_1);
        }
        
        public static class _ʺx2Fʺ_anyExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _anyExpr_1 in __GeneratedOdataV2.Parsers.Rules._anyExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr(_ʺx2Fʺ_1, _anyExpr_1);
        }
        
        public static class _ʺx2Fʺ_allExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _allExpr_1 in __GeneratedOdataV2.Parsers.Rules._allExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr(_ʺx2Fʺ_1, _allExpr_1);
        }
    }
    
}
