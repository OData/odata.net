namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._collectionNavigationExpr> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1 in __GeneratedOdata.Parsers.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._collectionNavigationExpr(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr_1.GetOrElse(null));
    }
    
}
