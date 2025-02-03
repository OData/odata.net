namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr> Instance { get; } = from _lambdaVariableExpr_1 in __GeneratedOdataV2.Parsers.Rules._lambdaVariableExprParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _COLON_1 in __GeneratedOdataV2.Parsers.Rules._COLONParser.Instance
from _BWS_2 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _lambdaPredicateExpr_1 in __GeneratedOdataV2.Parsers.Rules._lambdaPredicateExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._lambdaVariableExpr_BWS_COLON_BWS_lambdaPredicateExpr(_lambdaVariableExpr_1, _BWS_1, _COLON_1, _BWS_2, _lambdaPredicateExpr_1);
    }
    
}
