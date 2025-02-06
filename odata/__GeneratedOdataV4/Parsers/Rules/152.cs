namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _monthMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr> Parse(IInput<char>? input)
            {
                var _ʺx6Dx6Fx6Ex74x68ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Dx6Fx6Ex74x68ʺParser.Instance.Parse(input);
if (!_ʺx6Dx6Fx6Ex74x68ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(_ʺx6Dx6Fx6Ex74x68ʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

var _BWS_1 = __GeneratedOdataV4.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

var _BWS_2 = __GeneratedOdataV4.Parsers.Rules._BWSParser.Instance.Parse(_commonExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._monthMethodCallExpr(_ʺx6Dx6Fx6Ex74x68ʺ_1.Parsed, _OPEN_1.Parsed, _BWS_1.Parsed, _commonExpr_1.Parsed, _BWS_2.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
