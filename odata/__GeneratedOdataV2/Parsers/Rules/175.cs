namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _andExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._andExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx61x6Ex64ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx61x6Ex64ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _boolCommonExpr_1 in __GeneratedOdataV2.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._andExpr(_RWS_1, _ʺx61x6Ex64ʺ_1, _RWS_2, _boolCommonExpr_1);
    }
    
}
