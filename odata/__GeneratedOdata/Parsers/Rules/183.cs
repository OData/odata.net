namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _inExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._inExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx69x6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx69x6EʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._inExpr(_RWS_1, _ʺx69x6Eʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
