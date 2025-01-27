namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _notExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._notExpr> Instance { get; } = from _ʺx6Ex6Fx74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Ex6Fx74ʺParser.Instance
from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _boolCommonExpr_1 in __GeneratedOdata.Parsers.Rules._boolCommonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._notExpr(_ʺx6Ex6Fx74ʺ_1, _RWS_1, _boolCommonExpr_1);
    }
    
}
