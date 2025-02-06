namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityIdOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption> Instance { get; } = (_formatParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption>(_customQueryOptionParser.Instance);
        
        public static class _formatParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._format> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._format>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._format> Parse(IInput<char>? input)
                {
                    var _format_1 = __GeneratedOdataV4.Parsers.Rules._formatParser.Instance.Parse(input);
if (!_format_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityIdOption._format)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityIdOption._format(_format_1.Parsed), _format_1.Remainder);
                }
            }
        }
        
        public static class _customQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._customQueryOption> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._customQueryOption>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityIdOption._customQueryOption> Parse(IInput<char>? input)
                {
                    var _customQueryOption_1 = __GeneratedOdataV4.Parsers.Rules._customQueryOptionParser.Instance.Parse(input);
if (!_customQueryOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityIdOption._customQueryOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityIdOption._customQueryOption(_customQueryOption_1.Parsed), _customQueryOption_1.Remainder);
                }
            }
        }
    }
    
}
