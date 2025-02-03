namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _memberExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._memberExpr> Instance { get; } = from _qualifiedEntityTypeName_ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._qualifiedEntityTypeName_ʺx2FʺParser.Instance.Optional()
from _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._memberExpr(_qualifiedEntityTypeName_ʺx2Fʺ_1.GetOrElse(null), _ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ_1);
    }
    
}
