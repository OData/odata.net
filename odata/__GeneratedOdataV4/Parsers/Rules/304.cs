namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _fractionalSecondsParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._fractionalSeconds> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._fractionalSeconds>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._fractionalSeconds> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, 12).Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._fractionalSeconds)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._fractionalSeconds(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedFrom1To12<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
