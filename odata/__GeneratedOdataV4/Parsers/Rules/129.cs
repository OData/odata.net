namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionPathExprParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr> Instance { get; } = (_count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_boundFunctionExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_annotationExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_anyExprParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr>(_ʺx2Fʺ_allExprParser.Instance);
        
        public static class _count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _count_1 = __GeneratedOdataV4.Parsers.Rules._countParser.Instance.Parse(input);
if (!_count_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡)!, input);
}

var _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1 = __GeneratedOdataV4.Parsers.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser.Instance.Optional().Parse(_count_1.Remainder);
if (!_OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._count_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡(_count_1.Parsed, _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Parsed.GetOrElse(null)), _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fʺ_boundFunctionExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr)!, input);
}

var _boundFunctionExpr_1 = __GeneratedOdataV4.Parsers.Rules._boundFunctionExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_boundFunctionExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_boundFunctionExpr(_ʺx2Fʺ_1.Parsed, _boundFunctionExpr_1.Parsed), _boundFunctionExpr_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fʺ_annotationExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr)!, input);
}

var _annotationExpr_1 = __GeneratedOdataV4.Parsers.Rules._annotationExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_annotationExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_annotationExpr(_ʺx2Fʺ_1.Parsed, _annotationExpr_1.Parsed), _annotationExpr_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fʺ_anyExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr)!, input);
}

var _anyExpr_1 = __GeneratedOdataV4.Parsers.Rules._anyExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_anyExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_anyExpr(_ʺx2Fʺ_1.Parsed, _anyExpr_1.Parsed), _anyExpr_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fʺ_allExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr)!, input);
}

var _allExpr_1 = __GeneratedOdataV4.Parsers.Rules._allExprParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_allExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionPathExpr._ʺx2Fʺ_allExpr(_ʺx2Fʺ_1.Parsed, _allExpr_1.Parsed), _allExpr_1.Remainder);
                }
            }
        }
    }
    
}
