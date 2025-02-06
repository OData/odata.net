namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _commonExprParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._commonExpr> Instance { get; } = from _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance
from _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1 in __GeneratedOdata.Parsers.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser.Instance.Optional()
from _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 in __GeneratedOdata.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance.Optional()
from _andExprⳆorExpr_1 in __GeneratedOdata.Parsers.Inners._andExprⳆorExprParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._commonExpr(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1, _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1.GetOrElse(null), _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.GetOrElse(null), _andExprⳆorExpr_1.GetOrElse(null));
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._commonExpr> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._commonExpr>
        {
            public IOutput<char, _commonExpr> Parse(IInput<char>? input)
            {
                var _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 = __GeneratedOdata.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance.Parse(input);
                ////var _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1 in __GeneratedOdata.Parsers.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser.Instance.Optional()
                var _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 = __GeneratedOdata.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance.Optional().Parse(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Remainder);
                ////from _andExprⳆorExpr_1 in __GeneratedOdata.Parsers.Inners._andExprⳆorExprParser.Instance.Optional()
                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._commonExpr(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Parsed, null, _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Parsed.GetOrElse(null), null),
                    _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Remainder);
            }
        }
    }
    
}
