namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minuteMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._minuteMethodCallExpr> Instance { get; } = from _ʺx6Dx69x6Ex75x74x65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Dx69x6Ex75x74x65ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._minuteMethodCallExpr(_ʺx6Dx69x6Ex75x74x65ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
