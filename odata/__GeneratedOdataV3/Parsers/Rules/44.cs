namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParametersParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionParameters> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionParameters>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._functionParameters> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameters)!, input);
}

var _functionParameter_ЖⲤCOMMA_functionParameterↃ_1 = __GeneratedOdataV3.Parsers.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃParser.Instance.Optional().Parse(_OPEN_1.Remainder);
if (!_functionParameter_ЖⲤCOMMA_functionParameterↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameters)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_functionParameter_ЖⲤCOMMA_functionParameterↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameters)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._functionParameters(_OPEN_1.Parsed, _functionParameter_ЖⲤCOMMA_functionParameterↃ_1.Parsed.GetOrElse(null), _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
