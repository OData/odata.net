namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _timeMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._timeMethodCallExpr> Instance { get; } = from _ʺx74x69x6Dx65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx74x69x6Dx65ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._timeMethodCallExpr(_ʺx74x69x6Dx65ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
