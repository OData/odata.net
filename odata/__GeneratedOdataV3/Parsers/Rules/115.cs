namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolCommonExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr(_commonExpr_1);
    }
    
}
