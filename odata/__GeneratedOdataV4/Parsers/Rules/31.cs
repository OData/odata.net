namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _boundEntityColFunctionCallParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV4.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall)!, input);
}

var _entityColFunction_1 = __GeneratedOdataV4.Parsers.Rules._entityColFunctionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_entityColFunction_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV4.Parsers.Rules._functionParametersParser.Instance.Parse(_entityColFunction_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._boundEntityColFunctionCall(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _entityColFunction_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
