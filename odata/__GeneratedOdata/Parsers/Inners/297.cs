namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_BWS_commonExpr_BWSParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_BWS_commonExpr_BWS> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_BWS_commonExpr_BWS(_COMMA_1, _BWS_1, _commonExpr_1, _BWS_2);
    }
    
}
