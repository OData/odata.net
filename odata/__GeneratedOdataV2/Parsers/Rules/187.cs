namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _mulExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._mulExpr> Instance { get; } = from _RWS_1 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _ʺx6Dx75x6Cʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Dx75x6CʺParser.Instance
from _RWS_2 in __GeneratedOdataV2.Parsers.Rules._RWSParser.Instance
from _commonExpr_1 in __GeneratedOdataV2.Parsers.Rules._commonExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._mulExpr(_RWS_1, _ʺx6Dx75x6Cʺ_1, _RWS_2, _commonExpr_1);
    }
    
}
