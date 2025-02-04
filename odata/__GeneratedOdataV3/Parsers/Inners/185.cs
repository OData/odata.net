namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm> Instance { get; } = (_OPEN_BWS_searchExpr_BWS_CLOSEParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm>(_searchTermParser.Instance);
        
        public static class _OPEN_BWS_searchExpr_BWS_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE> Parse(IInput<char>? input)
                {
                    var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE)!, input);
}

var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_OPEN_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE)!, input);
}

var _searchExpr_1 = __GeneratedOdataV3.Parsers.Rules._searchExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_searchExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_searchExpr_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_BWS_2.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE(_OPEN_1.Parsed, _BWS_1.Parsed, _searchExpr_1.Parsed, _BWS_2.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _searchTermParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm> Parse(IInput<char>? input)
                {
                    var _searchTerm_1 = __GeneratedOdataV3.Parsers.Rules._searchTermParser.Instance.Parse(input);
if (!_searchTerm_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm(_searchTerm_1.Parsed), _searchTerm_1.Remainder);
                }
            }
        }
    }
    
}
