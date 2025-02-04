namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minDateTimeMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx6Dx69x6Ex64x61x74x65x74x69x6Dx65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Dx69x6Ex64x61x74x65x74x69x6Dx65ʺParser.Instance.Parse(input);
if (!_ʺx6Dx69x6Ex64x61x74x65x74x69x6Dx65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx6Dx69x6Ex64x61x74x65x74x69x6Dx65ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._minDateTimeMethodCallExpr(_ʺx6Dx69x6Ex64x61x74x65x74x69x6Dx65ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
