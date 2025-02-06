namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_positionLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ> Parse(IInput<char>? input)
            {
                var _COMMA_positionLiteral_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_positionLiteralParser.Instance.Parse(input);
if (!_COMMA_positionLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_positionLiteralↃ(_COMMA_positionLiteral_1.Parsed), _COMMA_positionLiteral_1.Remainder);
            }
        }
    }
    
}
