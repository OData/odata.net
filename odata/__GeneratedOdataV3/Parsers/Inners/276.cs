namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr> Instance { get; } = (_ʺx2Fʺ_propertyPathExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr>(_ʺx2Fʺ_boundFunctionExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr>(_ʺx2Fʺ_annotationExprParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_propertyPathExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
from _propertyPathExpr_1 in __GeneratedOdataV3.Parsers.Rules._propertyPathExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_propertyPathExpr(_ʺx2Fʺ_1, _propertyPathExpr_1);
        }
        
        public static class _ʺx2Fʺ_boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_boundFunctionExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
from _boundFunctionExpr_1 in __GeneratedOdataV3.Parsers.Rules._boundFunctionExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_boundFunctionExpr(_ʺx2Fʺ_1, _boundFunctionExpr_1);
        }
        
        public static class _ʺx2Fʺ_annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_annotationExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance
from _annotationExpr_1 in __GeneratedOdataV3.Parsers.Rules._annotationExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr._ʺx2Fʺ_annotationExpr(_ʺx2Fʺ_1, _annotationExpr_1);
        }
    }
    
}
