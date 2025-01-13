namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _hasSubsetMethodCallExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._hasSubsetMethodCallExpr> Instance { get; } = from _ʺx68x61x73x73x75x62x73x65x74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx68x61x73x73x75x62x73x65x74ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _BWS_3 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_2 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_4 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._hasSubsetMethodCallExpr(_ʺx68x61x73x73x75x62x73x65x74ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _COMMA_1, _BWS_3, _commonExpr_2, _BWS_4, _CLOSE_1);
    }
    
}
