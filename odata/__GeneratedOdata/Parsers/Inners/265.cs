namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_memberExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_memberExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _memberExpr_1 in __GeneratedOdata.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_memberExpr(_ʺx2Fʺ_1, _memberExpr_1);
    }
    
}
