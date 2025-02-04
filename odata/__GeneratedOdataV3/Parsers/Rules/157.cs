namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fractionalsecondsMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺParser.Instance.Parse(input);
if (!_ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._fractionalsecondsMethodCallExpr(_ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
