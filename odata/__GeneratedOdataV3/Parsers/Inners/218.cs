namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_parameterNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName)!, input);
}

var _parameterName_1 = __GeneratedOdataV3.Parsers.Rules._parameterNameParser.Instance.Parse(_COMMA_1.Remainder);
if (!_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName(_COMMA_1.Parsed, _parameterName_1.Parsed), _parameterName_1.Remainder);
            }
        }
    }
    
}
