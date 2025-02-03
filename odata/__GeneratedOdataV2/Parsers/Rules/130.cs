namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexPathExpr> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._complexPathExpr(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1.GetOrElse(null));
    }
    
}
