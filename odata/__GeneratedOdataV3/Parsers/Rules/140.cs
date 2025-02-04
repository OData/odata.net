namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boolMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr> Instance { get; } = (_endsWithMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr>(_startsWithMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr>(_containsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr>(_intersectsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr>(_hasSubsetMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr>(_hasSubsequenceMethodCallExprParser.Instance);
        
        public static class _endsWithMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _endsWithMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._endsWithMethodCallExprParser.Instance.Parse(input);
if (!_endsWithMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._endsWithMethodCallExpr(_endsWithMethodCallExpr_1.Parsed), _endsWithMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _startsWithMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _startsWithMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._startsWithMethodCallExprParser.Instance.Parse(input);
if (!_startsWithMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._startsWithMethodCallExpr(_startsWithMethodCallExpr_1.Parsed), _startsWithMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _containsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _containsMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._containsMethodCallExprParser.Instance.Parse(input);
if (!_containsMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._containsMethodCallExpr(_containsMethodCallExpr_1.Parsed), _containsMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _intersectsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _intersectsMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._intersectsMethodCallExprParser.Instance.Parse(input);
if (!_intersectsMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._intersectsMethodCallExpr(_intersectsMethodCallExpr_1.Parsed), _intersectsMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _hasSubsetMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _hasSubsetMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._hasSubsetMethodCallExprParser.Instance.Parse(input);
if (!_hasSubsetMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsetMethodCallExpr(_hasSubsetMethodCallExpr_1.Parsed), _hasSubsetMethodCallExpr_1.Remainder);
                }
            }
        }
        
        public static class _hasSubsequenceMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr> Parse(IInput<char>? input)
                {
                    var _hasSubsequenceMethodCallExpr_1 = __GeneratedOdataV3.Parsers.Rules._hasSubsequenceMethodCallExprParser.Instance.Parse(input);
if (!_hasSubsequenceMethodCallExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boolMethodCallExpr._hasSubsequenceMethodCallExpr(_hasSubsequenceMethodCallExpr_1.Parsed), _hasSubsequenceMethodCallExpr_1.Remainder);
                }
            }
        }
    }
    
}
