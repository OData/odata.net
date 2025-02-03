namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitivePathExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _annotationExprⳆboundFunctionExpr_1 in __GeneratedOdataV2.Parsers.Inners._annotationExprⳆboundFunctionExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._primitivePathExpr(_ʺx2Fʺ_1, _annotationExprⳆboundFunctionExpr_1.GetOrElse(null));
    }
    
}
