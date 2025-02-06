namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr> Instance { get; } = (_collectionPathExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_singleNavigationExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_complexPathExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr>(_primitivePathExprParser.Instance);
        
        public static class _collectionPathExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr> Parse(IInput<char>? input)
                {
                    var _collectionPathExpr_1 = __GeneratedOdataV4.Parsers.Rules._collectionPathExprParser.Instance.Parse(input);
if (!_collectionPathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._collectionPathExpr(_collectionPathExpr_1.Parsed), _collectionPathExpr_1.Remainder);
                }
            }
        }
        
        public static class _singleNavigationExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr> Parse(IInput<char>? input)
                {
                    var _singleNavigationExpr_1 = __GeneratedOdataV4.Parsers.Rules._singleNavigationExprParser.Instance.Parse(input);
if (!_singleNavigationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._singleNavigationExpr(_singleNavigationExpr_1.Parsed), _singleNavigationExpr_1.Remainder);
                }
            }
        }
        
        public static class _complexPathExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr> Parse(IInput<char>? input)
                {
                    var _complexPathExpr_1 = __GeneratedOdataV4.Parsers.Rules._complexPathExprParser.Instance.Parse(input);
if (!_complexPathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._complexPathExpr(_complexPathExpr_1.Parsed), _complexPathExpr_1.Remainder);
                }
            }
        }
        
        public static class _primitivePathExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr> Parse(IInput<char>? input)
                {
                    var _primitivePathExpr_1 = __GeneratedOdataV4.Parsers.Rules._primitivePathExprParser.Instance.Parse(input);
if (!_primitivePathExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._collectionPathExprⳆsingleNavigationExprⳆcomplexPathExprⳆprimitivePathExpr._primitivePathExpr(_primitivePathExpr_1.Parsed), _primitivePathExpr_1.Remainder);
                }
            }
        }
    }
    
}
