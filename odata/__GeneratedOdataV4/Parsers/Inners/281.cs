namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_functionExprParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ> Parse(IInput<char>? input)
            {
                var _COMMA_functionExprParameter_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_functionExprParameterParser.Instance.Parse(input);
if (!_COMMA_functionExprParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_functionExprParameterↃ(_COMMA_functionExprParameter_1.Parsed), _COMMA_functionExprParameter_1.Remainder);
            }
        }
    }
    
}
