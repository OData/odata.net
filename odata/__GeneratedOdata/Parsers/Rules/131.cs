namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitivePathExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitivePathExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _annotationExprⳆboundFunctionExpr_1 in __GeneratedOdata.Parsers.Inners._annotationExprⳆboundFunctionExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._primitivePathExpr(_ʺx2Fʺ_1, _annotationExprⳆboundFunctionExpr_1.GetOrElse(null));
    }
    
}
