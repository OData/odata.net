namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ> Instance { get; } = from _rootExpr_1 in __GeneratedOdataV2.Parsers.Rules._rootExprParser.Instance
from _Ⲥvalueⲻseparator_rootExprↃ_1 in Inners._Ⲥvalueⲻseparator_rootExprↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ(_rootExpr_1, _Ⲥvalueⲻseparator_rootExprↃ_1);
    }
    
}
