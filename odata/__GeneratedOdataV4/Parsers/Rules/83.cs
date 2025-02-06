namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchAndExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchAndExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchAndExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._searchAndExpr> Parse(IInput<char>? input)
            {
                var _RWS_1 = __GeneratedOdataV4.Parsers.Rules._RWSParser.Instance.Parse(input);
if (!_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchAndExpr)!, input);
}

var _ʺx41x4Ex44ʺ_RWS_1 = __GeneratedOdataV4.Parsers.Inners._ʺx41x4Ex44ʺ_RWSParser.Instance.Optional().Parse(_RWS_1.Remainder);
if (!_ʺx41x4Ex44ʺ_RWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchAndExpr)!, input);
}

var _searchExpr_1 = __GeneratedOdataV4.Parsers.Rules._searchExprParser.Instance.Parse(_ʺx41x4Ex44ʺ_RWS_1.Remainder);
if (!_searchExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchAndExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._searchAndExpr(_RWS_1.Parsed, _ʺx41x4Ex44ʺ_RWS_1.Parsed.GetOrElse(null), _searchExpr_1.Parsed), _searchExpr_1.Remainder);
            }
        }
    }
    
}
