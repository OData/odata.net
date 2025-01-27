namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._function> Instance { get; } = (_entityFunctionParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._function>(_entityColFunctionParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._function>(_complexFunctionParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._function>(_complexColFunctionParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._function>(_primitiveFunctionParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._function>(_primitiveColFunctionParser.Instance);
        
        public static class _entityFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._entityFunction> Instance { get; } = from _entityFunction_1 in __GeneratedOdata.Parsers.Rules._entityFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._entityFunction(_entityFunction_1);
        }
        
        public static class _entityColFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._entityColFunction> Instance { get; } = from _entityColFunction_1 in __GeneratedOdata.Parsers.Rules._entityColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._entityColFunction(_entityColFunction_1);
        }
        
        public static class _complexFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._complexFunction> Instance { get; } = from _complexFunction_1 in __GeneratedOdata.Parsers.Rules._complexFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._complexFunction(_complexFunction_1);
        }
        
        public static class _complexColFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._complexColFunction> Instance { get; } = from _complexColFunction_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._complexColFunction(_complexColFunction_1);
        }
        
        public static class _primitiveFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._primitiveFunction> Instance { get; } = from _primitiveFunction_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._primitiveFunction(_primitiveFunction_1);
        }
        
        public static class _primitiveColFunctionParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._function._primitiveColFunction> Instance { get; } = from _primitiveColFunction_1 in __GeneratedOdata.Parsers.Rules._primitiveColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._function._primitiveColFunction(_primitiveColFunction_1);
        }
    }
    
}
