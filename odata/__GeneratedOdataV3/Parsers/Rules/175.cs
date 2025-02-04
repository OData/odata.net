namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _andExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._andExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._andExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._andExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._andExpr)!, input);
}

var _ʺx61x6Ex64ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx61x6Ex64ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx61x6Ex64ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._andExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx61x6Ex64ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._andExpr)!, input);
}

var _boolCommonExpr_1 = __GeneratedOdataV3.Parsers.Rules._boolCommonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_boolCommonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._andExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._andExpr(_RWS_1.Parsed, _ʺx61x6Ex64ʺ_1.Parsed, _RWS_2.Parsed, _boolCommonExpr_1.Parsed), _boolCommonExpr_1.Remainder);
            }
        }
    }
    
}
