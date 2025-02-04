namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolCommonExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolCommonExpr(_commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
