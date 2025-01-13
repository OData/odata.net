namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _searchOrExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._searchOrExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx4Fx52ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Fx52ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _searchExpr_1 in __GeneratedOdata.Parsers.Rules._searchExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._searchOrExpr(_RWS_1, _ʺx4Fx52ʺ_1, _RWS_2, _searchExpr_1);
    }
    
}
