namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _neExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._neExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx6Ex65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Ex65ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._neExpr(_RWS_1, _ʺx6Ex65ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
