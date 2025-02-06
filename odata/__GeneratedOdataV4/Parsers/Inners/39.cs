namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_keyValuePairParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair)!, input);
}

var _keyValuePair_1 = __GeneratedOdataV4.Parsers.Rules._keyValuePairParser.Instance.Parse(_COMMA_1.Remainder);
if (!_keyValuePair_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair(_COMMA_1.Parsed, _keyValuePair_1.Parsed), _keyValuePair_1.Remainder);
            }
        }
    }
    
}
