namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundEntityFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall)!, input);
}

var _entityFunction_1 = __GeneratedOdataV3.Parsers.Rules._entityFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_entityFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_entityFunction_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundEntityFunctionCall(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _entityFunction_1.Parsed,  _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
