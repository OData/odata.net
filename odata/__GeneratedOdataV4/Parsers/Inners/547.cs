namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_pointDataↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_pointDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_pointDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_pointDataↃ> Parse(IInput<char>? input)
            {
                var _COMMA_pointData_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_pointDataParser.Instance.Parse(input);
if (!_COMMA_pointData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_pointDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_pointDataↃ(_COMMA_pointData_1.Parsed), _COMMA_pointData_1.Remainder);
            }
        }
    }
    
}
