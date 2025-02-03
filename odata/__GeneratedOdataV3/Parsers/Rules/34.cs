namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundPrimitiveFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall)!, input);
}

var _primitiveFunction_1 = __GeneratedOdataV3.Parsers.Rules._primitiveFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_primitiveFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_primitiveFunction_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundPrimitiveFunctionCall(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _primitiveFunction_1.Parsed,  _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
