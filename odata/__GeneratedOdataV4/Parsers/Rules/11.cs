namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPropertyValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPropertyValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPropertyValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPropertyValue> Parse(IInput<char>? input)
            {
                var _primitiveLiteral_1 = __GeneratedOdataV4.Parsers.Rules._primitiveLiteralParser.Instance.Parse(input);
if (!_primitiveLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPropertyValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPropertyValue(_primitiveLiteral_1.Parsed), _primitiveLiteral_1.Remainder);
            }
        }
    }
    
}
