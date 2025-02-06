namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_entitySetNameↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ> Parse(IInput<char>? input)
            {
                var _COMMA_entitySetName_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_entitySetNameParser.Instance.Parse(input);
if (!_COMMA_entitySetName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_entitySetNameↃ(_COMMA_entitySetName_1.Parsed), _COMMA_entitySetName_1.Remainder);
            }
        }
    }
    
}
