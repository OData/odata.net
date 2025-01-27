namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _leExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._leExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx6Cx65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Cx65ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._leExpr(_RWS_1, _ʺx6Cx65ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
