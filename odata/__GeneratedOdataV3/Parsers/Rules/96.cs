namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterNamesParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._parameterNames> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._parameterNames>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._parameterNames> Parse(IInput<char>? input)
            {
                var _parameterName_1 = __GeneratedOdataV3.Parsers.Rules._parameterNameParser.Instance.Parse(input);
if (!_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._parameterNames)!, input);
}

var _ⲤCOMMA_parameterNameↃ_1 = Inners._ⲤCOMMA_parameterNameↃParser.Instance.Many().Parse(_parameterName_1.Remainder);
if (!_ⲤCOMMA_parameterNameↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._parameterNames)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._parameterNames(_parameterName_1.Parsed,  _ⲤCOMMA_parameterNameↃ_1.Parsed), _ⲤCOMMA_parameterNameↃ_1.Remainder);
            }
        }
    }
    
}
