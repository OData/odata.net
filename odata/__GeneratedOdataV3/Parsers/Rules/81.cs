namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._searchExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._searchExpr> Parse(IInput<char>? input)
            {
                var _ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃParser.Instance.Parse(input);
if (!_ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchExpr)!, input);
}

var _searchOrExprⳆsearchAndExpr_1 = __GeneratedOdataV3.Parsers.Inners._searchOrExprⳆsearchAndExprParser.Instance.Optional().Parse(_ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1.Remainder);
if (!_searchOrExprⳆsearchAndExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._searchExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._searchExpr(_ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1.Parsed, _searchOrExprⳆsearchAndExpr_1.Parsed.GetOrElse(null)), _searchOrExprⳆsearchAndExpr_1.Remainder);
            }
        }
    }
    
}
