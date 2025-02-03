namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _startsWithMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx73x74x61x72x74x73x77x69x74x68ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx73x74x61x72x74x73x77x69x74x68ʺParser.Instance.Parse(input);
if (!_ʺx73x74x61x72x74x73x77x69x74x68ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx73x74x61x72x74x73x77x69x74x68ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(_BWS_2.Remainder);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _BWS_3 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COMMA_1.Remainder);
if (!_BWS_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _commonExpr_2 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_3.Remainder);
if (!_commonExpr_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _BWS_4 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_2.Remainder);
if (!_BWS_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_4.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._startsWithMethodCallExpr(_ʺx73x74x61x72x74x73x77x69x74x68ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed, _COMMA_1.Parsed, _BWS_3.Parsed, _commonExpr_2.Parsed, _BWS_4.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
