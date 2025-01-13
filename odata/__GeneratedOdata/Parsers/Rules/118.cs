namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _memberExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._memberExpr> Instance { get; } = from _qualifiedEntityTypeName_ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional()
from _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._memberExpr(_qualifiedEntityTypeName_ʺx2Fʺ_1.GetOrElse(null), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1);
    }
    
}
