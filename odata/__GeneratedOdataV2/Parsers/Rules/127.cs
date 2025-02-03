namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavigationExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleNavigationExpr> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _memberExpr_1 in __GeneratedOdataV2.Parsers.Rules._memberExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleNavigationExpr(_ʺx2Fʺ_1, _memberExpr_1);
    }
    
}
