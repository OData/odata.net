namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _divbyExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._divbyExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx64x69x76x62x79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx64x69x76x62x79ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._divbyExpr(_RWS_1, _ʺx64x69x76x62x79ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
