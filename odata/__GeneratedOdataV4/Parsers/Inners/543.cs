namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_lineStringDataↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_lineStringDataↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_lineStringDataↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_lineStringDataↃ> Parse(IInput<char>? input)
            {
                var _COMMA_lineStringData_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_lineStringDataParser.Instance.Parse(input);
if (!_COMMA_lineStringData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_lineStringDataↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_lineStringDataↃ(_COMMA_lineStringData_1.Parsed), _COMMA_lineStringData_1.Remainder);
            }
        }
    }
    
}
