namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _minuteParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._minute> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._minute>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._minute> Parse(IInput<char>? input)
            {
                var _zeroToFiftyNine_1 = __GeneratedOdataV4.Parsers.Rules._zeroToFiftyNineParser.Instance.Parse(input);
if (!_zeroToFiftyNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._minute)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._minute(_zeroToFiftyNine_1.Parsed), _zeroToFiftyNine_1.Remainder);
            }
        }
    }
    
}
