namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _commonExprParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._commonExpr> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._commonExpr>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._commonExpr Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance2.Parse(input, start, out newStart);

                var _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 = __GeneratedOdataV4.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance2.Parse(input, newStart, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._commonExpr> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._commonExpr>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._commonExpr> Parse(IInput<char>? input)
            {
                var _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance.Parse(input);


var _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1 = __GeneratedOdataV4.Parsers.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser.Instance.Optional().Parse(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Remainder);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._commonExpr(_ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1.Parsed, null, _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Parsed.GetOrElse(null), null), _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr_1.Remainder);
            }
        }
    }
    
}
