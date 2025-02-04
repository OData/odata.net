namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_functionParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionParameterↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionParameterↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionParameterↃ> Parse(IInput<char>? input)
            {
                var _COMMA_functionParameter_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_functionParameterParser.Instance.Parse(input);
if (!_COMMA_functionParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionParameterↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_functionParameterↃ(_COMMA_functionParameter_1.Parsed), _COMMA_functionParameter_1.Remainder);
            }
        }
    }
    
}
