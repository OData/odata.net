namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _byteValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._byteValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._byteValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._byteValue> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, 3).Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._byteValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._byteValue(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To3<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
