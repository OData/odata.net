namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dummyStartRuleParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule> Instance { get; } = (_odataUriParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule>(_headerParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule>(_primitiveValueParser.Instance);
        
        public static class _odataUriParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri> Parse(IInput<char>? input)
                {
                    var _odataUri_1 = __GeneratedOdataV3.Parsers.Rules._odataUriParser.Instance.Parse(input);
if (!_odataUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._odataUri(_odataUri_1.Parsed), _odataUri_1.Remainder);
                }
            }
        }
        
        public static class _headerParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header> Parse(IInput<char>? input)
                {
                    var _header_1 = __GeneratedOdataV3.Parsers.Rules._headerParser.Instance.Parse(input);
if (!_header_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._header(_header_1.Parsed), _header_1.Remainder);
                }
            }
        }
        
        public static class _primitiveValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue> Parse(IInput<char>? input)
                {
                    var _primitiveValue_1 = __GeneratedOdataV3.Parsers.Rules._primitiveValueParser.Instance.Parse(input);
if (!_primitiveValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._dummyStartRule._primitiveValue(_primitiveValue_1.Parsed), _primitiveValue_1.Remainder);
                }
            }
        }
    }
    
}
