namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _andExprⳆorExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr> Instance { get; } = (_andExprParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr>(_orExprParser.Instance);
        
        public static class _andExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr._andExpr> Instance { get; } = from _andExpr_1 in __GeneratedOdataV2.Parsers.Rules._andExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr._andExpr(_andExpr_1);
        }
        
        public static class _orExprParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr._orExpr> Instance { get; } = from _orExpr_1 in __GeneratedOdataV2.Parsers.Rules._orExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._andExprⳆorExpr._orExpr(_orExpr_1);
        }
    }
    
}
