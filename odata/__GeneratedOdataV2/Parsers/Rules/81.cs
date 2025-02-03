namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._searchExpr> Instance { get; } = from _ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃParser.Instance
from _searchOrExprⳆsearchAndExpr_1 in __GeneratedOdataV2.Parsers.Inners._searchOrExprⳆsearchAndExprParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._searchExpr(_ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ_1, _searchOrExprⳆsearchAndExpr_1.GetOrElse(null));
    }
    
}
