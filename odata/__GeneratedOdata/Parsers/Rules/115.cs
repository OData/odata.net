namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _boolCommonExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolCommonExpr> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boolCommonExpr(_commonExpr_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._boolCommonExpr> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._boolCommonExpr>
        {
            public IOutput<char, _boolCommonExpr> Parse(IInput<char>? input)
            {
                var _commonExpr_1 = __GeneratedOdata.Parsers.Rules._commonExprParser.Instance.Parse(input);
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._boolCommonExpr(_commonExpr_1.Parsed),
                    _commonExpr_1.Remainder);
            }
        }
    }
    
}
