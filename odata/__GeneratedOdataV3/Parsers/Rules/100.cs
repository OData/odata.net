namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nameAndValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nameAndValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._nameAndValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._nameAndValue> Parse(IInput<char>? input)
            {
                var _parameterName_1 = __GeneratedOdataV3.Parsers.Rules._parameterNameParser.Instance.Parse(input);
if (!_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameAndValue)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_parameterName_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameAndValue)!, input);
}

var _parameterValue_1 = __GeneratedOdataV3.Parsers.Rules._parameterValueParser.Instance.Parse(_EQ_1.Remainder);
if (!_parameterValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._nameAndValue(_parameterName_1.Parsed, _EQ_1.Parsed,  _parameterValue_1.Parsed), _parameterValue_1.Remainder);
            }
        }
    }
    
}
