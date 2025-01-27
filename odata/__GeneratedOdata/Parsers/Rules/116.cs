namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rootExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._rootExpr> Instance { get; } = from _ʺx24x72x6Fx6Fx74x2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x72x6Fx6Fx74x2FʺParser.Instance
from _ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤentitySetName_keyPredicateⳆsingletonEntityↃParser.Instance
from _singleNavigationExpr_1 in __GeneratedOdata.Parsers.Rules._singleNavigationExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._rootExpr(_ʺx24x72x6Fx6Fx74x2Fʺ_1, _ⲤentitySetName_keyPredicateⳆsingletonEntityↃ_1, _singleNavigationExpr_1.GetOrElse(null));
    }
    
}
