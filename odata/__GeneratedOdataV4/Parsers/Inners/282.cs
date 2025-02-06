namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ> Parse(IInput<char>? input)
            {
                var _functionExprParameter_1 = __GeneratedOdataV4.Parsers.Rules._functionExprParameterParser.Instance.Parse(input);
if (!_functionExprParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ)!, input);
}

var _ⲤCOMMA_functionExprParameterↃ_1 = Inners._ⲤCOMMA_functionExprParameterↃParser.Instance.Many().Parse(_functionExprParameter_1.Remainder);
if (!_ⲤCOMMA_functionExprParameterↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ(_functionExprParameter_1.Parsed, _ⲤCOMMA_functionExprParameterↃ_1.Parsed), _ⲤCOMMA_functionExprParameterↃ_1.Remainder);
            }
        }
    }
    
}
