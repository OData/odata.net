namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_memberExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_memberExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _memberExpr_1 in __GeneratedOdataV2.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_memberExpr(_ʺx2Fʺ_1, _memberExpr_1);
    }
    
}
