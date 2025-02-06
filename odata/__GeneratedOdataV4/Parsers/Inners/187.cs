namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _searchOrExprⳆsearchAndExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr> Instance { get; } = (_searchOrExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr>(_searchAndExprParser.Instance);
        
        public static class _searchOrExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr> Parse(IInput<char>? input)
                {
                    var _searchOrExpr_1 = __GeneratedOdataV4.Parsers.Rules._searchOrExprParser.Instance.Parse(input);
if (!_searchOrExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchOrExpr(_searchOrExpr_1.Parsed), _searchOrExpr_1.Remainder);
                }
            }
        }
        
        public static class _searchAndExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr> Parse(IInput<char>? input)
                {
                    var _searchAndExpr_1 = __GeneratedOdataV4.Parsers.Rules._searchAndExprParser.Instance.Parse(input);
if (!_searchAndExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._searchOrExprⳆsearchAndExpr._searchAndExpr(_searchAndExpr_1.Parsed), _searchAndExpr_1.Remainder);
                }
            }
        }
    }
    
}
