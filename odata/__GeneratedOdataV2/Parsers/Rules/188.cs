namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _divExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._divExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx64x69x76ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx64x69x76ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._divExpr(_RWS_1, _ʺx64x69x76ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
