namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExprParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃParser.Instance2.Parse(input, start, out newStart);

                for (; newStart < start + 17; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr> Instance { get; } = (_eqExprParser.Instance);
        
        public static class _eqExprParser
        {
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr>
            {
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
                public __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
                {
                    newStart = start;
                    for (; newStart < start + 17; ++newStart)
                    {
                        var next = input[newStart];
                    }

                    return default;
                }
            }

            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr> Parse(IInput<char>? input)
                {
                    var _eqExpr_1 = __GeneratedOdataV4.Parsers.Rules._eqExprParser.Instance.Parse(input);
if (!_eqExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr(_eqExpr_1.Parsed), _eqExpr_1.Remainder);
                }
            }
        }
        
        public static class _neExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr> Parse(IInput<char>? input)
                {
                    var _neExpr_1 = __GeneratedOdataV4.Parsers.Rules._neExprParser.Instance.Parse(input);
if (!_neExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._neExpr(_neExpr_1.Parsed), _neExpr_1.Remainder);
                }
            }
        }
        
        public static class _ltExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr> Parse(IInput<char>? input)
                {
                    var _ltExpr_1 = __GeneratedOdataV4.Parsers.Rules._ltExprParser.Instance.Parse(input);
if (!_ltExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._ltExpr(_ltExpr_1.Parsed), _ltExpr_1.Remainder);
                }
            }
        }
        
        public static class _leExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr> Parse(IInput<char>? input)
                {
                    var _leExpr_1 = __GeneratedOdataV4.Parsers.Rules._leExprParser.Instance.Parse(input);
if (!_leExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._leExpr(_leExpr_1.Parsed), _leExpr_1.Remainder);
                }
            }
        }
        
        public static class _gtExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr> Parse(IInput<char>? input)
                {
                    var _gtExpr_1 = __GeneratedOdataV4.Parsers.Rules._gtExprParser.Instance.Parse(input);
if (!_gtExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._gtExpr(_gtExpr_1.Parsed), _gtExpr_1.Remainder);
                }
            }
        }
        
        public static class _geExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr> Parse(IInput<char>? input)
                {
                    var _geExpr_1 = __GeneratedOdataV4.Parsers.Rules._geExprParser.Instance.Parse(input);
if (!_geExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._geExpr(_geExpr_1.Parsed), _geExpr_1.Remainder);
                }
            }
        }
        
        public static class _hasExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr> Parse(IInput<char>? input)
                {
                    var _hasExpr_1 = __GeneratedOdataV4.Parsers.Rules._hasExprParser.Instance.Parse(input);
if (!_hasExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._hasExpr(_hasExpr_1.Parsed), _hasExpr_1.Remainder);
                }
            }
        }
        
        public static class _inExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr> Parse(IInput<char>? input)
                {
                    var _inExpr_1 = __GeneratedOdataV4.Parsers.Rules._inExprParser.Instance.Parse(input);
if (!_inExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._inExpr(_inExpr_1.Parsed), _inExpr_1.Remainder);
                }
            }
        }
    }
    
}
