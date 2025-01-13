namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _divExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._divExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx64x69x76ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx64x69x76ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._divExpr(_RWS_1, _ʺx64x69x76ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
