namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchAndExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._searchAndExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx41x4Ex44ʺ_RWS_1 in __GeneratedOdata.Parsers.Inners._ʺx41x4Ex44ʺ_RWSParser.Instance.Optional()
from _searchExpr_1 in __GeneratedOdata.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._searchAndExpr(_RWS_1, _ʺx41x4Ex44ʺ_RWS_1.GetOrElse(null), _searchExpr_1);
    }
    
}
