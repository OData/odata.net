namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _searchExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._searchExpr> Instance { get; } = from _ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃParser.Instance
from _searchOrExprⳆsearchAndExpr_1 in __GeneratedOdata.Parsers.Inners._searchOrExprⳆsearchAndExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._searchExpr(_ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1, _searchOrExprⳆsearchAndExpr_1.GetOrElse(null));
    }
    
}
