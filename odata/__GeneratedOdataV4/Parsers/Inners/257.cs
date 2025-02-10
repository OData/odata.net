namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>
        {
            private static int CallCount = 0;

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                if (CallCount % 2 == 0)
                {
                    ++CallCount;

                    newStart = start;
                    for (; newStart < start + 2; ++newStart)
                    {
                        var next = input[newStart];
                    }

                    return default;
                }
                else if (CallCount % 2 == 1)
                {
                    ++CallCount;

                    newStart = start;
                    for (; newStart < start + 13; ++newStart)
                    {
                        var next = input[newStart];
                    }

                    return default;
                }

                newStart = start;
                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr> Instance { get; } = (_primitiveLiteralParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr>(_firstMemberExprParser.Instance);
        
        public static class _primitiveLiteralParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral> Parse(IInput<char>? input)
                {
                    var _primitiveLiteral_1 = __GeneratedOdataV4.Parsers.Rules._primitiveLiteralParser.Instance.Parse(input);
if (!_primitiveLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral(_primitiveLiteral_1.Parsed), _primitiveLiteral_1.Remainder);
                }
            }
        }
        
        public static class _arrayOrObjectParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject> Parse(IInput<char>? input)
                {
                    var _arrayOrObject_1 = __GeneratedOdataV4.Parsers.Rules._arrayOrObjectParser.Instance.Parse(input);
if (!_arrayOrObject_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._arrayOrObject(_arrayOrObject_1.Parsed), _arrayOrObject_1.Remainder);
                }
            }
        }
        
        public static class _rootExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr> Parse(IInput<char>? input)
                {
                    var _rootExpr_1 = __GeneratedOdataV4.Parsers.Rules._rootExprParser.Instance.Parse(input);
if (!_rootExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._rootExpr(_rootExpr_1.Parsed), _rootExpr_1.Remainder);
                }
            }
        }
        
        public static class _firstMemberExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr> Parse(IInput<char>? input)
                {
                    var _firstMemberExpr_1 = __GeneratedOdataV4.Parsers.Rules._firstMemberExprParser.Instance.Parse(input);
if (!_firstMemberExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr(_firstMemberExpr_1.Parsed), _firstMemberExpr_1.Remainder);
                }
            }
        }
        
        public static class _functionExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr> Parse(IInput<char>? input)
                {
                    var _functionExpr_1 = __GeneratedOdataV4.Parsers.Rules._functionExprParser.Instance.Parse(input);
if (!_functionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._functionExpr(_functionExpr_1.Parsed), _functionExpr_1.Remainder);
                }
            }
        }
        
        public static class _negateExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr> Parse(IInput<char>? input)
                {
                    var _negateExpr_1 = __GeneratedOdataV4.Parsers.Rules._negateExprParser.Instance.Parse(input);
if (!_negateExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._negateExpr(_negateExpr_1.Parsed), _negateExpr_1.Remainder);
                }
            }
        }
        
        public static class _methodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr> Parse(IInput<char>? input)
                {
                    var _methodCallExpr_1 = __GeneratedOdataV4.Parsers.Rules._methodCallExprParser.Instance.Parse(input);
if (!_methodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._methodCallExpr(_methodCallExpr_1.Parsed), _methodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _parenExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr> Parse(IInput<char>? input)
                {
                    var _parenExpr_1 = __GeneratedOdataV4.Parsers.Rules._parenExprParser.Instance.Parse(input);
if (!_parenExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._parenExpr(_parenExpr_1.Parsed), _parenExpr_1.Remainder);
                }
            }
        }
        
        public static class _listExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr> Parse(IInput<char>? input)
                {
                    var _listExpr_1 = __GeneratedOdataV4.Parsers.Rules._listExprParser.Instance.Parse(input);
if (!_listExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._listExpr(_listExpr_1.Parsed), _listExpr_1.Remainder);
                }
            }
        }
        
        public static class _castExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr> Parse(IInput<char>? input)
                {
                    var _castExpr_1 = __GeneratedOdataV4.Parsers.Rules._castExprParser.Instance.Parse(input);
if (!_castExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._castExpr(_castExpr_1.Parsed), _castExpr_1.Remainder);
                }
            }
        }
        
        public static class _isofExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr> Parse(IInput<char>? input)
                {
                    var _isofExpr_1 = __GeneratedOdataV4.Parsers.Rules._isofExprParser.Instance.Parse(input);
if (!_isofExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._isofExpr(_isofExpr_1.Parsed), _isofExpr_1.Remainder);
                }
            }
        }
        
        public static class _notExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr> Parse(IInput<char>? input)
                {
                    var _notExpr_1 = __GeneratedOdataV4.Parsers.Rules._notExprParser.Instance.Parse(input);
if (!_notExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._notExpr(_notExpr_1.Parsed), _notExpr_1.Remainder);
                }
            }
        }
    }
    
}
