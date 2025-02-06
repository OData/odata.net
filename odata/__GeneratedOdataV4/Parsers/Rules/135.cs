namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _functionExprParameterParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionExprParameter> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._functionExprParameter>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._functionExprParameter> Parse(IInput<char>? input)
            {
                var _parameterName_1 = __GeneratedOdataV4.Parsers.Rules._parameterNameParser.Instance.Parse(input);
if (!_parameterName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionExprParameter)!, input);
}

var _EQ_1 = __GeneratedOdataV4.Parsers.Rules._EQParser.Instance.Parse(_parameterName_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionExprParameter)!, input);
}

var _ⲤparameterAliasⳆparameterValueↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤparameterAliasⳆparameterValueↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_ⲤparameterAliasⳆparameterValueↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._functionExprParameter)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._functionExprParameter(_parameterName_1.Parsed, _EQ_1.Parsed, _ⲤparameterAliasⳆparameterValueↃ_1.Parsed), _ⲤparameterAliasⳆparameterValueↃ_1.Remainder);
            }
        }
    }
    
}
