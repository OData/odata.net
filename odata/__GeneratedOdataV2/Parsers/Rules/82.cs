namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchOrExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._searchOrExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx4Fx52ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Fx52ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _searchExpr_1 in __GeneratedOdataV2.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._searchOrExpr(_RWS_1, _ʺx4Fx52ʺ_1, _RWS_2, _searchExpr_1);
    }
    
}
