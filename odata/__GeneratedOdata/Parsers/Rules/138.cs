namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lambdaPredicateExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._lambdaPredicateExpr> Instance { get; } = from _boolCommonExpr_1 in __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._lambdaPredicateExpr(_boolCommonExpr_1);
    }
    
}
