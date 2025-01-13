namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _dateMethodCallExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._dateMethodCallExpr> Instance { get; } = from _ʺx64x61x74x65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx64x61x74x65ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._dateMethodCallExpr(_ʺx64x61x74x65ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
