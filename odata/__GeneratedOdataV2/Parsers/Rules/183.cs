namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _inExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._inExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx69x6Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx69x6EʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._inExpr(_RWS_1, _ʺx69x6Eʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
