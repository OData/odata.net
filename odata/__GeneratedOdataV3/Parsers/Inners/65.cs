namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _functionParameter_ЖⲤCOMMA_functionParameterↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ> Parse(IInput<char>? input)
            {
                var _functionParameter_1 = __GeneratedOdataV3.Parsers.Rules._functionParameterParser.Instance.Parse(input);
if (!_functionParameter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ)!, input);
}

var _ⲤCOMMA_functionParameterↃ_1 = Inners._ⲤCOMMA_functionParameterↃParser.Instance.Many().Parse(_functionParameter_1.Remainder);
if (!_ⲤCOMMA_functionParameterↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ(_functionParameter_1.Parsed,  _ⲤCOMMA_functionParameterↃ_1.Parsed), _ⲤCOMMA_functionParameterↃ_1.Remainder);
            }
        }
    }
    
}
