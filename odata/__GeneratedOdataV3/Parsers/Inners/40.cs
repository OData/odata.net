namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_keyValuePairↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ> Parse(IInput<char>? input)
            {
                var _COMMA_keyValuePair_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_keyValuePairParser.Instance.Parse(input);
if (!_COMMA_keyValuePair_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_keyValuePairↃ(_COMMA_keyValuePair_1.Parsed), _COMMA_keyValuePair_1.Remainder);
            }
        }
    }
    
}
