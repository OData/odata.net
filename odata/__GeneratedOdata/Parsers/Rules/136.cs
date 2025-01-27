namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _anyExprParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._anyExpr> Instance { get; } = from _ʺx61x6Ex79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x6Ex79ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1 in __GeneratedOdata.Parsers.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprParser.Instance.Optional()
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._anyExpr(_ʺx61x6Ex79ʺ_1, _OPEN_1, _BWS_1, _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr_1.GetOrElse(null), _BWS_2, _CLOSE_1);
    }
    
}
