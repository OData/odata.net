namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _doubleValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._doubleValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._doubleValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._doubleValue> Parse(IInput<char>? input)
            {
                var _decimalValue_1 = __GeneratedOdataV3.Parsers.Rules._decimalValueParser.Instance.Parse(input);
if (!_decimalValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._doubleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._doubleValue(_decimalValue_1.Parsed), _decimalValue_1.Remainder);
            }
        }
    }
    
}
