namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundComplexFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall)!, input);
}

var _complexFunction_1 = __GeneratedOdataV3.Parsers.Rules._complexFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_complexFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_complexFunction_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._boundComplexFunctionCall(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _complexFunction_1.Parsed,  _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
