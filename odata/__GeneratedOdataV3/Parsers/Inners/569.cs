namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_preferenceↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_preferenceↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_preferenceↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_preferenceↃ> Parse(IInput<char>? input)
            {
                var _COMMA_preference_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_preferenceParser.Instance.Parse(input);
if (!_COMMA_preference_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_preferenceↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_preferenceↃ(_COMMA_preference_1.Parsed), _COMMA_preference_1.Remainder);
            }
        }
    }
    
}
