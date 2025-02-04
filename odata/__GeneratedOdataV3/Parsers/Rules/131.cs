namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePathExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePathExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePathExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitivePathExpr> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitivePathExpr)!, input);
}

var _annotationExprⳆboundFunctionExpr_1 = __GeneratedOdataV3.Parsers.Inners._annotationExprⳆboundFunctionExprParser.Instance.Optional().Parse(_ʺx2Fʺ_1.Remainder);
if (!_annotationExprⳆboundFunctionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitivePathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitivePathExpr(_ʺx2Fʺ_1.Parsed, _annotationExprⳆboundFunctionExpr_1.Parsed.GetOrElse(null)), _annotationExprⳆboundFunctionExpr_1.Remainder);
            }
        }
    }
    
}
