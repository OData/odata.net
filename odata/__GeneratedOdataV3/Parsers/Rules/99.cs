namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _aliasAndValueParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._aliasAndValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._aliasAndValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._aliasAndValue> Parse(IInput<char>? input)
            {
                var _parameterAlias_1 = __GeneratedOdataV3.Parsers.Rules._parameterAliasParser.Instance.Parse(input);
if (!_parameterAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._aliasAndValue)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_parameterAlias_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._aliasAndValue)!, input);
}

var _parameterValue_1 = __GeneratedOdataV3.Parsers.Rules._parameterValueParser.Instance.Parse(_EQ_1.Remainder);
if (!_parameterValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._aliasAndValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._aliasAndValue(_parameterAlias_1.Parsed, _EQ_1.Parsed,  _parameterValue_1.Parsed), _parameterValue_1.Remainder);
            }
        }
    }
    
}
