namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _subExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._subExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx73x75x62ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx73x75x62ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._subExpr(_RWS_1, _ʺx73x75x62ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
