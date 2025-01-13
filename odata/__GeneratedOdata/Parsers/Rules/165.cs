namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _roundMethodCallExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._roundMethodCallExpr> Instance { get; } = from _ʺx72x6Fx75x6Ex64ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx72x6Fx75x6Ex64ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._roundMethodCallExpr(_ʺx72x6Fx75x6Ex64ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
