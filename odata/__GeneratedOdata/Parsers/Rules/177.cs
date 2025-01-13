namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _eqExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._eqExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx65x71ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx65x71ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._eqExpr(_RWS_1, _ʺx65x71ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
