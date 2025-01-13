namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _geExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._geExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx67x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx67x65ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._geExpr(_RWS_1, _ʺx67x65ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
