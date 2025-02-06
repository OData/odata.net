namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_polygonDataↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_polygonDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_polygonDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_polygonDataↃ> Parse(IInput<char>? input)
            {
                var _COMMA_polygonData_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_polygonDataParser.Instance.Parse(input);
if (!_COMMA_polygonData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_polygonDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_polygonDataↃ(_COMMA_polygonData_1.Parsed), _COMMA_polygonData_1.Remainder);
            }
        }
    }
    
}
