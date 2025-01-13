namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _commonExpr_BWS_COMMA_BWSParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._commonExpr_BWS_COMMA_BWS> Instance { get; } = from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Inners._commonExpr_BWS_COMMA_BWS(_commonExpr_1, _BWS_1, _COMMA_1, _BWS_2);
    }
    
}
