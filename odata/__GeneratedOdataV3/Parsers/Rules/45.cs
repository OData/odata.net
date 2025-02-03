namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionParameterParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionParameter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._functionParameter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._functionParameter> Parse(IInput<char>? input)
            {
                var _parameterName_1 = __GeneratedOdataV3.Parsers.Rules._parameterNameParser.Instance.Parse(input);
if (!_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameter)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_parameterName_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameter)!, input);
}

var _ⲤparameterAliasⳆprimitiveLiteralↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤparameterAliasⳆprimitiveLiteralↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_ⲤparameterAliasⳆprimitiveLiteralↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._functionParameter)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._functionParameter(_parameterName_1.Parsed, _EQ_1.Parsed,  _ⲤparameterAliasⳆprimitiveLiteralↃ_1.Parsed), _ⲤparameterAliasⳆprimitiveLiteralↃ_1.Remainder);
            }
        }
    }
    
}
