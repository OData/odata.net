namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundFunctionExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr> Parse(IInput<char>? input)
            {
                var _functionExpr_1 = __GeneratedOdataV3.Parsers.Rules._functionExprParser.Instance.Parse(input);
if (!_functionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionExpr(_functionExpr_1.Parsed), _functionExpr_1.Remainder);
            }
        }
    }
    
}
