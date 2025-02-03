namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _negateExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._negateExpr> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._negateExpr(_ʺx2Dʺ_1, _BWS_1, _commonExpr_1);
    }
    
}
