namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall> Parse(IInput<char>? input)
            {
                var _primitiveColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColFunctionImportParser.Instance.Parse(input);
if (!_primitiveColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV4.Parsers.Rules._functionParametersParser.Instance.Parse(_primitiveColFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._primitiveColFunctionImportCall(_primitiveColFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
