namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm> Instance { get; } = (_OPEN_BWS_searchExpr_BWS_CLOSEParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm>(_searchTermParser.Instance);
        
        public static class _OPEN_BWS_searchExpr_BWS_CLOSEParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _searchExpr_1 in __GeneratedOdata.Parsers.Rules._searchExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._OPEN_BWS_searchExpr_BWS_CLOSE(_OPEN_1, _BWS_1, _searchExpr_1, _BWS_2, _CLOSE_1);
        }
        
        public static class _searchTermParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm> Instance { get; } = from _searchTerm_1 in __GeneratedOdata.Parsers.Rules._searchTermParser.Instance
select new __GeneratedOdata.CstNodes.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm._searchTerm(_searchTerm_1);
        }
    }
    
}
