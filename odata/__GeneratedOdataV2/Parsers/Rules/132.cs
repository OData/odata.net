namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundFunctionExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boundFunctionExpr> Instance { get; } = from _functionExpr_1 in __GeneratedOdataV2.Parsers.Rules._functionExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boundFunctionExpr(_functionExpr_1);
    }
    
}
