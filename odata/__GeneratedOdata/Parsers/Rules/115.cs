namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _boolCommonExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._boolCommonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolCommonExpr(_commonExpr_1);
    }
    
}
