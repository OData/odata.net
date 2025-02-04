namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall> Parse(IInput<char>? input)
            {
                var _entityFunctionImport_1 = __GeneratedOdataV3.Parsers.Rules._entityFunctionImportParser.Instance.Parse(input);
if (!_entityFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_entityFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._entityFunctionImportCall(_entityFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
