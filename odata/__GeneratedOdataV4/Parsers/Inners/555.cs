namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_ringLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ> Parse(IInput<char>? input)
            {
                var _COMMA_ringLiteral_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_ringLiteralParser.Instance.Parse(input);
if (!_COMMA_ringLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_ringLiteralↃ(_COMMA_ringLiteral_1.Parsed), _COMMA_ringLiteral_1.Remainder);
            }
        }
    }
    
}
