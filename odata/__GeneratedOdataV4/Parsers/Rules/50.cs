namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _queryOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption> Instance { get; } = (_systemQueryOptionParser.Instance);
        
        public static class _systemQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._systemQueryOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._systemQueryOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._systemQueryOption> Parse(IInput<char>? input)
                {
                    var _systemQueryOption_1 = __GeneratedOdataV4.Parsers.Rules._systemQueryOptionParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOption._systemQueryOption(_systemQueryOption_1.Parsed), _systemQueryOption_1.Remainder);
                }
            }
        }
        
        public static class _aliasAndValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._aliasAndValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._aliasAndValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._aliasAndValue> Parse(IInput<char>? input)
                {
                    var _aliasAndValue_1 = __GeneratedOdataV4.Parsers.Rules._aliasAndValueParser.Instance.Parse(input);
if (!_aliasAndValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._queryOption._aliasAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOption._aliasAndValue(_aliasAndValue_1.Parsed), _aliasAndValue_1.Remainder);
                }
            }
        }
        
        public static class _nameAndValueParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._nameAndValue> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._nameAndValue>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._nameAndValue> Parse(IInput<char>? input)
                {
                    var _nameAndValue_1 = __GeneratedOdataV4.Parsers.Rules._nameAndValueParser.Instance.Parse(input);
if (!_nameAndValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._queryOption._nameAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOption._nameAndValue(_nameAndValue_1.Parsed), _nameAndValue_1.Remainder);
                }
            }
        }
        
        public static class _customQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._customQueryOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._customQueryOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOption._customQueryOption> Parse(IInput<char>? input)
                {
                    var _customQueryOption_1 = __GeneratedOdataV4.Parsers.Rules._customQueryOptionParser.Instance.Parse(input);
if (!_customQueryOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._queryOption._customQueryOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOption._customQueryOption(_customQueryOption_1.Parsed), _customQueryOption_1.Remainder);
                }
            }
        }
    }
    
}
