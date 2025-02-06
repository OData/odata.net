namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _enumValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._enumValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._enumValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._enumValue> Parse(IInput<char>? input)
            {
                var _singleEnumValue_1 = __GeneratedOdataV4.Parsers.Rules._singleEnumValueParser.Instance.Parse(input);
if (!_singleEnumValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enumValue)!, input);
}

var _ⲤCOMMA_singleEnumValueↃ_1 = Inners._ⲤCOMMA_singleEnumValueↃParser.Instance.Many().Parse(_singleEnumValue_1.Remainder);
if (!_ⲤCOMMA_singleEnumValueↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._enumValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._enumValue(_singleEnumValue_1.Parsed, _ⲤCOMMA_singleEnumValueↃ_1.Parsed), _ⲤCOMMA_singleEnumValueↃ_1.Remainder);
            }
        }
    }
    
}
