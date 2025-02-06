namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall> Parse(IInput<char>? input)
            {
                var _entityColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._entityColFunctionImportParser.Instance.Parse(input);
if (!_entityColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV4.Parsers.Rules._functionParametersParser.Instance.Parse(_entityColFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entityColFunctionImportCall(_entityColFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
