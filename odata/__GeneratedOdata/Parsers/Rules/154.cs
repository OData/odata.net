namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hourMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._hourMethodCallExpr> Instance { get; } = from _ʺx68x6Fx75x72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx68x6Fx75x72ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._hourMethodCallExpr(_ʺx68x6Fx75x72ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
