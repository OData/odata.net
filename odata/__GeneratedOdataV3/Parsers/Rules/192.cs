namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _notExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._notExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._notExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._notExpr> Parse(IInput<char>? input)
            {
                var _ʺx6Ex6Fx74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Ex6Fx74ʺParser.Instance.Parse(input);
if (!_ʺx6Ex6Fx74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._notExpr)!, input);
}

var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx6Ex6Fx74ʺ_1.Remainder);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._notExpr)!, input);
}

var _boolCommonExpr_1 = __GeneratedOdataV3.Parsers.Rules._boolCommonExprParser.Instance.Parse(_RWS_1.Remainder);
if (!_boolCommonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._notExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._notExpr(_ʺx6Ex6Fx74ʺ_1.Parsed, _RWS_1.Parsed,  _boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
