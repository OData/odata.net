namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._orExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._orExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._orExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orExpr)!, input);
}

var _ʺx6Fx72ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Fx72ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx6Fx72ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx6Fx72ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orExpr)!, input);
}

var _boolCommonExpr_1 = __GeneratedOdataV3.Parsers.Rules._boolCommonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_boolCommonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._orExpr(_RWS_1.Parsed, _ʺx6Fx72ʺ_1.Parsed, _RWS_2.Parsed,  _boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
