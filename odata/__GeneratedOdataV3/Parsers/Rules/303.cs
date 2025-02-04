namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _secondParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._second> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._second>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._second> Parse(IInput<char>? input)
            {
                var _zeroToFiftyNine_1 = __GeneratedOdataV3.Parsers.Rules._zeroToFiftyNineParser.Instance.Parse(input);
if (!_zeroToFiftyNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._second)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._second(_zeroToFiftyNine_1.Parsed), _zeroToFiftyNine_1.Remainder);
            }
        }
    }
    
}
