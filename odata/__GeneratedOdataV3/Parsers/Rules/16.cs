namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _propertyPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath> Instance { get; } = (_entityColNavigationProperty_꘡collectionNavigation꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_entityNavigationProperty_꘡singleNavigation꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_complexColProperty_꘡complexColPath꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_complexProperty_꘡complexPath꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_primitiveColProperty_꘡primitiveColPath꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_primitiveProperty_꘡primitivePath꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath>(_streamProperty_꘡boundOperation꘡Parser.Instance);
        
        public static class _entityColNavigationProperty_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡> Parse(IInput<char>? input)
                {
                    var _entityColNavigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._entityColNavigationPropertyParser.Instance.Parse(input);
if (!_entityColNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡)!, input);
}

var _collectionNavigation_1 = __GeneratedOdataV3.Parsers.Rules._collectionNavigationParser.Instance.Optional().Parse(_entityColNavigationProperty_1.Remainder);
if (!_collectionNavigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡(_entityColNavigationProperty_1.Parsed, _collectionNavigation_1.Parsed.GetOrElse(null)), _collectionNavigation_1.Remainder);
                }
            }
        }
        
        public static class _entityNavigationProperty_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡> Parse(IInput<char>? input)
                {
                    var _entityNavigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._entityNavigationPropertyParser.Instance.Parse(input);
if (!_entityNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡)!, input);
}

var _singleNavigation_1 = __GeneratedOdataV3.Parsers.Rules._singleNavigationParser.Instance.Optional().Parse(_entityNavigationProperty_1.Remainder);
if (!_singleNavigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡(_entityNavigationProperty_1.Parsed, _singleNavigation_1.Parsed.GetOrElse(null)), _singleNavigation_1.Remainder);
                }
            }
        }
        
        public static class _complexColProperty_꘡complexColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡> Parse(IInput<char>? input)
                {
                    var _complexColProperty_1 = __GeneratedOdataV3.Parsers.Rules._complexColPropertyParser.Instance.Parse(input);
if (!_complexColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡)!, input);
}

var _complexColPath_1 = __GeneratedOdataV3.Parsers.Rules._complexColPathParser.Instance.Optional().Parse(_complexColProperty_1.Remainder);
if (!_complexColPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexColProperty_꘡complexColPath꘡(_complexColProperty_1.Parsed, _complexColPath_1.Parsed.GetOrElse(null)), _complexColPath_1.Remainder);
                }
            }
        }
        
        public static class _complexProperty_꘡complexPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡> Parse(IInput<char>? input)
                {
                    var _complexProperty_1 = __GeneratedOdataV3.Parsers.Rules._complexPropertyParser.Instance.Parse(input);
if (!_complexProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡)!, input);
}

var _complexPath_1 = __GeneratedOdataV3.Parsers.Rules._complexPathParser.Instance.Optional().Parse(_complexProperty_1.Remainder);
if (!_complexPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._complexProperty_꘡complexPath꘡(_complexProperty_1.Parsed, _complexPath_1.Parsed.GetOrElse(null)), _complexPath_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColProperty_꘡primitiveColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡> Parse(IInput<char>? input)
                {
                    var _primitiveColProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColPropertyParser.Instance.Parse(input);
if (!_primitiveColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡)!, input);
}

var _primitiveColPath_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColPathParser.Instance.Optional().Parse(_primitiveColProperty_1.Remainder);
if (!_primitiveColPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveColProperty_꘡primitiveColPath꘡(_primitiveColProperty_1.Parsed, _primitiveColPath_1.Parsed.GetOrElse(null)), _primitiveColPath_1.Remainder);
                }
            }
        }
        
        public static class _primitiveProperty_꘡primitivePath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡> Parse(IInput<char>? input)
                {
                    var _primitiveProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitivePropertyParser.Instance.Parse(input);
if (!_primitiveProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡)!, input);
}

var _primitivePath_1 = __GeneratedOdataV3.Parsers.Rules._primitivePathParser.Instance.Optional().Parse(_primitiveProperty_1.Remainder);
if (!_primitivePath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._primitiveProperty_꘡primitivePath꘡(_primitiveProperty_1.Parsed, _primitivePath_1.Parsed.GetOrElse(null)), _primitivePath_1.Remainder);
                }
            }
        }
        
        public static class _streamProperty_꘡boundOperation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡> Parse(IInput<char>? input)
                {
                    var _streamProperty_1 = __GeneratedOdataV3.Parsers.Rules._streamPropertyParser.Instance.Parse(input);
if (!_streamProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡)!, input);
}

var _boundOperation_1 = __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance.Optional().Parse(_streamProperty_1.Remainder);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._propertyPath._streamProperty_꘡boundOperation꘡(_streamProperty_1.Parsed, _boundOperation_1.Parsed.GetOrElse(null)), _boundOperation_1.Remainder);
                }
            }
        }
    }
    
}
