namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _complexPathExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._complexPathExpr> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._complexPathExpr(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathExprⳆʺx2Fʺ_boundFunctionExprⳆʺx2Fʺ_annotationExpr_1.GetOrElse(null));
    }
    
}
