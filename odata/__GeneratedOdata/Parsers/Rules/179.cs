namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ltExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._ltExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx6Cx74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Cx74ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ltExpr(_RWS_1, _ʺx6Cx74ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
