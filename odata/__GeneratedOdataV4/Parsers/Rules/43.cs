namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionImportCallNoParensParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens> Instance { get; } = (_entityFunctionImportParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>(_entityColFunctionImportParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>(_complexFunctionImportParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>(_complexColFunctionImportParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>(_primitiveFunctionImportParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens>(_primitiveColFunctionImportParser.Instance);
        
        public static class _entityFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport> Parse(IInput<char>? input)
                {
                    var _entityFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._entityFunctionImportParser.Instance.Parse(input);
if (!_entityFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityFunctionImport(_entityFunctionImport_1.Parsed), _entityFunctionImport_1.Remainder);
                }
            }
        }
        
        public static class _entityColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport> Parse(IInput<char>? input)
                {
                    var _entityColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._entityColFunctionImportParser.Instance.Parse(input);
if (!_entityColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._entityColFunctionImport(_entityColFunctionImport_1.Parsed), _entityColFunctionImport_1.Remainder);
                }
            }
        }
        
        public static class _complexFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport> Parse(IInput<char>? input)
                {
                    var _complexFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._complexFunctionImportParser.Instance.Parse(input);
if (!_complexFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexFunctionImport(_complexFunctionImport_1.Parsed), _complexFunctionImport_1.Remainder);
                }
            }
        }
        
        public static class _complexColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport> Parse(IInput<char>? input)
                {
                    var _complexColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._complexColFunctionImportParser.Instance.Parse(input);
if (!_complexColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._complexColFunctionImport(_complexColFunctionImport_1.Parsed), _complexColFunctionImport_1.Remainder);
                }
            }
        }
        
        public static class _primitiveFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport> Parse(IInput<char>? input)
                {
                    var _primitiveFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._primitiveFunctionImportParser.Instance.Parse(input);
if (!_primitiveFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveFunctionImport(_primitiveFunctionImport_1.Parsed), _primitiveFunctionImport_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColFunctionImportParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport> Parse(IInput<char>? input)
                {
                    var _primitiveColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColFunctionImportParser.Instance.Parse(input);
if (!_primitiveColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionImportCallNoParens._primitiveColFunctionImport(_primitiveColFunctionImport_1.Parsed), _primitiveColFunctionImport_1.Remainder);
                }
            }
        }
    }
    
}
