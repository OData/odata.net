namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _boundFunctionExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionExpr> Instance { get; } = from _functionExpr_1 in __GeneratedOdata.Parsers.Rules._functionExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionExpr(_functionExpr_1);
    }
    
}
