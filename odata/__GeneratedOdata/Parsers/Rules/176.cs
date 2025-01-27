namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._orExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx6Fx72ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Fx72ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _boolCommonExpr_1 in __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._orExpr(_RWS_1, _ʺx6Fx72ʺ_1, _RWS_2, _boolCommonExpr_1);
    }
    
}
