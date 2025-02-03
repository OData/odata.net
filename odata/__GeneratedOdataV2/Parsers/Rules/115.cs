namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolCommonExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._boolCommonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._boolCommonExpr(_commonExpr_1);
    }
    
}
