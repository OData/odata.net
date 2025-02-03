namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._orExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx6Fx72ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Fx72ʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _boolCommonExpr_1 in __GeneratedOdataV2.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._orExpr(_RWS_1, _ʺx6Fx72ʺ_1, _RWS_2, _boolCommonExpr_1);
    }
    
}
