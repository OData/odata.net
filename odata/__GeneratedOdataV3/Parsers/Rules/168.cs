namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _distanceMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺParser.Instance.Parse(input);
if (!_ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(_BWS_2.Remainder);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _BWS_3 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COMMA_1.Remainder);
if (!_BWS_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _commonExpr_2 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_3.Remainder);
if (!_commonExpr_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _BWS_4 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_2.Remainder);
if (!_BWS_4.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_4.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._distanceMethodCallExpr(_ʺx67x65x6Fx2Ex64x69x73x74x61x6Ex63x65ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed, _COMMA_1.Parsed, _BWS_3.Parsed, _commonExpr_2.Parsed, _BWS_4.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
