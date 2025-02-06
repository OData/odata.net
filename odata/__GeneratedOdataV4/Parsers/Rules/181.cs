namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _gtExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._gtExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._gtExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._gtExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._gtExpr)!, input);
}

var _ʺx67x74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx67x74ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx67x74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._gtExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(_ʺx67x74ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._gtExpr)!, input);
}

var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._gtExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._gtExpr(_RWS_1.Parsed, _ʺx67x74ʺ_1.Parsed, _RWS_2.Parsed, _commonExpr_1.Parsed), _commonExpr_1.Remainder);
            }
        }
    }
    
}
