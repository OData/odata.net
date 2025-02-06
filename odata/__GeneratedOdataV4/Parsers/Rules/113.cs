namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _contextPropertyPathParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath> Instance { get; } = (_primitivePropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath>(_primitiveColPropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath>(_complexColPropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath>(_complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveProperty> Parse(IInput<char>? input)
                {
                    var _primitiveProperty_1 = __GeneratedOdataV4.Parsers.Rules._primitivePropertyParser.Instance.Parse(input);
if (!_primitiveProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveProperty(_primitiveProperty_1.Parsed), _primitiveProperty_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveColProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveColProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveColProperty> Parse(IInput<char>? input)
                {
                    var _primitiveColProperty_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColPropertyParser.Instance.Parse(input);
if (!_primitiveColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveColProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._primitiveColProperty(_primitiveColProperty_1.Parsed), _primitiveColProperty_1.Remainder);
                }
            }
        }
        
        public static class _complexColPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexColProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexColProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexColProperty> Parse(IInput<char>? input)
                {
                    var _complexColProperty_1 = __GeneratedOdataV4.Parsers.Rules._complexColPropertyParser.Instance.Parse(input);
if (!_complexColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexColProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexColProperty(_complexColProperty_1.Parsed), _complexColProperty_1.Remainder);
                }
            }
        }
        
        public static class _complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡> Parse(IInput<char>? input)
                {
                    var _complexProperty_1 = __GeneratedOdataV4.Parsers.Rules._complexPropertyParser.Instance.Parse(input);
if (!_complexProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡)!, input);
}

var _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1 = __GeneratedOdataV4.Parsers.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathParser.Instance.Optional().Parse(_complexProperty_1.Remainder);
if (!_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._contextPropertyPath._complexProperty_꘡꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath꘡(_complexProperty_1.Parsed, _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1.Parsed.GetOrElse(null)), _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath_1.Remainder);
                }
            }
        }
    }
    
}
