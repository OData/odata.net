namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _rootExpr_ЖⲤvalueⲻseparator_rootExprↃParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ> Instance { get; } = from _rootExpr_1 in __GeneratedOdata.Parsers.Rules._rootExprParser.Instance
from _Ⲥvalueⲻseparator_rootExprↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥvalueⲻseparator_rootExprↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._rootExpr_ЖⲤvalueⲻseparator_rootExprↃ(_rootExpr_1, _Ⲥvalueⲻseparator_rootExprↃ_1);
    }
    
}
