namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall> Parse(IInput<char>? input)
            {
                var _primitiveFunctionImport_1 = __GeneratedOdataV3.Parsers.Rules._primitiveFunctionImportParser.Instance.Parse(input);
if (!_primitiveFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_primitiveFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveFunctionImportCall(_primitiveFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
