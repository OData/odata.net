namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_singleEnumValueↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ> Parse(IInput<char>? input)
            {
                var _COMMA_singleEnumValue_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_singleEnumValueParser.Instance.Parse(input);
if (!_COMMA_singleEnumValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_singleEnumValueↃ(_COMMA_singleEnumValue_1.Parsed), _COMMA_singleEnumValue_1.Remainder);
            }
        }
    }
    
}
