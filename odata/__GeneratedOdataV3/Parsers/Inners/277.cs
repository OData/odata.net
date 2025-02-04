namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _annotationExprⳆboundFunctionExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr> Instance { get; } = (_annotationExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr>(_boundFunctionExprParser.Instance);
        
        public static class _annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr> Parse(IInput<char>? input)
                {
                    var _annotationExpr_1 = __GeneratedOdataV3.Parsers.Rules._annotationExprParser.Instance.Parse(input);
if (!_annotationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._annotationExpr(_annotationExpr_1.Parsed), _annotationExpr_1.Remainder);
                }
            }
        }
        
        public static class _boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr> Parse(IInput<char>? input)
                {
                    var _boundFunctionExpr_1 = __GeneratedOdataV3.Parsers.Rules._boundFunctionExprParser.Instance.Parse(input);
if (!_boundFunctionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._annotationExprⳆboundFunctionExpr._boundFunctionExpr(_boundFunctionExpr_1.Parsed), _boundFunctionExpr_1.Remainder);
                }
            }
        }
    }
    
}
