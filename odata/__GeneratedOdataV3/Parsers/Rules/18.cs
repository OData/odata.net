namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitivePathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath> Instance { get; } = (_valueParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath>(_boundOperationParser.Instance);
        
        public static class _valueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._value> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._value>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._value> Parse(IInput<char>? input)
                {
                    var _value_1 = __GeneratedOdataV3.Parsers.Rules._valueParser.Instance.Parse(input);
if (!_value_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitivePath._value)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitivePath._value(_value_1.Parsed), _value_1.Remainder);
                }
            }
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._boundOperation> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._boundOperation>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitivePath._boundOperation> Parse(IInput<char>? input)
                {
                    var _boundOperation_1 = __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance.Parse(input);
if (!_boundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitivePath._boundOperation)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitivePath._boundOperation(_boundOperation_1.Parsed), _boundOperation_1.Remainder);
                }
            }
        }
    }
    
}
