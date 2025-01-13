namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _allExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._allExpr> Instance { get; } = from _ʺx61x6Cx6Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x6Cx6CʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _lambdaVariableExpr_1 in __GeneratedOdata.Parsers.Rules._lambdaVariableExprParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COLON_1 in __GeneratedOdata.Parsers.Rules._COLONParser.Instance
from _BWS_3 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _lambdaPredicateExpr_1 in __GeneratedOdata.Parsers.Rules._lambdaPredicateExprParser.Instance
from _BWS_4 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._allExpr(_ʺx61x6Cx6Cʺ_1, _OPEN_1, _BWS_1, _lambdaVariableExpr_1, _BWS_2, _COLON_1, _BWS_3, _lambdaPredicateExpr_1, _BWS_4, _CLOSE_1);
    }
    
}
