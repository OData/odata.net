namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexFunctionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall> Parse(IInput<char>? input)
            {
                var _complexFunctionImport_1 = __GeneratedOdataV3.Parsers.Rules._complexFunctionImportParser.Instance.Parse(input);
if (!_complexFunctionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall)!, input);
}

var _functionParameters_1 = __GeneratedOdataV3.Parsers.Rules._functionParametersParser.Instance.Parse(_complexFunctionImport_1.Remainder);
if (!_functionParameters_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._complexFunctionImportCall(_complexFunctionImport_1.Parsed, _functionParameters_1.Parsed), _functionParameters_1.Remainder);
            }
        }
    }
    
}
