namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._singleValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._singleValue> Parse(IInput<char>? input)
            {
                var _decimalValue_1 = __GeneratedOdataV4.Parsers.Rules._decimalValueParser.Instance.Parse(input);
if (!_decimalValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._singleValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._singleValue(_decimalValue_1.Parsed), _decimalValue_1.Remainder);
            }
        }
    }
    
}
