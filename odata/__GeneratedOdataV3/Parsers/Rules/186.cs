namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _subExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._subExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._subExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._subExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._subExpr)!, input);
}

var _ʺx73x75x62ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx73x75x62ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx73x75x62ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._subExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx73x75x62ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._subExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV3.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._subExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._subExpr(_RWS_1.Parsed, _ʺx73x75x62ʺ_1.Parsed, _RWS_2.Parsed,  _commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
