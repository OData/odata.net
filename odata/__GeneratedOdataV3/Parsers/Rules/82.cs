namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchOrExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchOrExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchOrExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._searchOrExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr)!, input);
}

var _ʺx4Fx52ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Fx52ʺParser.Instance.Parse(_RWS_1.Remainder);
if (!_ʺx4Fx52ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr)!, input);
}

var _RWS_2 = __GeneratedOdataV3.Parsers.Rules._RWSParser.Instance.Parse(_ʺx4Fx52ʺ_1.Remainder);
if (!_RWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr)!, input);
}

var _searchExpr_1 = __GeneratedOdataV3.Parsers.Rules._searchExprParser.Instance.Parse(_RWS_2.Remainder);
if (!_searchExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchOrExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._searchOrExpr(_RWS_1.Parsed, _ʺx4Fx52ʺ_1.Parsed, _RWS_2.Parsed, _searchExpr_1.Parsed), _searchExpr_1.Remainder);
            }
        }
    }
    
}
