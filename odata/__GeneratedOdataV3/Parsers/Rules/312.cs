namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _geoLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral> Instance { get; } = (_collectionLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_lineStringLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_multiPointLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_multiLineStringLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_multiPolygonLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_pointLiteralParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral>(_polygonLiteralParser.Instance);
        
        public static class _collectionLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._collectionLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._collectionLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._collectionLiteral> Parse(IInput<char>? input)
                {
                    var _collectionLiteral_1 = __GeneratedOdataV3.Parsers.Rules._collectionLiteralParser.Instance.Parse(input);
if (!_collectionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._collectionLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._collectionLiteral(_collectionLiteral_1.Parsed), _collectionLiteral_1.Remainder);
                }
            }
        }
        
        public static class _lineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._lineStringLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._lineStringLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._lineStringLiteral> Parse(IInput<char>? input)
                {
                    var _lineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._lineStringLiteralParser.Instance.Parse(input);
if (!_lineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._lineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._lineStringLiteral(_lineStringLiteral_1.Parsed), _lineStringLiteral_1.Remainder);
                }
            }
        }
        
        public static class _multiPointLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPointLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPointLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPointLiteral> Parse(IInput<char>? input)
                {
                    var _multiPointLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiPointLiteralParser.Instance.Parse(input);
if (!_multiPointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPointLiteral(_multiPointLiteral_1.Parsed), _multiPointLiteral_1.Remainder);
                }
            }
        }
        
        public static class _multiLineStringLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiLineStringLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiLineStringLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiLineStringLiteral> Parse(IInput<char>? input)
                {
                    var _multiLineStringLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiLineStringLiteralParser.Instance.Parse(input);
if (!_multiLineStringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiLineStringLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiLineStringLiteral(_multiLineStringLiteral_1.Parsed), _multiLineStringLiteral_1.Remainder);
                }
            }
        }
        
        public static class _multiPolygonLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPolygonLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPolygonLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPolygonLiteral> Parse(IInput<char>? input)
                {
                    var _multiPolygonLiteral_1 = __GeneratedOdataV3.Parsers.Rules._multiPolygonLiteralParser.Instance.Parse(input);
if (!_multiPolygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPolygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._multiPolygonLiteral(_multiPolygonLiteral_1.Parsed), _multiPolygonLiteral_1.Remainder);
                }
            }
        }
        
        public static class _pointLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._pointLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._pointLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._pointLiteral> Parse(IInput<char>? input)
                {
                    var _pointLiteral_1 = __GeneratedOdataV3.Parsers.Rules._pointLiteralParser.Instance.Parse(input);
if (!_pointLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._pointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._pointLiteral(_pointLiteral_1.Parsed), _pointLiteral_1.Remainder);
                }
            }
        }
        
        public static class _polygonLiteralParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._polygonLiteral> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._polygonLiteral>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._geoLiteral._polygonLiteral> Parse(IInput<char>? input)
                {
                    var _polygonLiteral_1 = __GeneratedOdataV3.Parsers.Rules._polygonLiteralParser.Instance.Parse(input);
if (!_polygonLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._geoLiteral._polygonLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._geoLiteral._polygonLiteral(_polygonLiteral_1.Parsed), _polygonLiteral_1.Remainder);
                }
            }
        }
    }
    
}
