namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _totalsecondsMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr> Instance { get; } = from _ʺx74x6Fx74x61x6Cx73x65x63x6Fx6Ex64x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx74x6Fx74x61x6Cx73x65x63x6Fx6Ex64x73ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._totalsecondsMethodCallExpr(_ʺx74x6Fx74x61x6Cx73x65x63x6Fx6Ex64x73ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
