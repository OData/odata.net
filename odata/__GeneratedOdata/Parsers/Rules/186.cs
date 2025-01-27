namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _subExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._subExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx73x75x62ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx73x75x62ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._subExpr(_RWS_1, _ʺx73x75x62ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
