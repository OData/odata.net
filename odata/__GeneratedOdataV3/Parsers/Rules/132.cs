namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundFunctionExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr> Instance { get; } = from _functionExpr_1 in __GeneratedOdataV3.Parsers.Rules._functionExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr(_functionExpr_1);
    }
    
}
