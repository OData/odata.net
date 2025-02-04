namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_geoLiteralↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_geoLiteralↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_geoLiteralↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_geoLiteralↃ> Parse(IInput<char>? input)
            {
                var _COMMA_geoLiteral_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_geoLiteralParser.Instance.Parse(input);
if (!_COMMA_geoLiteral_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_geoLiteralↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_geoLiteralↃ(_COMMA_geoLiteral_1.Parsed), _COMMA_geoLiteral_1.Remainder);
            }
        }
    }
    
}
