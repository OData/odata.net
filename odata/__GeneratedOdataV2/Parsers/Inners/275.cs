namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr> Instance { get; } = (_keyPredicate_꘡singleNavigationExpr꘡Parser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr>(_collectionPathExprParser.Instance);
        
        public static class _keyPredicate_꘡singleNavigationExpr꘡Parser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡> Instance { get; } = from _keyPredicate_1 in __GeneratedOdataV2.Parsers.Rules._keyPredicateParser.Instance
from _singleNavigationExpr_1 in __GeneratedOdataV2.Parsers.Rules._singleNavigationExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._keyPredicate_꘡singleNavigationExpr꘡(_keyPredicate_1, _singleNavigationExpr_1.GetOrElse(null));
        }
        
        public static class _collectionPathExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr> Instance { get; } = from _collectionPathExpr_1 in __GeneratedOdataV2.Parsers.Rules._collectionPathExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._keyPredicate_꘡singleNavigationExpr꘡ⳆcollectionPathExpr._collectionPathExpr(_collectionPathExpr_1);
        }
    }
    
}
