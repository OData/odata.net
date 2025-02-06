namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundComplexColFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV4.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall)!, input);
}

var _complexColFunction_1 = __GeneratedOdataV4.Parsers.Rules._complexColFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_complexColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV4.Parsers.Rules._functionParametersParser.Instance.Parse(_complexColFunction_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._boundComplexColFunctionCall(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _complexColFunction_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
