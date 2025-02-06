namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _mulExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._mulExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._mulExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._mulExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._mulExpr)!, input);
}

var _ʺx6Dx75x6Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Dx75x6CʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx6Dx75x6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._mulExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(_ʺx6Dx75x6Cʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._mulExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._mulExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._mulExpr(_RWS_1.Parsed, _ʺx6Dx75x6Cʺ_1.Parsed, _RWS_2.Parsed, _commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
