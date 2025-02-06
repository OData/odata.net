namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _propertyPathExprⳆboundFunctionExprⳆannotationExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr> Instance { get; } = (_propertyPathExprParser.Instance);
        
        public static class _propertyPathExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr> Parse(IInput<char>? input)
                {
                    var _propertyPathExpr_1 = __GeneratedOdataV4.Parsers.Rules._propertyPathExprParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr(_propertyPathExpr_1.Parsed), _propertyPathExpr_1.Remainder);
                }
            }
        }
        
        public static class _boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr> Parse(IInput<char>? input)
                {
                    var _boundFunctionExpr_1 = __GeneratedOdataV4.Parsers.Rules._boundFunctionExprParser.Instance.Parse(input);
if (!_boundFunctionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._boundFunctionExpr(_boundFunctionExpr_1.Parsed), _boundFunctionExpr_1.Remainder);
                }
            }
        }
        
        public static class _annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr> Parse(IInput<char>? input)
                {
                    var _annotationExpr_1 = __GeneratedOdataV4.Parsers.Rules._annotationExprParser.Instance.Parse(input);
if (!_annotationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._annotationExpr(_annotationExpr_1.Parsed), _annotationExpr_1.Remainder);
                }
            }
        }
    }
    
}
