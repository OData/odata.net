namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _commonExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._commonExpr> Instance { get; } = from _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance
from _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1 in __GeneratedOdata.Parsers.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser.Instance.Optional()
from _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 in __GeneratedOdata.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance.Optional()
from _andExprⳆorExpr_1 in __GeneratedOdata.Parsers.Inners._andExprⳆorExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._commonExpr(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1, _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1.GetOrElse(null), _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.GetOrElse(null), _andExprⳆorExpr_1.GetOrElse(null));
    }
    
}