namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._singleNavigationExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _memberExpr_1 in __GeneratedOdata.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._singleNavigationExpr(_ʺx2Fʺ_1, _memberExpr_1);
    }
    
}
