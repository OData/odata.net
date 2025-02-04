namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _totalOffsetMinutesMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺParser.Instance.Parse(input);
if (!_ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._totalOffsetMinutesMethodCallExpr(_ʺx74x6Fx74x61x6Cx6Fx66x66x73x65x74x6Dx69x6Ex75x74x65x73ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
