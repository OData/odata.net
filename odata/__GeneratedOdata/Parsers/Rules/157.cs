namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _fractionalsecondsMethodCallExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._fractionalsecondsMethodCallExpr> Instance { get; } = from _ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._fractionalsecondsMethodCallExpr(_ʺx66x72x61x63x74x69x6Fx6Ex61x6Cx73x65x63x6Fx6Ex64x73ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
