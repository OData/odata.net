namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_singleEnumValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue)!, input);
}

var _singleEnumValue_1 = __GeneratedOdataV4.Parsers.Rules._singleEnumValueParser.Instance.Parse(_COMMA_1.Remainder);
if (!_singleEnumValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_singleEnumValue(_COMMA_1.Parsed, _singleEnumValue_1.Parsed), _singleEnumValue_1.Remainder);
            }
        }
    }
    
}
