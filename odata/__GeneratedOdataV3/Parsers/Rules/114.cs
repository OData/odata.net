namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _commonExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._commonExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._commonExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._commonExpr> Parse(IInput<char>? input)
            {
                var _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance.Parse(input);
if (!_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._commonExpr)!, input);
}

var _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1 = __GeneratedOdataV3.Parsers.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser.Instance.Optional().Parse(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Remainder);
if (!_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._commonExpr)!, input);
}

var _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 = __GeneratedOdataV3.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance.Optional().Parse(_addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1.Remainder);
if (!_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._commonExpr)!, input);
}

var _andExprⳆorExpr_1 = __GeneratedOdataV3.Parsers.Inners._andExprⳆorExprParser.Instance.Optional().Parse(_eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Remainder);
if (!_andExprⳆorExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._commonExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._commonExpr(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Parsed, _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr_1.Parsed.GetOrElse(null), _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Parsed.GetOrElse(null), _andExprⳆorExpr_1.Parsed.GetOrElse(null)), _andExprⳆorExpr_1.Remainder);
            }
        }
    }
    
}
