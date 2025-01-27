namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _gtExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._gtExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx67x74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx67x74ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdata.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._gtExpr(_RWS_1, _ʺx67x74ʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
