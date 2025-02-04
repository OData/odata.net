namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty> Instance { get; } = (_primitiveKeyPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty>(_primitiveNonKeyPropertyParser.Instance);
        
        public static class _primitiveKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty> Parse(IInput<char>? input)
                {
                    var _primitiveKeyProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveKeyPropertyParser.Instance.Parse(input);
if (!_primitiveKeyProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveKeyProperty(_primitiveKeyProperty_1.Parsed), _primitiveKeyProperty_1.Remainder);
                }
            }
        }
        
        public static class _primitiveNonKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty> Parse(IInput<char>? input)
                {
                    var _primitiveNonKeyProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveNonKeyPropertyParser.Instance.Parse(input);
if (!_primitiveNonKeyProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveProperty._primitiveNonKeyProperty(_primitiveNonKeyProperty_1.Parsed), _primitiveNonKeyProperty_1.Remainder);
                }
            }
        }
    }
    
}
