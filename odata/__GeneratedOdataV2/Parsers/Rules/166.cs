namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _floorMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._floorMethodCallExpr> Instance { get; } = from _ʺx66x6Cx6Fx6Fx72ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx66x6Cx6Fx6Fx72ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._floorMethodCallExpr(_ʺx66x6Cx6Fx6Fx72ʺ_1, _OPEN_1, _BWS_1, _commonExpr_1, _BWS_2, _CLOSE_1);
    }
    
}
