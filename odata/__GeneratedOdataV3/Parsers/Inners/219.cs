namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_parameterNameↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ> Parse(IInput<char>? input)
            {
                var _COMMA_parameterName_1 = __GeneratedOdataV3.Parsers.Inners._COMMA_parameterNameParser.Instance.Parse(input);
if (!_COMMA_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤCOMMA_parameterNameↃ(_COMMA_parameterName_1.Parsed), _COMMA_parameterName_1.Remainder);
            }
        }
    }
    
}
