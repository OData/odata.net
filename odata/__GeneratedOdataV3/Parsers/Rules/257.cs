namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function> Instance { get; } = (_entityFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._function>(_entityColFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._function>(_complexFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._function>(_complexColFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._function>(_primitiveFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._function>(_primitiveColFunctionParser.Instance);
        
        public static class _entityFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._entityFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._entityFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._entityFunction> Parse(IInput<char>? input)
                {
                    var _entityFunction_1 = __GeneratedOdataV3.Parsers.Rules._entityFunctionParser.Instance.Parse(input);
if (!_entityFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._entityFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._entityFunction(_entityFunction_1.Parsed), _entityFunction_1.Remainder);
                }
            }
        }
        
        public static class _entityColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._entityColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._entityColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._entityColFunction> Parse(IInput<char>? input)
                {
                    var _entityColFunction_1 = __GeneratedOdataV3.Parsers.Rules._entityColFunctionParser.Instance.Parse(input);
if (!_entityColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._entityColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._entityColFunction(_entityColFunction_1.Parsed), _entityColFunction_1.Remainder);
                }
            }
        }
        
        public static class _complexFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._complexFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._complexFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._complexFunction> Parse(IInput<char>? input)
                {
                    var _complexFunction_1 = __GeneratedOdataV3.Parsers.Rules._complexFunctionParser.Instance.Parse(input);
if (!_complexFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._complexFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._complexFunction(_complexFunction_1.Parsed), _complexFunction_1.Remainder);
                }
            }
        }
        
        public static class _complexColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._complexColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._complexColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._complexColFunction> Parse(IInput<char>? input)
                {
                    var _complexColFunction_1 = __GeneratedOdataV3.Parsers.Rules._complexColFunctionParser.Instance.Parse(input);
if (!_complexColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._complexColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._complexColFunction(_complexColFunction_1.Parsed), _complexColFunction_1.Remainder);
                }
            }
        }
        
        public static class _primitiveFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveFunction> Parse(IInput<char>? input)
                {
                    var _primitiveFunction_1 = __GeneratedOdataV3.Parsers.Rules._primitiveFunctionParser.Instance.Parse(input);
if (!_primitiveFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._primitiveFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._primitiveFunction(_primitiveFunction_1.Parsed), _primitiveFunction_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._function._primitiveColFunction> Parse(IInput<char>? input)
                {
                    var _primitiveColFunction_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColFunctionParser.Instance.Parse(input);
if (!_primitiveColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._function._primitiveColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._function._primitiveColFunction(_primitiveColFunction_1.Parsed), _primitiveColFunction_1.Remainder);
                }
            }
        }
    }
    
}
