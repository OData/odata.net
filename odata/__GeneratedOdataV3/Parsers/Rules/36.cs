namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundFunctionCallNoParensParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens> Instance { get; } = (_namespace_ʺx2Eʺ_entityFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_entityColFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_complexFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_complexColFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_primitiveFunctionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_primitiveColFunctionParser.Instance);
        
        public static class _namespace_ʺx2Eʺ_entityFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction)!, input);
}

var _entityFunction_1 = __GeneratedOdataV3.Parsers.Rules._entityFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_entityFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _entityFunction_1.Parsed), _entityFunction_1.Remainder);
                }
            }
        }
        
        public static class _namespace_ʺx2Eʺ_entityColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction)!, input);
}

var _entityColFunction_1 = __GeneratedOdataV3.Parsers.Rules._entityColFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_entityColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _entityColFunction_1.Parsed), _entityColFunction_1.Remainder);
                }
            }
        }
        
        public static class _namespace_ʺx2Eʺ_complexFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction)!, input);
}

var _complexFunction_1 = __GeneratedOdataV3.Parsers.Rules._complexFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_complexFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _complexFunction_1.Parsed), _complexFunction_1.Remainder);
                }
            }
        }
        
        public static class _namespace_ʺx2Eʺ_complexColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction)!, input);
}

var _complexColFunction_1 = __GeneratedOdataV3.Parsers.Rules._complexColFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_complexColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _complexColFunction_1.Parsed), _complexColFunction_1.Remainder);
                }
            }
        }
        
        public static class _namespace_ʺx2Eʺ_primitiveFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction)!, input);
}

var _primitiveFunction_1 = __GeneratedOdataV3.Parsers.Rules._primitiveFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_primitiveFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _primitiveFunction_1.Parsed), _primitiveFunction_1.Remainder);
                }
            }
        }
        
        public static class _namespace_ʺx2Eʺ_primitiveColFunctionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction)!, input);
}

var _primitiveColFunction_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_primitiveColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _primitiveColFunction_1.Parsed), _primitiveColFunction_1.Remainder);
                }
            }
        }
    }
    
}
