namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lambdaPredicateExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr> Parse(IInput<char>? input)
            {
                var _boolCommonExpr_1 = __GeneratedOdataV4.Parsers.Rules._boolCommonExprParser.Instance.Parse(input);
if (!_boolCommonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._lambdaPredicateExpr(_boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
