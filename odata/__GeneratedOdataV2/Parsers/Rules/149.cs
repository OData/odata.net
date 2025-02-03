namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _toUpperMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._toUpperMethodCallExpr> Instance { get; } = from _ʺx74x6Fx75x70x70x65x72ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx74x6Fx75x70x70x65x72ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._toUpperMethodCallExpr(_ʺx74x6Fx75x70x70x65x72ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
