namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParametersParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionExprParameters> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionExprParameters>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._functionExprParameters> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExprParameters)!, input);
}

var _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1 = __GeneratedOdataV3.Parsers.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃParser.Instance.Optional().Parse(_OPEN_1.Remainder);
if (!_functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExprParameters)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionExprParameters)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._functionExprParameters(_OPEN_1.Parsed, _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1.Parsed.GetOrElse(null), _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
