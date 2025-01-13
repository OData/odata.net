namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _mulExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._mulExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx6Dx75x6Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Dx75x6CʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._mulExpr(_RWS_1, _ʺx6Dx75x6Cʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
