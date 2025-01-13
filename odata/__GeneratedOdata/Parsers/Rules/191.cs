namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _negateExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._negateExpr> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._negateExpr(_ʺx2Dʺ_1, _BWS_1, _commonExpr_1);
    }
    
}
