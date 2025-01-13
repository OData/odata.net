namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _listExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._listExpr> Instance { get; } = from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _ⲤCOMMA_BWS_commonExpr_BWSↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃParser.Instance.Many()
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._listExpr(_OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _ⲤCOMMA_BWS_commonExpr_BWSↃ_1, _CLOSE_1);
    }
    
}
