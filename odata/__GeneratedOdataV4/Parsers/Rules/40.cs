namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall> Parse(IInput<char>? input)
            {
                var _complexColFunctionImport_1 = __GeneratedOdataV4.Parsers.Rules._complexColFunctionImportParser.Instance.Parse(input);
if (!_complexColFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV4.Parsers.Rules._functionParametersParser.Instance.Parse(_complexColFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._complexColFunctionImportCall(_complexColFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
