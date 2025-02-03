namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _divExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._divExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._divExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._divExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._divExpr)!, input);
}

var _ʺx64x69x76ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx64x69x76ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx64x69x76ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._divExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx64x69x76ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._divExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._divExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._divExpr(_RWS_1.Parsed, _ʺx64x69x76ʺ_1.Parsed, _RWS_2.Parsed,  _commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
