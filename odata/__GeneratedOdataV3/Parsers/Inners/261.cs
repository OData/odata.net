namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _andExprⳆorExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr> Instance { get; } = (_andExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr>(_orExprParser.Instance);
        
        public static class _andExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._andExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._andExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._andExpr> Parse(IInput<char>? input)
                {
                    var _andExpr_1 = __GeneratedOdataV3.Parsers.Rules._andExprParser.Instance.Parse(input);
if (!_andExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._andExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._andExpr(_andExpr_1.Parsed), _andExpr_1.Remainder);
                }
            }
        }
        
        public static class _orExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._orExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._orExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._orExpr> Parse(IInput<char>? input)
                {
                    var _orExpr_1 = __GeneratedOdataV3.Parsers.Rules._orExprParser.Instance.Parse(input);
if (!_orExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._orExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._andExprⳆorExpr._orExpr(_orExpr_1.Parsed), _orExpr_1.Remainder);
                }
            }
        }
    }
    
}
