namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _andExprⳆorExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._andExprⳆorExpr> Instance { get; } = (_andExprParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._andExprⳆorExpr>(_orExprParser.Instance);
        
        public static class _andExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._andExprⳆorExpr._andExpr> Instance { get; } = from _andExpr_1 in __GeneratedOdata.Parsers.Rules._andExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._andExprⳆorExpr._andExpr(_andExpr_1);
        }
        
        public static class _orExprParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._andExprⳆorExpr._orExpr> Instance { get; } = from _orExpr_1 in __GeneratedOdata.Parsers.Rules._orExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._andExprⳆorExpr._orExpr(_orExpr_1);
        }
    }
    
}
