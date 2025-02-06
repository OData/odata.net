namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr> Instance { get; } = (_addExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_subExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_mulExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_divExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_divbyExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr>(_modExprParser.Instance);
        
        public static class _addExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr> Parse(IInput<char>? input)
                {
                    var _addExpr_1 = __GeneratedOdataV4.Parsers.Rules._addExprParser.Instance.Parse(input);
if (!_addExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._addExpr(_addExpr_1.Parsed), _addExpr_1.Remainder);
                }
            }
        }
        
        public static class _subExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr> Parse(IInput<char>? input)
                {
                    var _subExpr_1 = __GeneratedOdataV4.Parsers.Rules._subExprParser.Instance.Parse(input);
if (!_subExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._subExpr(_subExpr_1.Parsed), _subExpr_1.Remainder);
                }
            }
        }
        
        public static class _mulExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr> Parse(IInput<char>? input)
                {
                    var _mulExpr_1 = __GeneratedOdataV4.Parsers.Rules._mulExprParser.Instance.Parse(input);
if (!_mulExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._mulExpr(_mulExpr_1.Parsed), _mulExpr_1.Remainder);
                }
            }
        }
        
        public static class _divExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr> Parse(IInput<char>? input)
                {
                    var _divExpr_1 = __GeneratedOdataV4.Parsers.Rules._divExprParser.Instance.Parse(input);
if (!_divExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divExpr(_divExpr_1.Parsed), _divExpr_1.Remainder);
                }
            }
        }
        
        public static class _divbyExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr> Parse(IInput<char>? input)
                {
                    var _divbyExpr_1 = __GeneratedOdataV4.Parsers.Rules._divbyExprParser.Instance.Parse(input);
if (!_divbyExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._divbyExpr(_divbyExpr_1.Parsed), _divbyExpr_1.Remainder);
                }
            }
        }
        
        public static class _modExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr> Parse(IInput<char>? input)
                {
                    var _modExpr_1 = __GeneratedOdataV4.Parsers.Rules._modExprParser.Instance.Parse(input);
if (!_modExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._addExprⳆsubExprⳆmulExprⳆdivExprⳆdivbyExprⳆmodExpr._modExpr(_modExpr_1.Parsed), _modExpr_1.Remainder);
                }
            }
        }
    }
    
}
