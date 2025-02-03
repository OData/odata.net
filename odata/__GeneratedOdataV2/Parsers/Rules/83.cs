namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchAndExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._searchAndExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx41x4Ex44ʺ_RWS_1 in __GeneratedOdataV2.Parsers.Inners._ʺx41x4Ex44ʺ_RWSParser.Instance.Optional()
from _searchExpr_1 in __GeneratedOdataV2.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._searchAndExpr(_RWS_1, _ʺx41x4Ex44ʺ_RWS_1.GetOrElse(null), _searchExpr_1);
    }
    
}
