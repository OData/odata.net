namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _portParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._port> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._port>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._port> Parse(IInput<char>? input)
            {
                var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Many().Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._port)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._port(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
            }
        }
    }
    
}
