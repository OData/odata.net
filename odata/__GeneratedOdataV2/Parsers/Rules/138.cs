namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lambdaPredicateExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._lambdaPredicateExpr> Instance { get; } = from _boolCommonExpr_1 in __GeneratedOdataV2.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._lambdaPredicateExpr(_boolCommonExpr_1);
    }
    
}
