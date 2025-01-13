namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _addExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._addExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx61x64x64ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x64x64ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._addExpr(_RWS_1, _ʺx61x64x64ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
